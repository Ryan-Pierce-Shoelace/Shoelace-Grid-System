using ShoelaceStudios.GridSystem.Utils;
using UnityEngine;

namespace ShoelaceStudios.GridSystem
{
    public static class GridFloodFill
    {
        public static FloodFillResult Execute(IWorldGrid grid, Vector2Int start, FloodFillParams parameters, FloodFillContext context)
        {
            context.Reset();

            if (!grid.IsValidCell(start.x, start.y))
                return new FloodFillResult(context, 0);

            // Don't seed from a wall when walls block.
            if (parameters.StopAtWalls && grid.IsBlockedCell(start.x, start.y))
                return new FloodFillResult(context, 0);

            Vector2Int[] directions = parameters.Diagonals
                ? GridDirections.All8
                : GridDirections.Cardinal;

            float radiusSqr = parameters.MaxRadius * parameters.MaxRadius;
            int head = 0;
            int tail = 0;
            int count = 0;

            Enqueue(context, start, 0, ref tail, ref count);

            while (head < tail)
            {
                Vector2Int current = context.GetFrontier(head);
                int depth = context.GetFrontierDepth(head);
                head++;

                if (parameters.HasStepLimit && depth >= parameters.MaxSteps)
                    continue;

                for (int d = 0; d < directions.Length; d++)
                {
                    int nx = current.x + directions[d].x;
                    int ny = current.y + directions[d].y;

                    if (!CanVisit(grid, current.x, current.y, nx, ny, start, parameters, radiusSqr, context))
                        continue;

                    Enqueue(context, new Vector2Int(nx, ny), depth + 1, ref tail, ref count);
                }
            }

            return new FloodFillResult(context, count);
        }

        private static bool CanVisit(
            IWorldGrid grid,
            int fromX,
            int fromY,
            int x,
            int y,
            Vector2Int origin,
            FloodFillParams parameters,
            float radiusSqr,
            FloodFillContext context)
        {
            if (!grid.IsValidCell(x, y)) return false;
            if (context.IsVisited(x, y)) return false;
            if (parameters.HasBounds && (x < parameters.BoundsMin.x || x > parameters.BoundsMax.x || y < parameters.BoundsMin.y || y > parameters.BoundsMax.y)) return false;
            if (parameters.StopAtWalls && grid.IsBlockedCell(x, y)) return false;
            if (parameters.StopAtWalls)
            {
                int dx = x - fromX;
                int dy = y - fromY;
                if (dx != 0 && dy != 0)
                {
                    if (grid.IsBlockedCell(x, fromY) || grid.IsBlockedCell(fromX, y))
                        return false;
                }
            }

            if (parameters.HasRegionMask && !parameters.RegionMask[y * grid.Width + x]) return false;
            if (parameters.HasEdgeSystem && !parameters.EdgeSystem.CanCrossEdge(fromX, fromY, x, y)) return false;
            if (parameters.HasRadiusLimit)
            {
                int rx = x - origin.x;
                int ry = y - origin.y;
                return rx * rx + ry * ry <= radiusSqr;
            }

            return true;
        }

        private static void Enqueue(FloodFillContext context, Vector2Int cell, int depth, ref int tail, ref int count)
        {
            context.MarkVisited(cell.x, cell.y);
            context.SetFrontier(tail++, cell, depth);
            context.SetResult(count, cell);
            context.SetResultDepth(count, depth);
            count++;
        }
    }
}
