using UnityEngine;

namespace ShoelaceStudios.GridSystem
{
	public struct GridTopology
	{
		private readonly int width;
		private readonly int height;
		private readonly float cellSize;
		private readonly Vector3 worldOrigin;
		
		public GridTopology(int width, int height, float cellSize, Vector3 worldOrigin)
		{
			this.width = width;
			this.height = height;
			this.cellSize = cellSize;
			this.worldOrigin = worldOrigin;
		}
		
		public Vector3 CellToWorldSpace(int x, int y)
		{
			return new Vector3(x, y, 0) * cellSize + (new Vector3(1, 1, 0) * cellSize * .5f);
		}

		public Vector2Int WorldToCell(Vector3 worldPosition, Grid grid)
		{
			Vector3Int cellPosition = grid.WorldToCell(worldPosition);
			return new Vector2Int(cellPosition.x, cellPosition.y);
		}

		public bool IsValid(int x, int y)
		{
			return x >= 0 && x < width && y >= 0 && y < height;
		}

		// public void ForEach(Action<int, int> action)
		// {
		// 	for (int x = 0; x < width; x++)
		// 	for (int y = 0; y < height; y++)
		// 		action(x, y);
		// }
	}
}