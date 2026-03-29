using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ShoelaceStudios.GridSystem.Partition
{
	public class DataPartition<T> : AbstractPartition<DataChunk<T>>
	{
		private readonly IGrid<T> grid;

		public DataPartition(IGrid<T> grid, int width, int height, int chunkSize)
			: base(width, height, chunkSize)
		{
			this.grid = grid;

			for (int cx = 0; cx < ChunksX; cx++)
			for (int cy = 0; cy < ChunksY; cy++)
			{
				Vector2Int index = new(cx, cy);
				chunks[index] = new DataChunk<T>(index, chunkSize, width, height, grid);
			}
		}

		public void NotifyChanged(int x, int y, T value)
		{
			Vector2Int index = AbstractChunk.CellToChunkIndex(x, y, chunkSize);
			if (!chunks.TryGetValue(index, out DataChunk<T> chunk)) return;

			chunk.MarkDirty();

			if (!EqualityComparer<T>.Default.Equals(value, default))
			{
				chunk.SetHasData(true);
			}
			else
			{
				bool stillHasData = chunk.HasAny((cx, cy, v) =>
					!EqualityComparer<T>.Default.Equals(v, default));
				chunk.SetHasData(stillHasData);
			}
		}

		public void ForEachActiveChunk(Action<DataChunk<T>> action)
		{
			foreach (DataChunk<T> chunk in chunks.Values)
				if (chunk.HasData)
					action(chunk);
		}

//TODO add a way to filter the chunks and have a foreach filtered or something
	}
}