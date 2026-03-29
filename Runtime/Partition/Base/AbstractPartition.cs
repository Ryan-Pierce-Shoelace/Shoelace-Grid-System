using System;
using System.Collections.Generic;
using UnityEngine;

namespace ShoelaceStudios.GridSystem.Partition
{
	public abstract class AbstractPartition<TChunk> where TChunk : AbstractChunk
	{
		protected readonly int chunkSize;
		protected readonly Dictionary<Vector2Int, TChunk> chunks = new();

		public int ChunksX { get; }
		public int ChunksY { get; }
		public int ChunkSize => chunkSize;

		protected AbstractPartition(int width, int height, int chunkSize)
		{
			this.chunkSize = chunkSize;
			ChunksX = Mathf.CeilToInt((float)width / chunkSize);
			ChunksY = Mathf.CeilToInt((float)height / chunkSize);
		}

		public bool TryGetChunk(Vector2Int index, out TChunk chunk)
		{
			return chunks.TryGetValue(index, out chunk);
		}

		public TChunk GetChunkContaining(Vector2Int cell)
		{
			Vector2Int index = AbstractChunk.CellToChunkIndex(cell.x, cell.y, chunkSize);
			return chunks.TryGetValue(index, out TChunk chunk) ? chunk : throw new ArgumentOutOfRangeException($"Cell {cell} has no chunk.");
		}

		public IEnumerable<TChunk> GetAllChunks()
		{
			return chunks.Values;
		}

		public IEnumerable<Vector2Int> GetAllChunkIndices()
		{
			return chunks.Keys;
		}

		public void ForEachChunk(Action<TChunk> action)
		{
			foreach (TChunk chunk in chunks.Values)
				action(chunk);
		}
	}
}