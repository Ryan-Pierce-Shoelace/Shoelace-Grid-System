using UnityEngine;

namespace ShoelaceStudios.GridSystem
{
    public readonly struct FloodFillParams
    {
        public readonly int MaxSteps;
        public readonly float MaxRadius;
        public readonly bool StopAtWalls;
        public readonly bool Diagonals;
        public readonly bool TrackDepth;
        public readonly bool[] RegionMask;
        public readonly IEdgeSystem EdgeSystem;

        public readonly Vector2Int BoundsMin;
        public readonly Vector2Int BoundsMax;
        public readonly bool HasBounds;

        public bool HasStepLimit => MaxSteps > 0;
        public bool HasRadiusLimit => MaxRadius > 0f;
        public bool HasRegionMask => RegionMask != null;
        public bool HasEdgeSystem => EdgeSystem != null;

        private FloodFillParams(int maxSteps, float maxRadius, bool stopAtWalls, bool diagonals, bool trackDepth,
            bool[] regionMask, IEdgeSystem edgeSystem, Vector2Int boundsMin, Vector2Int boundsMax, bool hasBounds)
        {
            MaxSteps = maxSteps;
            MaxRadius = maxRadius;
            StopAtWalls = stopAtWalls;
            Diagonals = diagonals;
            TrackDepth = trackDepth;
            RegionMask = regionMask;
            EdgeSystem = edgeSystem;
            BoundsMin = boundsMin;
            BoundsMax = boundsMax;
            HasBounds = hasBounds;
        }

        public static FloodFillParams Unlimited(bool stopAtWalls = true, bool diagonals = false, bool trackDepth = false, IEdgeSystem edgeSystem = null)
        {
            return new FloodFillParams(0, 0f, stopAtWalls, diagonals, trackDepth, null, edgeSystem, default, default, false);
        }

        public static FloodFillParams WithSteps(int steps, bool stopAtWalls = true, bool diagonals = false, bool trackDepth = false, IEdgeSystem edgeSystem = null)
        {
            return new FloodFillParams(steps, 0f, stopAtWalls, diagonals, trackDepth, null, edgeSystem, default, default, false);
        }

        public static FloodFillParams WithRadius(float radius, bool stopAtWalls = true, bool diagonals = false, bool trackDepth = false, IEdgeSystem edgeSystem = null)
        {
            return new FloodFillParams(0, radius, stopAtWalls, diagonals, trackDepth, null, edgeSystem, default, default, false);
        }

        public static FloodFillParams WithStepsAndRadius(int steps, float radius, bool stopAtWalls = true, bool diagonals = false, bool trackDepth = false, IEdgeSystem edgeSystem = null)
        {
            return new FloodFillParams(steps, radius, stopAtWalls, diagonals, trackDepth, null, edgeSystem, default, default, false);
        }

        public static FloodFillParams WithRegionMask(bool[] mask, bool stopAtWalls = true, bool diagonals = false, bool trackDepth = false, IEdgeSystem edgeSystem = null)
        {
            return new FloodFillParams(0, 0f, stopAtWalls, diagonals, trackDepth, mask, edgeSystem, default, default, false);
        }

        public static FloodFillParams WithinBounds(Vector2Int min, Vector2Int max, bool stopAtWalls = true, bool diagonals = false, bool trackDepth = false, IEdgeSystem edgeSystem = null)
        {
            return new FloodFillParams(0, 0f, stopAtWalls, diagonals, trackDepth, null, edgeSystem, min, max, true);
        }
    }
}
