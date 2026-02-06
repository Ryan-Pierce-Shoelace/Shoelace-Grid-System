using System;
using System.Collections.Generic;
using ShoelaceStudios.Utilities.Singleton;
using UnityEngine;
using UnityEngine.Tilemaps;


namespace ShoelaceStudios.GridSystem
{
	public class WorldGridManager : Singleton<WorldGridManager>
	{
		[Header("Tilemap Refs")]
		[SerializeField] private Tilemap wallTileMap;
		[SerializeField] private Grid grid;
		[SerializeField] private TileBase wallTile;

		[Header("Grid Settings")]
		[SerializeField] private int gridWidth;
		[SerializeField] private int gridHeight;
		[SerializeField] private bool buildPerimeterWall;
		[SerializeField] private GridSettingsSO settings;

		public int GridWidth => gridWidth;
		public int GridHeight => gridHeight;
		public float CellSize => settings.CellSize;

		public float GridWorldWidth => gridWidth * CellSize;
		public float GridWorldHeight => gridHeight * CellSize;
		public Vector2 GridWorldSize => new Vector2(GridWorldWidth, GridWorldHeight);

		public event Action OnGridMapGeneration = delegate { };

		private GridTopology topology;
		private GridObstacles obstacles;
		private bool isInitialized;

		#region Validation

		protected override void Awake()
		{
			base.Awake();
			ValidateReferences();
		}

		private void ValidateReferences()
		{
			if (settings == null)
			{
				Debug.LogError($"[WorldGridManager] GridSettingsSO is NULL on {gameObject.name}! Assign it in the inspector.", this);
				enabled = false;
				return;
			}

			if (grid == null)
			{
				grid = GetComponent<Grid>();
				if (grid == null)
				{
					Debug.LogError($"[WorldGridManager] Grid component is NULL on {gameObject.name}! Add a Grid component.", this);
					enabled = false;
					return;
				}
			}

			if (wallTileMap == null)
			{
				Debug.LogError($"[WorldGridManager] WallTileMap is NULL on {gameObject.name}! Assign it in the inspector.", this);
				enabled = false;
			}
		}

		#endregion


		#region Setup

		public void InitWithSettings(GridSettingsSO gridSettingsSO)
		{
			settings = gridSettingsSO;
			InitializeGrid();
		}

		public void InitializeGrid()
		{
			if (isInitialized) return;

			topology = new GridTopology(gridWidth, gridHeight, CellSize, transform.position);
			obstacles.Initialize();

			if (buildPerimeterWall)
			{
				GenerateGridPerimeter();
			}

			GatherWallData();

			isInitialized = true;
			OnGridMapGeneration?.Invoke();
		}


		private void GenerateGridPerimeter()
		{
			topology.ForEach(
				(x, y) =>
				{
					if (!IsBorderCell(x, y)) return;

					Vector3Int tilePosition = new Vector3Int(x, y, 0);
					wallTileMap.SetTile(tilePosition, wallTile);
				});
		}

		private void GatherWallData()
		{
			obstacles.Clear();

			topology.ForEach(
				(x, y) =>
				{
					Vector3Int tilePosition = new Vector3Int(x, y, 0);
					if (wallTileMap.HasTile(tilePosition))
					{
						obstacles.AddWall(new Vector2Int(x, y));
					}
				});
		}

		private bool IsBorderCell(int x, int y)
		{
			return x == 0 || y == 0 || x == gridWidth - 1 || y == gridHeight - 1;
		}

		#endregion

		#region Public API - Validation

		public bool IsValidCell(Vector2Int cell)
		{
			return topology.IsValid(cell.x, cell.y);
		}

		public bool IsValidCell(int x, int y)
		{
			return topology.IsValid(x, y);
		}

		public bool IsWallCell(Vector2Int cell)
		{
			return obstacles.IsWall(cell);
		}

		public bool IsWallCell(int x, int y)
		{
			return obstacles.IsWall(new Vector2Int(x, y));
		}

