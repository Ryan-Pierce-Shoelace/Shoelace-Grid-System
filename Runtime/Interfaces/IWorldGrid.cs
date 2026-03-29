using System;
using System.Collections.Generic;
using UnityEngine;

namespace ShoelaceStudios.GridSystem
{
	public interface IWorldGrid
	{
		int Width { get; }
		int Height { get; }
		float CellSize { get; }
		Vector3 Origin { get; }

		Vector3 GetWorldFromCell(int x, int y);
		Vector2Int GetCellFromWorld(Vector3 worldPosition);

		bool IsWallCell(int x, int y);
		bool IsWallCell(Vector2Int cell);

		bool IsWalkable(int x, int y);
		bool IsWalkable(Vector2Int cell);

		void AddWall(Vector2Int cell);
		void RemoveWall(Vector2Int cell);
		void SetWalls(IEnumerable<Vector2Int> wallCells);
		void ClearWalls();

		void ForEachCell(Action<int, int> action);

		bool IsValidCell(int x, int y);
		bool IsValidCell(Vector2Int cell);
	}
}