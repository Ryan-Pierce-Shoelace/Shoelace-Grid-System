using System.Collections.Generic;
using UnityEngine;

namespace ShoelaceStudios.GridSystem
{
    public static class WorldGridUtilities
    {
        #region Polygon Overlapping Cells

        // Main method to get a list of grid cells that a given 2D collider overlaps.
        // overlapThreshold determines how much of the cell must be covered to count as overlapping.
        public static List<Vector2Int> GetOverlappingCells(this WorldGridManager grid, Collider2D collider,
            float overlapThreshold)
        {
            List<Vector2Int> overlappingCells = new List<Vector2Int>();

            // If no collider is provided, return an empty list
            if (!collider)
            {
                return overlappingCells;
            }

            // Convert the collider into an array of world-space points (corners or approximated shape)
            var points = GetColliderWorldPoints(collider);

            // If the collider points are not valid, return empty
            if (points == null || points.Length < 3)
                return overlappingCells;

            // Get the bounding box of the collider in world space
            Bounds bounds = collider.bounds;
            Vector3 min = bounds.min;
            Vector3 max = bounds.max;

            // Convert the bounding box into grid coordinates
            int minX = Mathf.FloorToInt(min.x / grid.CellSize);
            int maxX = Mathf.CeilToInt(max.x / grid.CellSize);
            int minY = Mathf.FloorToInt(min.y / grid.CellSize);
            int maxY = Mathf.CeilToInt(max.y / grid.CellSize);

            // Loop over all grid cells inside the bounding box
            for (int x = minX; x <= maxX; x++)
            {
                for (int y = minY; y <= maxY; y++)
                {
                    // Skip invalid cells (outside the grid)
                    if (!grid.IsValidCell(x, y))
                        continue;

                    // Get the square polygon representing this grid cell in world space
                    var cellPoly = GetCellPolygon(grid, x, y);

                    // Calculate what fraction of the cell is overlapped by the collider
                    float overlapRatio = ComputePolygonOverlapRatio(points, cellPoly);

                    // Only add the cell if the overlap is above the threshold
                    if (overlapRatio >= overlapThreshold)
                        overlappingCells.Add(new Vector2Int(x, y));
                }
            }

            if (overlappingCells.Count == 0)
            {
                //Find fallback cell (single cell that the object mostly covers
                Vector3 center = collider.bounds.center;
                Vector2Int fallbackCell = grid.GetCell(center);
                if (grid.IsValidCell(fallbackCell))
                {
                    overlappingCells.Add(fallbackCell);
                }
                else
                {
                    Debug.LogWarning("Could not find overlapping cell: " + fallbackCell);
                }
            }


            return overlappingCells;
        }

        // Converts a collider into an array of points in world space
        private static Vector2[] GetColliderWorldPoints(Collider2D collider)
        {
            switch (collider)
            {
                case PolygonCollider2D polygon:
                    // For polygon colliders, transform each point to world space
                    var points = polygon.points;
                    var worldPoints = new Vector2[points.Length];
                    for (int i = 0; i < points.Length; i++)
                    {
                        worldPoints[i] = collider.transform.TransformPoint(points[i]);
                    }

                    return worldPoints;

                case BoxCollider2D box:
                    // For box colliders, get the 4 corners
                    Vector2 size = box.size * .5f; // half-size for offsets
                    Vector2[] corners =
                    {
                        new Vector2(-size.x, -size.y),
                        new Vector2(-size.x, size.y),
                        new Vector2(size.x, size.y),
                        new Vector2(size.x, -size.y),
                    };

                    // Transform corners to world space
                    for (var i = 0; i < corners.Length; i++)
                    {
                        corners[i] = collider.transform.TransformPoint(corners[i]);
                    }

                    return corners;

                case CapsuleCollider2D capsule:
                    // Approximate a capsule as a series of points
                    return ApproximateCapsule(capsule, 8);

                case CircleCollider2D circle:
                    // Approximate a circle as a series of points
                    return ApproximateCircle(circle, 12);

                default:
                    // Collider type not supported
                    Debug.LogWarning($"Unsupported collider type: {collider.GetType().Name}");
                    return null;
            }
        }

