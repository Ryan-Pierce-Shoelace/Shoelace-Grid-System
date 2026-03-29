using System;
using System.Collections.Generic;
using ShoelaceStudios.GridSystem.FloodFill;
using ShoelaceStudios.GridSystem.Partition;
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
		[SerializeField] private bool buildPerimeterWall;
		[SerializeField] private Grid unityGrid;
		[Header("Tilemap")]
		[SerializeField] private Tilemap wallTilemap;
		[SerializeField] private TileBase wallTile;

		[Header("Gizmos")]
		[SerializeField] private bool showGizmos = true;
		[SerializeField] private bool showGizmosOnSelected = true;
		[SerializeField] private Color gridColor = new(1f, 1f, 1f, 0.1f);
		[SerializeField] private Color selectedGridColor = new(1f, 1f, 0f, 0.3f);

		public IWorldGrid Grid { get; private set; }
		public WorldPartition WorldPartition { get; private set; }
		public bool IsInitialized { get; private set; }

		private readonly Dictionary<string, IDataLayer> flatLayers = new();
		private readonly Dictionary<string, IDataLayer> spatialLayers = new();

		#region Setup

		protected override void Awake()
		{
			base.Awake();
			ValidateRefs();
		}

		protected void Start()
		{
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
			IsInitialized = true;

			if (buildPerimeterWall) BuildPerimeterWalls();
			PopulateWalls();
			OnInitialized();
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
				Debug.LogError($"[WorldGridManager] WallTilemap is not assigned on {gameObject.name}.", this);

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
				if (!IsBorderCell(x, y)) return;

				AddWall(new Vector2Int(x, y), true);
			});
		}

		protected virtual void PopulateWalls()
		{
			if (wallTilemap == null) return;

			Grid.ForEachCell((x, y) =>
			{
				if (wallTilemap.HasTile(new Vector3Int(x, y, 0)))
					AddWall(new Vector2Int(x, y));
			});
		}


		protected virtual void OnInitialized()
		{
		}

		public virtual void Reset()
		{
			Grid = null;
			IsInitialized = false;
		}

		#endregion

		#region DataLayers

		public DataGrid<T> AddDataLayer<T>(string layerName)
		{
			if (!IsInitialized)
			{
				Debug.LogError("[WorldGridManager] Call Initialize() before adding data layers.");
				return null;
			}

			if (flatLayers.ContainsKey(layerName))
			{
				Debug.LogWarning($"[WorldGridManager] Layer '{layerName}' already exists. Returning existing.");
				return GetDataLayer<T>(layerName);
			}

			DataGrid<T> grid = new(gridWidth, gridHeight, layerName);
			flatLayers[layerName] = grid;
			return grid;
		}

		public DataGrid<T> GetDataLayer<T>(string layerName)
		{
			return flatLayers.TryGetValue(layerName, out IDataLayer layer)
				? layer as DataGrid<T>
				: null;
		}


		public bool HasDataLayer(string layerName)
		{
			return flatLayers.ContainsKey(layerName);
		}

		public bool HasSpatialDataLayer(string layerName)
		{
			return spatialLayers.ContainsKey(layerName);
		}

		public void RemoveDataLayer(string layerName)
		{
			if (!flatLayers.Remove(layerName))
				Debug.LogWarning($"[WorldGridManager] Flat layer '{layerName}' not found.");
		}

		public void RemoveSpatialDataLayer(string layerName)
		{
			if (!spatialLayers.Remove(layerName))
				Debug.LogWarning($"[WorldGridManager] Spatial layer '{layerName}' not found.");
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

		private bool IsBorderCell(int x, int y)
		{
			return x == 0 || y == 0 || x == gridWidth - 1 || y == gridHeight - 1;
		}


		#region Public API - helpers

		public bool IsWallCell(int x, int y)
		{
			return Grid.IsWallCell(x, y);
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

		public HashSet<Vector2Int> FloodFill(Vector2Int start, FloodFillParams parameters)
		{
			return GridFloodFill.Execute(Grid, start, parameters);
		}

		public HashSet<Vector2Int> FloodFillInRegion(Vector2Int start, FloodFillParams parameters, ICollection<Vector2Int> allowedRegion)
		{
			return !allowedRegion.Contains(start) ? new HashSet<Vector2Int>() : GridFloodFill.Execute(Grid, start, parameters, allowedRegion);
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