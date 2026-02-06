using UnityEngine;

namespace ShoelaceStudios.GridSystem
{
	public readonly struct ChunkCoordinate
	{
		public readonly Vector2Int ChunkIndex;
		public readonly Vector2Int LocalCell;

		public ChunkCoordinate(Vector2Int chunkIndex, Vector2Int localCell)
		{
			ChunkIndex = chunkIndex;
			LocalCell = localCell;
		}

		public static ChunkCoordinate FromGlobal(int globalX, int globalY, int chunkSize)
		{
			int chunkX = Mathf.FloorToInt((float)globalX / chunkSize);
			int chunkY = Mathf.FloorToInt((float)globalY / chunkSize);

			int localX = globalX - (chunkX * chunkSize);
			int localY = globalY - (chunkY * chunkSize);

			return new ChunkCoordinate(
				new Vector2Int(chunkX, chunkY),
				new Vector2Int(localX, localY)
			);
		}

		public Vector2Int ToGlobal(int chunkSize)
		{
			return new Vector2Int(
				ChunkIndex.x * chunkSize + LocalCell.x,
				ChunkIndex.y * chunkSize + LocalCell.y
			);
		}
	}
}