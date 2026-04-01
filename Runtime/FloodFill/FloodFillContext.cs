using System;
using UnityEngine;

namespace ShoelaceStudios.GridSystem
{
	public class FloodFillContext
	{
		public readonly int GridWidth;

		private readonly bool[] visited;
		private readonly Vector2Int[] frontier;
		private readonly int[] frontierDepth;
		private readonly Vector2Int[] results;
		private readonly int[] resultDepths;

		public bool IsUnlimited => true;
		public bool HasResultDepths => resultDepths != null;

		public FloodFillContext(int gridWidth, int gridHeight, FloodFillParams parameters)
		{
			GridWidth = gridWidth;

			int cellCount = gridWidth * gridHeight;

			visited = new bool[cellCount];
			frontier = new Vector2Int[cellCount];
			frontierDepth = new int[cellCount];
			results = new Vector2Int[cellCount];
			resultDepths = parameters.TrackDepth ? new int[cellCount] : null;
		}

		public void Reset()
		{
			Array.Clear(visited, 0, visited.Length);
		}

		public bool IsVisited(int x, int y)
		{
			return visited[y * GridWidth + x];
		}

		public void MarkVisited(int x, int y)
		{
			visited[y * GridWidth + x] = true;
		}

		public Vector2Int GetFrontier(int index)
		{
			return frontier[index];
		}

		public int GetFrontierDepth(int index)
		{
			return frontierDepth[index];
		}

		public Vector2Int GetResult(int index)
		{
			return results[index];
		}

		public int GetResultDepth(int index)
		{
			return resultDepths[index];
		}


		public void SetFrontier(int index, Vector2Int cell, int depth)
		{
			frontier[index] = cell;
			frontierDepth[index] = depth;
		}

		public void SetResult(int index, Vector2Int cell)
		{
			results[index] = cell;
		}

		public void SetResultDepth(int index, int depth)
		{
			if (resultDepths != null)
				resultDepths[index] = depth;
		}
	}
}