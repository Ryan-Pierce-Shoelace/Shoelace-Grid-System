using System;
using System.Collections.Generic;
using ShoelaceStudios.GridSystem.Utils;
using UnityEngine;

namespace ShoelaceStudios.GridSystem.Core
{
	public class WorldGrid : IWorldGrid
	{
		public int Width => width;
		public int Height => height;
		public float CellSize => cellSize;
		public Vector3 Origin => gridOrigin;
		public Bounds WorldBounds => worldBounds;

		private readonly int width;
		private readonly int height;
		private readonly float cellSize;
		private readonly Vector3 gridOrigin;
		private readonly Vector3 cellCenterOffset;
		private readonly Bounds worldBounds;
		private readonly HashSet<Vector2Int> walls = new();


		public WorldGrid(int gridWidth, int gridHeight, float cellSize, Vector3 origin)
		{
			if (gridWidth <= 0 || gridHeight <= 0 || cellSize <= 0f)
				throw new ArgumentException($"Invalid WorldGrid: width={gridWidth} height={gridHeight} cellSize={cellSize}");

			width = gridWidth;
			height = gridHeight;
			this.cellSize = cellSize;
			gridOrigin = origin;
			cellCenterOffset = new Vector3(cellSize * 0.5f, cellSize * 0.5f, 0f);
			worldBounds = new Bounds(
				origin + cellCenterOffset + new Vector3((width - 1) * cellSize * 0.5f, (height - 1) * cellSize * 0.5f, 0f),
				new Vector3(width * cellSize, height * cellSize, 1f));
		}


		#region Coordinate Conversions

		public Vector3 GetWorldFromCell(int x, int y)
		{
			return new Vector3(x * cellSize, y * cellSize, 0f) + cellCenterOffset + gridOrigin;
		}

		public Vector3 GetWorldFromCell(Vector2Int cell)
		{
			return GetWorldFromCell(cell.x, cell.y);
		}

		public Vector2Int GetCellFromWorld(Vector3 worldPosition)
		{
			int x = Mathf.FloorToInt((worldPosition.x - gridOrigin.x) / cellSize);
			int y = Mathf.FloorToInt((worldPosition.y - gridOrigin.y) / cellSize);
			return new Vector2Int(x, y);
		}

		#endregion

		#region World Grid Operations

		public float GetDistanceBetween(Vector2Int cellA, Vector2Int cellB)
		{
			Vector3 worldA = GetWorldFromCell(cellA.x, cellA.y);
			Vector3 worldB = GetWorldFromCell(cellB.x, cellB.y);
			return Vector3.Distance(worldA, worldB);
		}

		public bool IsWithinRadius(Vector2Int center, Vector2Int target, float radius)
		{
			Vector3 worldCenter = GetWorldFromCell(center.x, center.y);
			Vector3 worldTarget = GetWorldFromCell(target.x, target.y);
			return (worldTarget - worldCenter).sqrMagnitude <= radius * radius;
		}

		public List<Vector2Int> GetCellsOverlappingCollider(Collider2D collider, float overlapThreshold = 0f)
		{
			if (!GridColliderUtils.TryGetColliderPoints(collider, out Vector2[] points))
				return new List<Vector2Int>();

			List<Vector2Int> result = new();
			GridBounds bounds = GridBounds.FromWorldBounds(collider.bounds, cellSize, gridOrigin);

			bounds.ForEachCell((x, y) =>
			{
				if (!IsValidCell(x, y)) return;

				Vector2[] corners = GridColliderUtils.GetCellCorners(GetWorldFromCell(x, y), cellSize);
				float overlap = GridColliderUtils.ComputePolygonOverlapRatio(points, corners);

				if (overlap >= overlapThreshold)
					result.Add(new Vector2Int(x, y));
			});

			return result;
		}

		public List<Vector2Int> GetNeighbors4(Vector2Int cell)
		{
			List<Vector2Int> result = new(4);
			foreach (Vector2Int dir in GridDirections.Cardinal)
			{
				Vector2Int neighbor = cell + dir;
				if (IsValidCell(neighbor))
					result.Add(neighbor);
			}

			return result;
		}

		public List<Vector2Int> GetNeighbors8(Vector2Int cell)
		{
			List<Vector2Int> result = new(8);
			foreach (Vector2Int dir in GridDirections.All8)
			{
				Vector2Int neighbor = cell + dir;
				if (IsValidCell(neighbor))
					result.Add(neighbor);
			}

			return result;
		}

		public List<Vector2Int> GetDiagonalNeighbors(Vector2Int cell)
		{
			List<Vector2Int> result = new(4);
			foreach (Vector2Int dir in GridDirections.Diagonal)
			{
				Vector2Int neighbor = cell + dir;
				if (IsValidCell(neighbor))
					result.Add(neighbor);
			}

			return result;
		}

		#endregion


		#region Iterators

		public void ForEachCell(Action<int, int> action)
		{
			for (int x = 0; x < width; x++)
			for (int y = 0; y < height; y++)
				action(x, y);
		}

		public void ForEachNeighbor4(Vector2Int cell, Action<Vector2Int> action)
		{
			foreach (Vector2Int dir in GridDirections.Cardinal)
			{
				Vector2Int neighbor = cell + dir;
				if (IsValidCell(neighbor))
					action(neighbor);
			}
		}

		public void ForEachNeighbor8(Vector2Int cell, Action<Vector2Int> action)
		{
			foreach (Vector2Int dir in GridDirections.All8)
			{
				Vector2Int neighbor = cell + dir;
				if (IsValidCell(neighbor))
					action(neighbor);
			}
		}

		public void ForEachNeighborDiagonal(Vector2Int cell, Action<Vector2Int> action)
		{
			foreach (Vector2Int dir in GridDirections.Diagonal)
			{
				Vector2Int neighbor = cell + dir;
				if (IsValidCell(neighbor))
					action(neighbor);
			}
		}

		#endregion


		#region Validation

		public bool IsValidCell(int x, int y)
		{
			return GridMath.ValidateDimension(x, width) &&
			       GridMath.ValidateDimension(y, height);
		}

		public bool IsValidCell(Vector2Int cell)
		{
			return IsValidCell(cell.x, cell.y);
		}

		#endregion

		#region Walls and Pathing

		public bool IsWallCell(int x, int y)
		{
			return walls.Contains(new Vector2Int(x, y));
		}

		public bool IsWallCell(Vector2Int cell)
		{
			return walls.Contains(cell);
		}

		public bool IsWalkable(int x, int y)
		{
			return IsValidCell(x, y) && !IsWallCell(x, y);
		}

		public bool IsWalkable(Vector2Int cell)
		{
			return IsWalkable(cell.x, cell.y);
		}

		public void AddWall(Vector2Int cell)
		{
			walls.Add(cell);
		}

		public void RemoveWall(Vector2Int cell)
		{
			walls.Remove(cell);
		}

		public void ClearWalls()
		{
			walls.Clear();
		}

		public void SetWalls(IEnumerable<Vector2Int> wallCells)
		{
			walls.Clear();
			foreach (Vector2Int cell in wallCells)
				if (IsValidCell(cell))
					walls.Add(cell);
		}

		public IEnumerable<Vector2Int> GetAllWalls()
		{
			return walls;
		}

		#endregion
	}
}