        // Returns the 4 corners of a grid cell in world space
        private static Vector2[] GetCellPolygon(WorldGridManager grid, int x, int y)
        {
            float s = grid.CellSize * .5f; // half the size to calculate corners
            Vector3 center = grid.CellToWorldSpace(x, y);
            return new[]
            {
                (Vector2)(center + new Vector3(-s, -s)), // bottom-left
                (Vector2)(center + new Vector3(s, -s)), // bottom-right
                (Vector2)(center + new Vector3(s, s)), // top-right
                (Vector2)(center + new Vector3(-s, s)) // top-left
            };
        }

        // Approximate a circle as a series of points around the edge
        private static Vector2[] ApproximateCircle(CircleCollider2D circle, int segments)
        {
            var points = new Vector2[segments];
            float r = circle.radius;
            for (int i = 0; i < segments; i++)
            {
                float angle = i * Mathf.PI * 2 / segments;
                Vector2 local = new Vector2(Mathf.Cos(angle) * r, Mathf.Sin(angle) * r);
                points[i] = circle.transform.TransformPoint(local + circle.offset);
            }

            return points;
        }

        // Approximate a capsule as a series of points around its edges
        private static Vector2[] ApproximateCapsule(CapsuleCollider2D capsule, int segments)
        {
            var points = new List<Vector2>();
            float r = capsule.size.x * 0.5f; // radius of the capsule ends
            float h = capsule.size.y - 2 * r; // length of the center rectangle

            // Top half-circle
            for (int i = 0; i < segments; i++)
            {
                float angle = Mathf.PI * i / (segments - 1);
                Vector2 local = new Vector2(Mathf.Cos(angle) * r, Mathf.Sin(angle) * r + h * 0.5f);
                points.Add(capsule.transform.TransformPoint(local + capsule.offset));
            }

            // Bottom half-circle
            for (int i = 0; i < segments; i++)
            {
                float angle = Mathf.PI * i / (segments - 1);
                Vector2 local = new Vector2(-Mathf.Cos(angle) * r, -Mathf.Sin(angle) * r - h * 0.5f);
                points.Add(capsule.transform.TransformPoint(local + capsule.offset));
            }

            return points.ToArray();
        }

        // Compute how much of a cell polygon is covered by the collider polygon
        private static float ComputePolygonOverlapRatio(Vector2[] colliderPoly, Vector2[] cellPoly,
            int sampleResolution = 3)
        {
            int insideCount = 0;
            int totalSamples = sampleResolution * sampleResolution;

            // Get bottom-left and top-right corners of the cell
            Vector2 min = cellPoly[0];
            Vector2 max = cellPoly[2];

            // Distance between each sample point
            float dx = (max.x - min.x) / (sampleResolution - 1);
            float dy = (max.y - min.y) / (sampleResolution - 1);

            // Sample points in a grid across the cell
            for (int ix = 0; ix < sampleResolution; ix++)
            {
                for (int iy = 0; iy < sampleResolution; iy++)
                {
                    Vector2 sample = new Vector2(min.x + ix * dx, min.y + iy * dy);
                    // If the point is inside the collider polygon, count it
                    if (IsPointInPolygon(sample, colliderPoly))
                        insideCount++;
                }
            }

            // Return the fraction of points inside
            return (float)insideCount / totalSamples;
        }

        // Determine if a 2D point is inside a polygon
        private static bool IsPointInPolygon(Vector2 point, Vector2[] poly)
        {
            bool inside = false;
            // Loop through each edge of the polygon
            for (int i = 0, j = poly.Length - 1; i < poly.Length; j = i++)
            {
                // Check if the horizontal line from the point crosses this edge
                if (((poly[i].y > point.y) != (poly[j].y > point.y)) &&
                    (point.x < (poly[j].x - poly[i].x) * (point.y - poly[i].y) / (poly[j].y - poly[i].y) + poly[i].x))
                    inside = !inside; // toggle the inside flag
            }

            return inside; // true if inside, false if outside
        }

        #endregion
    }
}