		/// <summary>
		/// is within bounds and not blocked by wall
		/// </summary>
		public bool IsWalkable(Vector2Int cell)
		{
			return IsValidCell(cell) && !IsWallCell(cell);
		}

		/// <summary>
		/// is within bounds and not blocked by wall
		/// </summary>
		public bool IsWalkable(int x, int y)
		{
			return IsValidCell(x, y) && !IsWallCell(x, y);
		}

		#endregion


		#region Public API - Coordinate Conversion

		public Vector3 CellToWorldSpace(Vector2Int cell)
		{
			return topology.CellToWorldSpace(cell.x, cell.y);
		}

		public Vector3 CellToWorldSpace(int x, int y)
		{
			return topology.CellToWorldSpace(x, y);
		}

		public Vector2Int WorldToCell(Vector3 worldPosition)
		{
			return topology.WorldToCell(worldPosition, grid);
		}

		#endregion

		#region Public API - Wall managment

		public void AddWallCell(Vector2Int cell)
		{
			if (obstacles.IsWall(cell))
				return;

			obstacles.AddWall(cell);

			Vector3 worldPos = CellToWorldSpace(cell);
			Debug.DrawLine(worldPos, worldPos, Color.red, 10f);
		}

		public void RemoveWallCell(Vector2Int cell)
		{
			obstacles.RemoveWall(cell);
		}

		#endregion

		#region Public API - Grid Iteration

		public void ForEachCell(Action<int, int> action)
		{
			topology.ForEach(action);
		}

		public void ForEachNeighbor8(int x, int y, Action<int, int> action)
		{
			for (int nx = x - 1; nx <= x + 1; nx++)
			{
				for (int ny = y - 1; ny <= y + 1; ny++)
				{
					if (nx == x && ny == y)
						continue;

					if (IsValidCell(nx, ny))
					{
						action(nx, ny);
					}
				}
			}
		}

		public void ForEachNeighbor8(Vector2Int cell, Action<int, int> action)
		{
			ForEachNeighbor8(cell.x, cell.y, action);
		}

		public void ForEachNeighbor4(Vector2Int cell, Action<int, int> action)
		{
			foreach (Vector2Int dir in WorldGridUtilities.FourDirections)
			{
				int nx = cell.x + dir.x;
				int ny = cell.y + dir.y;

				if (IsValidCell(nx, ny))
				{
					action(nx, ny);
				}
			}
		}

		public List<Vector2Int> GetNeighbors8(Vector2Int cell)
		{
			List<Vector2Int> neighbors = new List<Vector2Int>(8);
			ForEachNeighbor8(cell, (x, y) => neighbors.Add(new Vector2Int(x, y)));
			return neighbors;
		}

		public List<Vector2Int> GetNeighbors4(Vector2Int cell)
		{
			List<Vector2Int> neighbors = new List<Vector2Int>(4);
			ForEachNeighbor4(cell, (x, y) => neighbors.Add(new Vector2Int(x, y)));
			return neighbors;
		}

		#endregion

		#region Public API - Spatial Queries

		/// <summary>
		/// Get all cells that overlap with a 2D collider.
		/// </summary>
		public List<Vector2Int> GetCellsOverlappingCollider(Collider2D col, float overlapThreshold = 0f)
		{
			return this.GetOverlappingCells(col, overlapThreshold);
		}

		/// <summary>
		/// Get all cells within a circular radius
		/// </summary>
		public List<Vector2Int> GetCellsInRadius(Vector2Int origin, int radius, bool requireLineOfSight = false, bool useCircularShape = true)
		{
			HashSet<Vector2Int> result = new();
			foreach (Vector2Int candidate in WorldGridUtilities.GetCellsInSquareArea(origin, radius))
			{
				if (!IsValidCell(candidate))
					continue;

				if (useCircularShape && !WorldGridUtilities.IsWithinCircularRadius(origin, candidate, radius))
					continue;

				if (requireLineOfSight && candidate != origin &&
				    !WorldGridUtilities.HasLineOfSight(origin, candidate, IsWallCell))
				{
					continue;
				}

				result.Add(candidate);
			}

			return new List<Vector2Int>(result);
		}

