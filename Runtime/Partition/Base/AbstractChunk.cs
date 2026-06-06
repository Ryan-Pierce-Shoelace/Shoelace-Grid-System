using System;
using UnityEngine;

namespace ShoelaceStudios.GridSystem
{
    public abstract class AbstractChunk
    {
        public Vector2Int Index { get; }
        public int MinX { get; }
        public int MinY { get; }
        public int MaxX { get; }
        public int MaxY { get; }

        protected AbstractChunk(Vector2Int index, int chunkSize, int gridWidth, int gridHeight)
        {
            Index = index;
            MinX = index.x * chunkSize;
            MinY = index.y * chunkSize;
            MaxX = Mathf.Min(MinX + chunkSize - 1, gridWidth - 1);
            MaxY = Mathf.Min(MinY + chunkSize - 1, gridHeight - 1);
        }

        public void ForEachCell(Action<int, int> action)
        {
            for (int x = MinX; x <= MaxX; x++)
            for (int y = MinY; y <= MaxY; y++)
                action(x, y);
        }

        public static Vector2Int CellToChunkIndex(int x, int y, int chunkSize)
        {
            return new Vector2Int(
                Mathf.FloorToInt((float)x / chunkSize),
                Mathf.FloorToInt((float)y / chunkSize));
        }
    }
}
