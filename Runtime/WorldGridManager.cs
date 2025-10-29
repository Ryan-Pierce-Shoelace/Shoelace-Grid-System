using System;
using System.Collections.Generic;
using ShoelaceStudios.Utilities.Singleton;
using UnityEngine;
using UnityEngine.Tilemaps;

// ReSharper disable All

namespace ShoelaceStudios.GridSystem
{
    public class WorldGridManager : Singleton<WorldGridManager>
    {
        [Header("Tilemap Refs")] [SerializeField]
        private Tilemap wallTileMap;

        [SerializeField] private Grid grid;
        [SerializeField] private TileBase wallTile;

        [Header("Grid Settings")] [SerializeField]
        private int gridWidth;

        [SerializeField] private int gridHeight;
        [SerializeField] private bool buildPerimeterWall;
        [SerializeField] private GridSettingsSO settings;

        [Header("GameGrids")] public int GridWidth => gridWidth;

        public int GridHeight => gridHeight;
        public Vector2 TotalWorldSize => new(gridWidth, gridHeight);
        public float CellSize => grid.cellSize.x;
        public Vector3 WorldPlacementOffset => settings.WorldPlacementOffset(gridHeight, gridWidth, transform);
        public event Action OnGridMapGeneration = delegate { };

        public HashSet<Vector2Int> WallPositions;

        #region Setup

        private void Start()
        {
            CreateGridMap();
        }

        public void CreateGridMap()
        {
            if (buildPerimeterWall)
            {
                GenerateGridPerimeter();
            }

            GatherGridData();

            OnGridMapGeneration?.Invoke();
        }

