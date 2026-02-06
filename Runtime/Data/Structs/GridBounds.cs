using UnityEngine;

namespace ShoelaceStudios.GridSystem
{
	public readonly struct GridBounds
	{
		private readonly int minX;
		private readonly int maxX;
		private readonly int minY;
		private readonly int maxY;

		public int MinX => minX;
		public int MaxX => maxX;
		public int MinY => minY;
		public int MaxY => maxY;

		public GridBounds(int minXBound, int maxXBound, int minYBound, int maxYBound)
		{
			minX = minXBound;
			maxX = maxXBound;
			minY = minYBound;
			maxY = maxYBound;
		}

		public int Width => maxX - minX + 1;
		public int Height => maxY - minY + 1;
		public int CellCount => Width * Height;
		public Vector2Int Min => new(minX, minY);
		public Vector2Int Max => new(maxX, maxY);
		public Vector2Int Size => new(Width, Height);

		public void ForEachCell(System.Action<int, int> action)
		{
			for (int x = minX; x <= maxX; x++)
			{
				for (int y = minY; y <= maxY; y++)
				{
					action(x, y);
				}
			}
		}

		public bool Contains(int x, int y)
		{
			return x >= minX && x <= maxX && y >= minY && y <= maxY;
		}

		public bool Contains(Vector2Int cell)
		{
			return Contains(cell.x, cell.y);
		}

		public static GridBounds FromWorldBounds(Bounds worldBounds, float cellSize)
		{
			return new GridBounds(
				Mathf.FloorToInt(worldBounds.min.x / cellSize),
				Mathf.CeilToInt(worldBounds.max.x / cellSize),
				Mathf.FloorToInt(worldBounds.min.y / cellSize),
				Mathf.CeilToInt(worldBounds.max.y / cellSize)
			);
		}
	}
}


