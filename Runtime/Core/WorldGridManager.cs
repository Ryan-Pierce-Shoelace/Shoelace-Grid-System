using System.Collections.Generic;
using ShoelaceStudios.GridSystem.Edges;
using ShoelaceStudios.Utilities.Singleton;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace ShoelaceStudios.GridSystem.Core
{
	public class WorldGridManager : Singleton<WorldGridManager>
	{
		[Header("Grid Settings")]
		[SerializeField] private int gridWidth;
		[SerializeField] private int gridHeight;
		[SerializeField] private float cellSize = 1f;
		[SerializeField] private int partitionChunkSize = 16;
		[SerializeField] private bool buildPerimeterWall;

		[Header("Tilemap")]
		[SerializeField] private Grid unityGrid;
		[SerializeField] private Tilemap wallTilemap;
		[SerializeField] private TileBase wallTile;

		[Header("Gizmos")]
		[SerializeField] private bool showGizmos = true;
		[SerializeField] private bool showGizmosOnSelected = true;
		[SerializeField] private Color gridColor = new(0, 1f, 1f);
		[SerializeField] private Color selectedGridColor = new(1f, 1f, 0f);
		public float CellSize => cellSize;
		public IWorldGrid Grid { get; private set; }
		public IEdgeSystem EdgeSystem { get; private set; }
		public WorldPartition WorldPartition { get; private set; }
		public bool IsInitialized { get; private set; }

		private readonly Dictionary<string, IDataLayer> layers = new();

		#region Setup

		protected override void Awake()
		{
			base.Awake();
			ValidateRefs();
			Initialize();
		}


		public virtual void Initialize()
		{
			if (IsInitialized)
			{
				Debug.LogWarning("[WorldGridManager] Already initialized. Call Reset() first.");
				return;
			}

			SyncUnityGrid();
			Grid = new WorldGrid(gridWidth, gridHeight, cellSize, transform.position);
			WorldPartition = new WorldPartition(Grid, partitionChunkSize);
			IsInitialized = true;

			TryInitializeEdgeManager();

			if (buildPerimeterWall) BuildPerimeterWalls();
			PopulateWalls();
			OnInitialized();
		}

		private void TryInitializeEdgeManager()
		{
			EdgeManager edgeManager = GetComponent<EdgeManager>();
			if (edgeManager == null) return;

			edgeManager.Initialize(Grid);
			EdgeSystem = edgeManager;
		}

		public void InitializeForEditor()
		{
			if (Grid != null) return;

			SyncUnityGrid();
			Grid = new WorldGrid(gridWidth, gridHeight, cellSize, transform.position);
		}


		private void SyncUnityGrid()
		{
			if (unityGrid == null) return;

			unityGrid.cellSize = new Vector3(cellSize, cellSize, 0f);
			unityGrid.transform.position = transform.position;
		}

		private void ValidateRefs()
		{
			if (wallTilemap == null)
				Debug.LogError($"[WorldGridManager] WallTilemap not assigned on {gameObject.name}.", this);

			if (gridWidth <= 0 || gridHeight <= 0 || cellSize <= 0f)
				Debug.LogError($"[WorldGridManager] Invalid grid dimensions: width={gridWidth} height={gridHeight} cellSize={cellSize}", this);
		}

		#if UNITY_EDITOR
		private void OnValidate()
		{
			UnityEditor.EditorApplication.delayCall += () =>
			{
				if (this == null) return;

				SyncUnityGrid();
			};
		}
		#endif

		protected virtual void BuildPerimeterWalls()
		{
			Grid.ForEachCell((x, y) =>
			{
				if (IsBorderCell(x, y))
					AddWall(new Vector2Int(x, y), true);
			});
		}

		protected virtual void PopulateWalls()
		{
			if (wallTilemap == null) return;

			List<Vector2Int> wallCells = new();
			Grid.ForEachCell((x, y) =>
			{
				Vector3 worldPos = Grid.GetWorldFromCell(x, y);
				Vector3Int tilemapCell = wallTilemap.WorldToCell(worldPos);
				if (wallTilemap.HasTile(tilemapCell))
					wallCells.Add(new Vector2Int(x, y));
			});
			Grid.SetWalls(wallCells);
		}

		private bool IsBorderCell(int x, int y)
		{
			return x == 0 || y == 0 || x == gridWidth - 1 || y == gridHeight - 1;
		}


		protected virtual void OnInitialized()
		{
		}

		public virtual void Reset()
		{
			Grid = null;
			WorldPartition = null;
			IsInitialized = false;
			EdgeSystem = null;
			layers.Clear();
		}

		#endregion

		#region Data Layers

		public DataGrid<T> AddDataLayer<T>(string layerName)
		{
			if (!IsInitialized)
			{
				Debug.LogError("[WorldGridManager] Call Initialize() before adding data layers.");
				return null;
			}

			if (layers.TryGetValue(layerName, out IDataLayer layer))
			{
				Debug.LogWarning($"[WorldGridManager] Layer '{layerName}' already exists. Returning existing.");
				return layer as DataGrid<T>;
			}

			DataGrid<T> grid = new(gridWidth, gridHeight, layerName);
			layers[layerName] = grid;
			return grid;
		}

		public DataGrid<T> GetDataLayer<T>(string layerName)
		{
			return layers.TryGetValue(layerName, out IDataLayer layer)
				? layer as DataGrid<T>
				: null;
		}

		public bool HasLayer(string layerName)
		{
			return layers.ContainsKey(layerName);
		}

		public void RemoveLayer(string layerName)
		{
			layers.Remove(layerName);
		}

		#endregion


		public virtual void AddWall(Vector2Int cell, bool paintTile = false)
		{
			if (!IsInitialized) return;

			Grid.AddWall(cell);
			if (paintTile && wallTilemap != null)
				wallTilemap.SetTile(new Vector3Int(cell.x, cell.y, 0), wallTile);
		}

		public virtual void RemoveWall(Vector2Int cell, bool clearTile = false)
		{
			if (!IsInitialized) return;

			Grid.RemoveWall(cell);
			if (clearTile && wallTilemap != null)
				wallTilemap.SetTile(new Vector3Int(cell.x, cell.y, 0), null);
		}


		#region Public API - helpers

		public bool IsWallCell(int x, int y)
		{
			return Grid.IsBlockedCell(x, y);
		}

		public bool IsWallCell(Vector2Int cell)
		{
			return IsWallCell(cell.x, cell.y);
		}

		public bool IsWalkable(int x, int y)
		{
			return Grid.IsWalkable(x, y);
		}

		public bool IsWalkable(Vector2Int cell)
		{
			return IsWalkable(cell.x, cell.y);
		}

		public Vector3 GetWorldFromCell(int x, int y)
		{
			return Grid.GetWorldFromCell(x, y);
		}

		public Vector3 GetWorldFromCell(Vector2Int cell)
		{
			return GetWorldFromCell(cell.x, cell.y);
		}

		public Vector2Int GetCellFromWorld(Vector3 worldPosition)
		{
			return Grid.GetCellFromWorld(worldPosition);
		}

		#endregion

		#region Public API - Flood Fill

		public FloodFillResult FloodFill(Vector2Int start, FloodFillParams parameters, FloodFillContext context)
		{
			return GridFloodFill.Execute(Grid, start, parameters, context);
		}

		#endregion


		#if UNITY_EDITOR
		private void OnDrawGizmos()
		{
			if (!showGizmos) return;

			DrawGridGizmos(gridColor);
		}

		private void OnDrawGizmosSelected()
		{
			if (!showGizmosOnSelected) return;

			DrawGridGizmos(selectedGridColor);
		}

		private void DrawGridGizmos(Color color)
		{
			Gizmos.color = color;

			Vector3 origin = transform.position;

			for (int x = 0; x < gridWidth; x++)
			for (int y = 0; y < gridHeight; y++)
			{
				Vector3 center = new Vector3(x * cellSize, y * cellSize, 0f)
				                 + new Vector3(cellSize * 0.5f, cellSize * 0.5f, 0f)
				                 + origin;

				Gizmos.DrawWireCube(center, new Vector3(cellSize, cellSize, 0f));
			}
		}
		#endif
	}
}