		/// <summary>
		/// Get all cells within radius from multiple origin points (combined result)
		/// </summary>
		public List<Vector2Int> GetCellsInRadius(IEnumerable<Vector2Int> origins, int radius, bool requireLineOfSight = false, bool useCircularShape = true)
		{
			HashSet<Vector2Int> result = new();

			foreach (Vector2Int origin in origins)
			{
				foreach (Vector2Int candidate in WorldGridUtilities.GetCellsInSquareArea(origin, radius))
				{
					if (!IsValidCell(candidate))
						continue;

					if (useCircularShape && !WorldGridUtilities.IsWithinCircularRadius(origin, candidate, radius))
						continue;

					if (requireLineOfSight && candidate != origin &&
					    !WorldGridUtilities.HasLineOfSight(origin, candidate, IsWallCell))
					{
						continue;
					}

					result.Add(candidate);
				}
			}

			return new List<Vector2Int>(result);
		}

		#endregion

		#region Public API - Flood Fill

		public HashSet<Vector2Int> FloodFill(Vector2Int start, FloodFillParams parameters)
		{
			return !IsValidCell(start) ? new HashSet<Vector2Int>() : ExecuteFloodFill(start, parameters, regionConstraint: null);
		}

		public HashSet<Vector2Int> FloodFillInRegion(Vector2Int start, ICollection<Vector2Int> allowedRegion, FloodFillParams parameters)
		{
			return !allowedRegion.Contains(start) ? new HashSet<Vector2Int>() : ExecuteFloodFill(start, parameters, regionConstraint: allowedRegion);
		}

		#endregion

		#region Private - Floodfill

		private HashSet<Vector2Int> ExecuteFloodFill(
			Vector2Int start,
			FloodFillParams parameters,
			ICollection<Vector2Int> regionConstraint)
		{
			HashSet<Vector2Int> visited = new HashSet<Vector2Int>();
			Queue<FloodFillState> frontier = new Queue<FloodFillState>();

			frontier.Enqueue(new FloodFillState(start, 0));
			visited.Add(start);

			float radiusSqr = parameters.MaxRadius * parameters.MaxRadius;
			Vector3 startWorld = parameters.HasRadiusLimit ? CellToWorldSpace(start) : Vector3.zero;

			while (frontier.Count > 0)
			{
				FloodFillState current = frontier.Dequeue();

				if (IsAtLimit(current, parameters, startWorld, radiusSqr))
					continue;

				foreach (Vector2Int direction in WorldGridUtilities.FourDirections)
				{
					Vector2Int neighbor = current.Cell + direction;

					if (!CanExpand(neighbor, visited, parameters, regionConstraint)) continue;

					visited.Add(neighbor);
					frontier.Enqueue(new FloodFillState(neighbor, current.Depth + 1));
				}
			}

			return visited;
		}

		private bool IsAtLimit(FloodFillState state, FloodFillParams parameters, Vector3 startWorld, float radiusSqr)
		{
			if (parameters.HasStepLimit && state.Depth >= parameters.MaxSteps)
				return true;

			if (!parameters.HasRadiusLimit) return false;

			Vector3 currentWorld = CellToWorldSpace(state.Cell);
			return (currentWorld - startWorld).sqrMagnitude > radiusSqr;
		}

		private bool CanExpand(Vector2Int cell, HashSet<Vector2Int> visited, FloodFillParams parameters, ICollection<Vector2Int> regionConstraint)
		{
			if (!IsValidCell(cell)) return false;

			if (visited.Contains(cell)) return false;

			if (parameters.StopAtWalls && IsWallCell(cell)) return false;

			return regionConstraint == null || regionConstraint.Contains(cell);
		}

		#endregion
	}
}