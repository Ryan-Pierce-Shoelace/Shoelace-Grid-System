using System;
using UnityEngine;

namespace ShoelaceStudios.GridSystem
{
	public class GridChunk<T> where T : IChunkData
	{
		public readonly Vector2Int Origin;
		public readonly int ChunkSize;
		private readonly T[,] data;

		public Vector3 WorldOrigin; // World position of the bottom-left of the chunk
		public Bounds WorldBounds; // Cached AABB in world space


		public GridChunk(Vector2Int origin, int chunkSize, float cellSize, Vector3 gridOrigin)
		{
			Origin = origin;
			ChunkSize = chunkSize;
			data = new T[chunkSize, chunkSize];

			WorldOrigin = new Vector3(origin.x * chunkSize * cellSize, origin.y * chunkSize * cellSize, 0f) + gridOrigin;
			Vector3 boundsSize = new(chunkSize * cellSize, chunkSize * cellSize, 1f);

			WorldBounds = new Bounds(WorldOrigin + boundsSize / 2f, boundsSize);
		}

		#region Indexer

		public T this[int localX, int localY]
		{
			get => data[localX, localY];
			set => data[localX, localY] = value;
		}

		public T this[Vector2Int localCell]
		{
			get => data[localCell.x, localCell.y];
			set => data[localCell.x, localCell.y] = value;
		}

		#endregion

		#region Coordinate Conversion

		public Vector2Int GlobalToLocal(int globalX, int globalY)
		{
			return new Vector2Int(globalX - (Origin.x * ChunkSize), globalY - (Origin.y * ChunkSize));
		}

		public Vector2Int GlobalToLocal(Vector2Int globalCell)
		{
			return GlobalToLocal(globalCell.x, globalCell.y);
		}

		public Vector2Int LocalToGlobal(int localX, int localY)
		{
			return new Vector2Int(Origin.x * ChunkSize + localX, Origin.y * ChunkSize + localY);
		}

		public Vector2Int LocalToGlobal(Vector2Int localCell)
		{
			return LocalToGlobal(localCell.x, localCell.y);
		}

		#endregion

		#region Iteration

		public void ForEachCell(Action<int, int> action)
		{
			for (int x = 0; x < ChunkSize; x++)
			{
				for (int y = 0; y < ChunkSize; y++)
				{
					action(x, y);
				}
			}
		}

		#endregion
	}
}