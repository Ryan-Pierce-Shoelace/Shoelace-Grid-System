using UnityEngine;

namespace ShoelaceStudios.GridSystem
{
	public struct FloodFillState
	{
		public readonly Vector2Int Cell;
		public readonly int Depth;

		public FloodFillState(Vector2Int cell, int depth)
		{
			Cell = cell;
			Depth = depth;
		}
	}
}