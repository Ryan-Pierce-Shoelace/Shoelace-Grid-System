using UnityEngine;

namespace ShoelaceStudios.GridSystem
{
	public readonly struct FloodFillResult
	{
		private readonly FloodFillContext context;

		public readonly int Count;

		public FloodFillResult(FloodFillContext context, int count)
		{
			this.context = context;
			Count = count;
		}

		public Vector2Int this[int index] => context.GetResult(index);

		public bool Contains(int x, int y, Vector2Int origin)
		{
			return context.IsVisited(x, y, origin);
		}

		public bool Contains(Vector2Int cell, Vector2Int origin)
		{
			return Contains(cell.x, cell.y, origin);
		}

		public int GetDepth(int index)
		{
			return !context.HasResultDepths ? 0 : context.GetResultDepth(index);
		}

		public int GetCellsAtDepth(int depth, Vector2Int[] buffer)
		{
			if (!context.HasResultDepths) return 0;

			int count = 0;
			for (int i = 0; i < Count; i++)
				if (context.GetResultDepth(i) == depth)
					buffer[count++] = context.GetResult(i);
			return count;
		}

		public int GetMaxDepth()
		{
			if (!context.HasResultDepths) return 0;

			int max = 0;
			for (int i = 0; i < Count; i++)
			{
				int d = context.GetResultDepth(i);
				if (d > max) max = d;
			}

			return max;
		}

		public Vector2Int[] ToArray()
		{
			Vector2Int[] copy = new Vector2Int[Count];
			for (int i = 0; i < Count; i++)
				copy[i] = context.GetResult(i);
			return copy;
		}
	}
}