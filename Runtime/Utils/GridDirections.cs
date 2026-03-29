using UnityEngine;

namespace ShoelaceStudios.GridSystem.Utils
{
	public static class GridDirections
	{
		public static readonly Vector2Int[] Cardinal =
		{
			new(0, 1), // North
			new(0, -1), // South
			new(1, 0), // East
			new(-1, 0) // West
		};

		public static readonly Vector2Int[] All8 =
		{
			new(0, 1), // North
			new(0, -1), // South
			new(1, 0), // East
			new(-1, 0), // West
			new(1, 1), // NE
			new(-1, 1), // NW
			new(1, -1), // SE
			new(-1, -1) // SW
		};

		public static readonly Vector2Int[] Diagonal =
		{
			new(1, 1), // NE
			new(-1, 1), // NW
			new(1, -1), // SE
			new(-1, -1) // SW
		};
	}
}