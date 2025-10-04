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

		public T this[int localX, int localY]
		{
			get => data[localX, localY];
			set => data[localX, localY] = value;
		}

		public bool TryGetLocal(int globalX, int globalY, out int localX, out int localY)
		{
			localX = globalX - (Origin.x * ChunkSize);
			localY = globalY - (Origin.y * ChunkSize);
			return ValidToChunk(localX, localY);
		}

		public Vector2Int GetGlobalCoords(Vector2Int localCoord)
		{
			return GetGlobalCoords(localCoord.x, localCoord.y);
		}

		public Vector2Int GetGlobalCoords(int localX, int localY)
		{
			return new Vector2Int(
				Origin.x * ChunkSize + localX,
				Origin.y * ChunkSize + localY
			);
		}

		public bool ValidToChunk(int x, int y)
		{
			return x >= 0 && y >= 0 && x < ChunkSize && y < ChunkSize;
		}


		public void ForEachChunkCell(Action<int, int> action)
		{
			for (int x = 0; x < ChunkSize; x++)
			{
				for (int y = 0; y < ChunkSize; y++)
				{
					action(x, y);
				}
			}
		}
	}
}