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


		public static int DiamondSize(int r)
		{
			return 2 * r * (r + 1) + 1;
		}

		public static IEnumerable<Vector2Int> GetCellsInSquareArea(Vector2Int origin, int radius)
		{
			for (int dx = -radius; dx <= radius; dx++)
			for (int dy = -radius; dy <= radius; dy++)
				yield return new Vector2Int(origin.x + dx, origin.y + dy);
		}
		//TODO as per the RedBlog article we can actually improve this a lot by passing in the grid dimensions
		// Maybe even make a system that has to PASS in an arry to mutate? so you make an array or whatever in the los checker script and it is updated by this and checks those. That way we dont make a million lists 
		// I think this will work if we create a array elsewhere and pass it in to be mutated so clear it send it in => it is updated => for each cell  and we return int count so that we can loop over only the results that exist

		public static int GetCellsInSquareArea(
			Vector2Int origin,
			int radius,
			int gridWidth,
			int gridHeight,
			Vector2Int[] results) //Maybe end in NoAlloc since we use an outside bugger
		{
			int minX = Mathf.Max(0, origin.x - radius);
			int maxX = Mathf.Min(gridWidth - 1, origin.x + radius);
			int minY = Mathf.Max(0, origin.y - radius);
			int maxY = Mathf.Min(gridHeight - 1, origin.y + radius);

			int count = 0;
			for (int x = minX; x <= maxX; x++)
			for (int y = minY; y <= maxY; y++)
				results[count++] = new Vector2Int(x, y);

			return count;
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