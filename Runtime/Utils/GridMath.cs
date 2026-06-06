using System.Collections.Generic;
using UnityEngine;

namespace ShoelaceStudios.GridSystem.Utils
{
    public static class GridMath
    {
        public static bool ValidateDimension(int value, int max)
        {
            return value >= 0 && value < max;
        }


        #region Distance

        public static bool IsWithinCircularRadius(Vector2Int origin, Vector2Int candidate, int radius)
        {
            int dx = candidate.x - origin.x;
            int dy = candidate.y - origin.y;
            return dx * dx + dy * dy <= radius * radius;
        }

        public static bool IsWithinSquareRadius(Vector2Int origin, Vector2Int candidate, int radius)
        {
            int dx = Mathf.Abs(candidate.x - origin.x);
            int dy = Mathf.Abs(candidate.y - origin.y);
            return dx <= radius && dy <= radius;
        }

        public static bool IsWithinManhattanDistance(Vector2Int origin, Vector2Int candidate, int distance)
        {
            int dx = Mathf.Abs(candidate.x - origin.x);
            int dy = Mathf.Abs(candidate.y - origin.y);
            return dx + dy <= distance;
        }

        public static float GetWorldDistance(Vector2Int a, Vector2Int b, float cellSize)
        {
            int dx = b.x - a.x;
            int dy = b.y - a.y;
            return Mathf.Sqrt(dx * dx + dy * dy) * cellSize;
        }

        public static bool IsWithinWorldRadius(Vector2Int origin, Vector2Int candidate, float worldRadius, float cellSize)
        {
            int dx = candidate.x - origin.x;
            int dy = candidate.y - origin.y;
            float radiusInCells = worldRadius / cellSize;
            return dx * dx + dy * dy <= radiusInCells * radiusInCells;
        }

        public static int ManhattanDistance(Vector2Int a, Vector2Int b)
        {
            return Mathf.Abs(b.x - a.x) + Mathf.Abs(b.y - a.y);
        }

        public static int KingDistance(Vector2Int a, Vector2Int b)
        {
            return Mathf.Max(Mathf.Abs(b.x - a.x), Mathf.Abs(b.y - a.y));
        }

        #endregion

        #region Area

        public static IEnumerable<Vector2Int> GetCellsInSquareArea(Vector2Int origin, int radius)
        {
            for (int dx = -radius; dx <= radius; dx++)
            for (int dy = -radius; dy <= radius; dy++)
                yield return new Vector2Int(origin.x + dx, origin.y + dy);
        }

        public static int GetCellsInSquareArea(Vector2Int origin, int radius, int gridWidth, int gridHeight, Vector2Int[] results)
        {
            int minX = Mathf.Max(0, origin.x - radius);
            int maxX = Mathf.Min(gridWidth - 1, origin.x + radius);
            int minY = Mathf.Max(0, origin.y - radius);
            int maxY = Mathf.Min(gridHeight - 1, origin.y + radius);

            int count = 0;
            for (int x = minX; x <= maxX; x++)
            for (int y = minY; y <= maxY; y++)
                results[count++] = new Vector2Int(x, y);

            return count;
        }

        #endregion

        #region Line

        public static IEnumerable<Vector2Int> GetCellsOnLine(Vector2Int start, Vector2Int end)
        {
            int x = start.x;
            int y = start.y;
            int dx = Mathf.Abs(end.x - x);
            int dy = Mathf.Abs(end.y - y);
            int stepX = x < end.x ? 1 : -1;
            int stepY = y < end.y ? 1 : -1;
            int error = dx - dy;

            while (true)
            {
                yield return new Vector2Int(x, y);

                if (x == end.x && y == end.y) break;

                int doubleError = 2 * error;

                if (doubleError > -dy)
                {
                    error -= dy;
                    x += stepX;
                }

                if (doubleError < dx)
                {
                    error += dx;
                    y += stepY;
                }
            }
        }

        public static int GetCellsOnLine(Vector2Int start, Vector2Int end, Vector2Int[] results)
        {
            int x = start.x;
            int y = start.y;
            int dx = Mathf.Abs(end.x - x);
            int dy = Mathf.Abs(end.y - y);
            int stepX = x < end.x ? 1 : -1;
            int stepY = y < end.y ? 1 : -1;
            int error = dx - dy;
            int count = 0;

            while (true)
            {
                results[count++] = new Vector2Int(x, y);

                if (x == end.x && y == end.y) break;

                int doubleError = 2 * error;

                if (doubleError > -dy)
                {
                    error -= dy;
                    x += stepX;
                }

                if (doubleError < dx)
                {
                    error += dx;
                    y += stepY;
                }
            }

            return count;
        }

        #endregion
    }
}