        private void GatherGridData()
        {
            WallPositions = new HashSet<Vector2Int>();

            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridHeight; y++)
                {
                    Vector3Int tilePosition = new Vector3Int(x, y, 0);

                    if (wallTileMap.HasTile(tilePosition))
                    {
                        WallPositions.Add((Vector2Int)tilePosition);
                    }
                }
            }
        }

        private void GenerateGridPerimeter()
        {
            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridHeight; y++)
                {
                    Vector3Int tilePosition = new Vector3Int(x, y, 0);
                    if (IsBorderTile(x, y))
                    {
                        wallTileMap.SetTile(tilePosition, wallTile);
                    }
                }
            }
        }

        #endregion

        #region Helpers

        public void ForEachGridPosition(Action<int, int> action)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridHeight; y++)
                {
                    action(x, y);
                }
            }
        }

        public void ForEachNeighbors(int posX, int posY, Action<int, int> action)
        {
            for (int x = posX - 1; x <= posX + 1; x++)
            {
                for (int y = posY - 1; y <= posY + 1; y++)
                {
                    if (x == posX && y == posY)
                        continue;

                    if (!IsValidCell(x, y))
                    {
                        continue;
                    }

                    action(x, y);
                }
            }
        }


        private bool IsBorderTile(int x, int y)
        {
            return x == 0 || y == 0 || x == gridWidth - 1 || y == gridHeight - 1;
        }

        public Vector3 CellToWorldSpace(Vector2Int coord)
        {
            return CellToWorldSpace(coord.x, coord.y);
        }

        public Vector3 CellToWorldSpace(int x, int y)
        {
            return new Vector3(x, y, 0) * CellSize + (new Vector3(1, 1, 0) * CellSize * .5f);
        }

        public Vector2Int GetCell(Vector3 worldPosition)
        {
            Vector3Int cellPosition = grid.WorldToCell(worldPosition);
            cellPosition.z = 0;
            return IsValidCell(cellPosition.x, cellPosition.y) ? (Vector2Int)cellPosition : default;
        }

        public List<Vector2Int> GetOverlappingCells(Collider2D collider, float overlapThreshold = 0f)
        {
            return WorldGridUtilities.GetOverlappingCells(this, collider, overlapThreshold);
        }

        public List<Vector2Int> GetCellsInRadius(IEnumerable<Vector2Int> originCells, bool stopAtWalls, int radius,
            bool radialClipping = true)
        {
            HashSet<Vector2Int> radiusCells = new HashSet<Vector2Int>();

            foreach (var origin in originCells)
            {
                foreach (var candidate in WorldGridUtilities.GetCandidateCells(origin, radius))
                {
                    if (!IsValidCell(candidate))
                        continue;

                    if (!WorldGridUtilities.IsWithinRadius(origin, candidate, radius, radialClipping))
                        continue;

                    if (stopAtWalls && candidate != origin &&
                        WorldGridUtilities.IsBlockedByWalls(origin, candidate, IsWallCell))
                    {
                        continue;
                    }

                    radiusCells.Add(candidate);
                }
            }

            return new List<Vector2Int>(radiusCells);
        }

        public bool IsValidCell(Vector2Int coord)
        {
            return IsValidCell(coord.x, coord.y);
        }

        public bool IsValidCell(int x, int y)
        {
            return x >= 0 && x < gridWidth && y >= 0 && y < gridHeight;
        }

        public bool IsWallCell(Vector2Int coord)
        {
            return WallPositions.Contains(coord);
        }

        public bool IsWallCell(int x, int y)
        {
            return IsWallCell(new Vector2Int(x, y));
        }

        public void AddWallCell(Vector2Int coord)
        {
            if (!IsWallCell(coord))
            {
                WallPositions.Add(coord);
                Debug.DrawLine(CellToWorldSpace(coord) + (Vector3.one * CellSize * .5f),
                    CellToWorldSpace(coord) + (Vector3.one * CellSize * .5f), Color.red, 10f);
            }
        }

        public void RemoveWallCell(Vector2Int coord)
        {
            if (IsWallCell(coord))
            {
                WallPositions.Remove(coord);
            }
        }

        public void GetGridSpaceDimensions(out float width, out float height)
        {
            if (gridWidth == 0 || gridHeight == 0)
            {
                CreateGridMap();
            }

            width = (gridWidth) * CellSize;
            height = (gridHeight) * CellSize;
        }


        /// <summary>
        /// Flood fills from a start cell up to a given number of steps (or radius in world units).
        /// Stops expansion at walls if stopAtWalls = true.
        /// </summary>
        /// <param name="start">The origin cell to flood from.</param>
        /// <param name="steps">Number of grid steps to expand. Ignored if radius > 0.</param>
        /// <param name="radius">Optional world-space radius to expand. Overrides steps if > 0.</param>
        /// <param name="stopAtWalls">Whether to block propagation through wall cells.</param>
        /// <returns>A HashSet of all reached cells including the start cell.</returns>
        public HashSet<Vector2Int> FloodFill(Vector2Int start, int steps = 0, float radius = 0f,
            bool stopAtWalls = true)
        {
            HashSet<Vector2Int> visited = new HashSet<Vector2Int>();
            Queue<(Vector2Int cell, int depth)> frontier = new Queue<(Vector2Int, int)>();

            frontier.Enqueue((start, 0));
            visited.Add(start);

            float radiusSqr = radius > 0 ? radius * radius : float.MaxValue;
            float cellSize = CellSize;

            while (frontier.Count > 0)
            {
                var (current, depth) = frontier.Dequeue();

                // Limit by steps
                if (steps > 0 && depth >= steps)
                    continue;

                Vector3 worldPos = CellToWorldSpace(current);
                if (radius > 0f)
                {
                    Vector3 originWorld = CellToWorldSpace(start);
                    if ((worldPos - originWorld).sqrMagnitude > radiusSqr)
                        continue;
                }

                // Expand to 4 neighbors (N, S, E, W)
                foreach (Vector2Int offset in WorldGridUtilities.FourDirections)
                {
                    Vector2Int next = current + offset;

                    if (!IsValidCell(next) || visited.Contains(next))
                        continue;

                    if (stopAtWalls && IsWallCell(next))
                        continue;

                    visited.Add(next);
                    frontier.Enqueue((next, depth + 1));
                }
            }

            return visited;
        }

        /// <summary>
        /// Flood fills from a start cell up to a given number of steps (or radius),
        /// but only propagates into allowed region cells.
        /// </summary>
        /// <param name="start">Origin cell.</param>
        /// <param name="allowedRegion">Cells that the flood fill is allowed to enter.</param>
        /// <param name="steps">Number of grid steps to expand.</param>
        /// <param name="radius">Optional world-space radius. Overrides steps if > 0.</param>
        /// <param name="stopAtWalls">Whether to block propagation through walls.</param>
        /// <returns>A HashSet of all reached cells including the start cell.</returns>
        public HashSet<Vector2Int> FloodFillInRegion(
            Vector2Int start,
            List<Vector2Int> allowedRegion,
            int steps = 0,
            float radius = 0f,
            bool stopAtWalls = false)
        {
            HashSet<Vector2Int> visited = new HashSet<Vector2Int>();
            Queue<(Vector2Int cell, int depth)> frontier = new Queue<(Vector2Int, int)>();

            if (!allowedRegion.Contains(start))
                return visited; // start is not in the allowed region

            frontier.Enqueue((start, 0));
            visited.Add(start);

            float radiusSqr = radius > 0f ? radius * radius : float.MaxValue;
            Vector3 originWorld = CellToWorldSpace(start);

            while (frontier.Count > 0)
            {
                var (current, depth) = frontier.Dequeue();

                // Limit by steps
                if (steps > 0 && depth >= steps)
                    continue;

                Vector3 worldPos = CellToWorldSpace(current);
                if (radius > 0f && (worldPos - originWorld).sqrMagnitude > radiusSqr)
                    continue;

                // Expand to 4 neighbors
                foreach (Vector2Int offset in WorldGridUtilities.FourDirections)
                {
                    Vector2Int next = current + offset;

                    if (!IsValidCell(next) || visited.Contains(next))
                        continue;

                    if (!allowedRegion.Contains(next))
                        continue;

                    if (stopAtWalls && IsWallCell(next))
                        continue;

                    visited.Add(next);
                    frontier.Enqueue((next, depth + 1));
                }
            }

            return visited;
        }

        #endregion
    }
}