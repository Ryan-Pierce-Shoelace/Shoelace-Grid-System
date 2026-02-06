using System.Collections.Generic;
using UnityEngine;

namespace ShoelaceStudios.GridSystem
{
	public struct GridObstacles
	{
		private HashSet<Vector2Int> wallPositions;

		public void Initialize()
		{
			wallPositions = new HashSet<Vector2Int>();
		}

		public bool IsWall(Vector2Int coord) => wallPositions.Contains(coord);
        
		public void AddWall(Vector2Int coord) => wallPositions.Add(coord);
        
		public void RemoveWall(Vector2Int coord) => wallPositions.Remove(coord);

		public void Clear() => wallPositions.Clear();

		public IEnumerable<Vector2Int> AllWalls => wallPositions;
	}
}