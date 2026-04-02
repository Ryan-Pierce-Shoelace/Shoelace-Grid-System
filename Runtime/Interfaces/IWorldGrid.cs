using System;
using System.Collections.Generic;
using ShoelaceStudios.GridSystem.Core;
using UnityEngine;

namespace ShoelaceStudios.GridSystem
{
	public interface IWorldGrid
	{
		int Width { get; }
		int Height { get; }
		float CellSize { get; }
		Vector3 Origin { get; }
		Bounds WorldBounds { get; }

		#region Coordinate Conversions

		Vector3 GetWorldFromCell(int x, int y);
		Vector3 GetWorldFromCell(Vector2Int cell);
		Vector2Int GetCellFromWorld(Vector3 worldPosition);

		#endregion

		#region Walls and Pathing

		bool IsBlockedCell(int x, int y);
		bool IsBlockedCell(Vector2Int cell);
		bool IsSlowCell(int x, int y);
		bool IsSlowCell(Vector2Int cell);
		bool IsWalkable(int x, int y);
		bool IsWalkable(Vector2Int cell);
		void AddWall(Vector2Int cell);
		void RemoveWall(Vector2Int cell);
		void SetWalls(IEnumerable<Vector2Int> wallCells);
		void ClearWalls();
		int GetAllWalls(Vector2Int[] buffer);

		#endregion

		#region Cell Flags

		CellFlags GetFlags(int x, int y);
		CellFlags GetFlags(Vector2Int cell);

		bool HasFlag(int x, int y, CellFlags flag);
		bool HasFlag(Vector2Int cell, CellFlags flag);

		void SetFlag(int x, int y, CellFlags flag);
		void SetFlag(Vector2Int cell, CellFlags flag);

		void ClearFlag(int x, int y, CellFlags flag);
		void ClearFlag(Vector2Int cell, CellFlags flag);

		void ClearAllOfFlag(CellFlags flag);
		void ClearAllFlags();

		#endregion

		#region Validation

		bool IsValidCell(int x, int y);
		bool IsValidCell(Vector2Int cell);

		#endregion

		#region Iterators

		void ForEachCell(Action<int, int> action);

		#endregion
	}
}