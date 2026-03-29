using System.Collections.Generic;
using UnityEngine;

namespace ShoelaceStudios.GridSystem.Utils
{
	public static class GridMath
	{
		public static bool ValidateDimension(int value, int max)
		{
			return value >= 0 && value < max;
		}

		public static bool IsWithinCircularRadius(Vector2Int origin, Vector2Int candidate, int radius)
		{
			int dx = candidate.x - origin.x;
			int dy = candidate.y - origin.y;
			return dx * dx + dy * dy <= radius * radius;
		}

		public static bool IsWithinSquareRadius(Vector2Int origin, Vector2Int candidate, int radius)
		{
			int dx = Mathf.Abs(candidate.x - origin.x);
			int dy = Mathf.Abs(candidate.y - origin.y);
			return dx <= radius && dy <= radius;
		}


		public static bool IsWithinManhattanDistance(Vector2Int origin, Vector2Int candidate, int distance)
		{
			int dx = Mathf.Abs(candidate.x - origin.x);
			int dy = Mathf.Abs(candidate.y - origin.y);
			return dx + dy <= distance;
		}

		public static bool HasLineOfSight(Vector2Int from, Vector2Int to, IWorldGrid grid)
		{
			foreach (Vector2Int cell in GetCellsOnLine(from, to))
				if (grid.IsWallCell(cell.x, cell.y))
					return false;

			return true;
		}

		public static IEnumerable<Vector2Int> GetCellsInSquareArea(Vector2Int origin, int radius)
		{
			for (int dx = -radius; dx <= radius; dx++)
			for (int dy = -radius; dy <= radius; dy++)
				yield return new Vector2Int(origin.x + dx, origin.y + dy);
		}

		public static IEnumerable<Vector2Int> GetCellsOnLine(Vector2Int start, Vector2Int end)
		{
			int x = start.x;
			int y = start.y;
			int dx = Mathf.Abs(end.x - x);
			int dy = Mathf.Abs(end.y - y);
			int stepX = x < end.x ? 1 : -1;
			int stepY = y < end.y ? 1 : -1;
			int error = dx - dy;

			while (true)
			{
				yield return new Vector2Int(x, y);

				if (x == end.x && y == end.y) break;

				int doubleError = 2 * error;

				if (doubleError > -dy)
				{
					error -= dy;
					x += stepX;
				}

				if (doubleError < dx)
				{
					error += dx;
					y += stepY;
				}
			}
		}
	}
}