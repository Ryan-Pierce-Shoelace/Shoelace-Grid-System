using System;
using ShoelaceStudios.GridSystem.Utils;
using UnityEngine;

namespace ShoelaceStudios.GridSystem
{
	public class FloodFillContext
	{
		public readonly int GridWidth;
		public readonly int MaxSteps;

		private readonly bool[] visited;
		private readonly Vector2Int[] frontier;
		private readonly int[] frontierDepth;
		private readonly Vector2Int[] results;
		private readonly int[] resultDepths;

		private readonly int boxSize;

		public bool IsUnlimited => MaxSteps == int.MaxValue;

		public FloodFillContext(int gridWidth, int gridHeight, FloodFillParams parameters)
		{
			GridWidth = gridWidth;
			MaxSteps = parameters.HasStepLimit ? parameters.MaxSteps : int.MaxValue;

			if (parameters.HasStepLimit)
			{
				int reachable = GridMath.DiamondSize(parameters.MaxSteps);
				boxSize = 2 * parameters.MaxSteps + 1;

				visited = new bool[boxSize * boxSize];
				frontier = new Vector2Int[reachable];
				frontierDepth = new int[reachable];
				results = new Vector2Int[reachable];
				resultDepths = parameters.TrackDepth ? new int[reachable] : null;
			}
			else
			{
				int cellCount = gridWidth * gridHeight;
				boxSize = 0;

				visited = new bool[cellCount];
				frontier = new Vector2Int[cellCount];
				frontierDepth = new int[cellCount];
				results = new Vector2Int[cellCount];
				resultDepths = parameters.TrackDepth ? new int[cellCount] : null;
			}
		}

		public void Reset()
		{
			Array.Clear(visited, 0, visited.Length);
		}

		public bool IsVisited(int x, int y, Vector2Int origin)
		{
			if (IsUnlimited)
				return visited[y * GridWidth + x];

			int lx = x - origin.x + MaxSteps;
			int ly = y - origin.y + MaxSteps;
			return visited[ly * boxSize + lx];
		}

		public void MarkVisited(int x, int y, Vector2Int origin)
		{
			if (IsUnlimited)
			{
				visited[y * GridWidth + x] = true;
				return;
			}

			int lx = x - origin.x + MaxSteps;
			int ly = y - origin.y + MaxSteps;
			visited[ly * boxSize + lx] = true;
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

		public bool HasResultDepths => resultDepths != null;

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