using UnityEngine;

namespace ShoelaceStudios.GridSystem.Utils
{
	public static class GridDirections
	{
		public static readonly Vector2Int[] Cardinal =
		{
			new Vector2Int(0, 1), // North
			new Vector2Int(0, -1), // South
			new Vector2Int(1, 0), // East
			new Vector2Int(-1, 0), // West
		};

		public static readonly Vector2Int[] All8 =
		{
			new Vector2Int(0, 1), // North
			new Vector2Int(0, -1), // South
			new Vector2Int(1, 0), // East
			new Vector2Int(-1, 0), // West
			new Vector2Int(1, 1), // NE
			new Vector2Int(-1, 1), // NW
			new Vector2Int(1, -1), // SE
			new Vector2Int(-1, -1), // SW
		};

		public static readonly Vector2Int[] Diagonal =
		{
			new Vector2Int(1, 1), // NE
			new Vector2Int(-1, 1), // NW
			new Vector2Int(1, -1), // SE
			new Vector2Int(-1, -1), // SW
		};
	}
}