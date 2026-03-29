using System;
using UnityEngine;

namespace ShoelaceStudios.GridSystem.Partition
{
	public class DataChunk<T> : AbstractChunk
	{
		private readonly IGrid<T> grid;

		public bool HasData { get; private set; }
		public bool IsDirty { get; private set; }

		public DataChunk(Vector2Int index, int chunkSize, int gridWidth, int gridHeight, IGrid<T> grid)
			: base(index, chunkSize, gridWidth, gridHeight)
		{
			this.grid = grid;
		}

		#region State

		public void MarkDirty()
		{
			IsDirty = true;
		}

		public void ClearDirty()
		{
			IsDirty = false;
		}

		public void SetHasData(bool value)
		{
			HasData = value;
		}

		#endregion

		#region Queries

		public void ForEachCell(Action<int, int, T> action)
		{
			for (int x = MinX; x <= MaxX; x++)
			for (int y = MinY; y <= MaxY; y++)
				action(x, y, grid.GetValue(x, y));
		}

		public bool HasAny(Func<int, int, T, bool> predicate)
		{
			for (int x = MinX; x <= MaxX; x++)
			for (int y = MinY; y <= MaxY; y++)
				if (predicate(x, y, grid.GetValue(x, y)))
					return true;

			return false;
		}

		#endregion
	}
}