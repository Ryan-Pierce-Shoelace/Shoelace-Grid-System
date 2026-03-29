using System.Collections.Generic;
using ShoelaceStudios.GridSystem.Utils;
using UnityEngine;

namespace ShoelaceStudios.GridSystem.FloodFill
{
	public static class GridFloodFill
	{
		public static HashSet<Vector2Int> Execute(
			IWorldGrid grid,
			Vector2Int start,
			FloodFillParams parameters,
			ICollection<Vector2Int> regionConstraint = null)
		{
			if (!grid.IsValidCell(start.x, start.y)) return new HashSet<Vector2Int>();
			if (regionConstraint != null && !regionConstraint.Contains(start)) return new HashSet<Vector2Int>();

			HashSet<Vector2Int> visited = new();
			Queue<FloodFillState> frontier = new();

			visited.Add(start);
			frontier.Enqueue(new FloodFillState(start, 0));

			float radiusSqr = parameters.MaxRadius * parameters.MaxRadius;
			Vector3 startWorld = parameters.HasRadiusLimit ? grid.GetWorldFromCell(start.x, start.y) : Vector3.zero;

			while (frontier.Count > 0)
			{
				FloodFillState current = frontier.Dequeue();

				if (IsAtLimit(grid, current, parameters, startWorld, radiusSqr))
					continue;

				foreach (Vector2Int dir in GridDirections.Cardinal)
				{
					Vector2Int neighbor = current.Cell + dir;

					if (!CanExpand(grid, neighbor, visited, parameters, regionConstraint))
						continue;

					visited.Add(neighbor);
					frontier.Enqueue(new FloodFillState(neighbor, current.Depth + 1));
				}
			}

			return visited;
		}


		#region private helpers

		private static bool IsAtLimit(
			IWorldGrid grid,
			FloodFillState state,
			FloodFillParams parameters,
			Vector3 startWorld,
			float radiusSqr)
		{
			if (parameters.HasStepLimit && state.Depth >= parameters.MaxSteps)
				return true;

			if (parameters.HasRadiusLimit)
			{
				Vector3 worldPos = grid.GetWorldFromCell(state.Cell.x, state.Cell.y);
				return (worldPos - startWorld).sqrMagnitude > radiusSqr;
			}

			return false;
		}

		private static bool CanExpand(
			IWorldGrid grid,
			Vector2Int cell,
			HashSet<Vector2Int> visited,
			FloodFillParams parameters,
			ICollection<Vector2Int> regionConstraint)
		{
			if (!grid.IsValidCell(cell.x, cell.y)) return false;
			if (visited.Contains(cell)) return false;
			if (parameters.StopAtWalls && grid.IsWallCell(cell.x, cell.y)) return false;
			if (regionConstraint != null && !regionConstraint.Contains(cell)) return false;

			return true;
		}


		private struct FloodFillState
		{
			public readonly Vector2Int Cell;
			public readonly int Depth;

			public FloodFillState(Vector2Int cell, int depth)
			{
				Cell = cell;
				Depth = depth;
			}
		}

		#endregion
	}
}