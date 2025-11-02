using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ShoelaceStudios.GridSystem
{
	public static class WorldGridUtilities
	{
		#region Consts

		private const int MIN_POLYGON_POINTS = 3;
		private const int DEFAULT_OVERLAP_SAMPLE_RESOLUTION = 3;
		private const int CIRCLE_APPROXIMATION_SEGMENTS = 12;
		private const int CAPSULE_APPROXIMATION_SEGMENTS = 8;
		private const int CELL_CORNER_COUNT = 4;

		public static readonly Vector2Int[] FourDirections = new[]
		{
			new Vector2Int(1, 0), // East
			new Vector2Int(-1, 0), // West
			new Vector2Int(0, 1), // North
			new Vector2Int(0, -1) // South
		};

		public static readonly Vector2Int[] EightDirections = new[]
		{
			new Vector2Int(1, 0), // East
			new Vector2Int(-1, 0), // West
			new Vector2Int(0, 1), // North
			new Vector2Int(0, -1), // South
			new Vector2Int(1, 1), // NE
			new Vector2Int(-1, 1), // NW
			new Vector2Int(1, -1), // SE
			new Vector2Int(-1, -1), // SW
		};

		#endregion


		#region Public API - Spatial Queries

		/// <summary>
		/// Check if a cell is within circular radius of an origin using Euclidean distance
		/// </summary>
		public static bool IsWithinCircularRadius(Vector2Int origin, Vector2Int candidate, int radius)
		{
			int dx = candidate.x - origin.x;
			int dy = candidate.y - origin.y;
			return dx * dx + dy * dy <= radius * radius;
		}
        

		/// <summary>
		/// Check if a cell is within square/Manhattan radius of an origin
		/// </summary>
		public static bool IsWithinSquareRadius(Vector2Int origin, Vector2Int candidate, int radius)
		{
			int dx = Mathf.Abs(candidate.x - origin.x);
			int dy = Mathf.Abs(candidate.y - origin.y);
			return dx <= radius && dy <= radius;
		}

		/// <summary>
		/// Check if a cell is within Manhattan distance of an origin
		/// </summary>
		public static bool IsWithinManhattanDistance(Vector2Int origin, Vector2Int candidate, int distance)
		{
			int dx = Mathf.Abs(candidate.x - origin.x);
			int dy = Mathf.Abs(candidate.y - origin.y);
			return dx + dy <= distance;
		}

		/// <summary>
		/// Check if the path from origin to candidate is blocked by walls using line-of-sight
		/// </summary>
		public static bool HasLineOfSight(Vector2Int from, Vector2Int to, System.Func<Vector2Int, bool> isWallCell)
		{
			return GetCellsOnLine(from, to).All(cellOnLine => !isWallCell(cellOnLine));
		}

		public static bool IsBlockedByWalls(Vector2Int origin, Vector2Int target, System.Func<Vector2Int, bool> isWallCell)
		{
			return !HasLineOfSight(origin, target, isWallCell);
		}


		/// <summary>
		/// Return all cells in a square area around an origin.
		/// Use with IsWithinRadius() to filter to circular area.
		/// </summary>
		public static IEnumerable<Vector2Int> GetCellsInSquareArea(Vector2Int origin, int radius)
		{
			for (int dx = -radius; dx <= radius; dx++)
			{
				for (int dy = -radius; dy <= radius; dy++)
				{
					yield return new Vector2Int(origin.x + dx, origin.y + dy);
				}
			}
		}

		/// <summary>
		/// Bresenham's line algorithm: returns all cells between start and end inclusive
		/// </summary>
		public static IEnumerable<Vector2Int> GetCellsOnLine(Vector2Int start, Vector2Int end)
		{
			int x = start.x;
			int y = start.y;
			int endX = end.x;
			int endY = end.y;

			int dx = Mathf.Abs(endX - x);
			int dy = Mathf.Abs(endY - y);
			int stepX = x < endX ? 1 : -1;
			int stepY = y < endY ? 1 : -1;
			int error = dx - dy;

			while (true)
			{
				yield return new Vector2Int(x, y);

				if (x == endX && y == endY)
					break;

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

		#endregion

		#region Public API - Collider Overlap

		/// <summary>
		/// Get all grid cells that a 2D collider overlaps.
		/// </summary>
		public static List<Vector2Int> GetOverlappingCells(this WorldGridManager grid, Collider2D collider, float overlapThreshold)
		{
			if (!TryGetColliderPoints(collider, out Vector2[] colliderPoints))
				return new List<Vector2Int>();

			GridBounds searchBounds = GridBounds.FromWorldBounds(collider.bounds, grid.CellSize);

			return FindOverlappingCellsInBounds(grid, searchBounds, colliderPoints, overlapThreshold);
		}

		#endregion

		#region Private Validation

		private static bool IsValidCollider(Collider2D collider) => collider != null;

		private static bool IsValidPolygon(Vector2[] points) => points is { Length: >= MIN_POLYGON_POINTS };

		private static bool TryGetColliderPoints(Collider2D collider, out Vector2[] points)
		{
			if (!IsValidCollider(collider))
			{
				points = null;
				return false;
			}

			points = GetColliderWorldPoints(collider);
			return IsValidPolygon(points);
		}

		#endregion

		#region Private Helpers - Overlap Detection

		private static List<Vector2Int> FindOverlappingCellsInBounds(WorldGridManager grid, GridBounds bounds, Vector2[] colliderPoints, float overlapThreshold)
		{
			List<Vector2Int> overlappingCells = new();

			for (int x = bounds.MinX; x <= bounds.MaxX; x++)
			{
				for (int y = bounds.MinY; y <= bounds.MaxY; y++)
				{
					if (!grid.IsValidCell(x, y))
						continue;

					if (DoesCellOverlapCollider(grid, x, y, colliderPoints, overlapThreshold))
					{
						overlappingCells.Add(new Vector2Int(x, y));
					}
				}
			}

			return overlappingCells;
		}

		private static bool DoesCellOverlapCollider(
			WorldGridManager grid,
			int x,
			int y,
			Vector2[] colliderPoints,
			float overlapThreshold)
		{
			Vector2[] cellCorners = GetCellPolygon(grid, x, y);
			float overlapRatio = ComputePolygonOverlapRatio(colliderPoints, cellCorners);
			return overlapRatio >= overlapThreshold;
		}

		private static Vector2[] GetCellPolygon(WorldGridManager grid, int x, int y)
		{
			float halfCellSize = grid.CellSize * 0.5f;
			Vector3 cellCenter = grid.CellToWorldSpace(x, y);

			return new Vector2[CELL_CORNER_COUNT]
			{
				cellCenter + new Vector3(-halfCellSize, -halfCellSize), // Bottom-left
				cellCenter + new Vector3(halfCellSize, -halfCellSize), // Bottom-right
				cellCenter + new Vector3(halfCellSize, halfCellSize), // Top-right
				cellCenter + new Vector3(-halfCellSize, halfCellSize) // Top-left
			};
		}

		#endregion

		#region Private Helpers - Collider Conversion

		private static Vector2[] GetColliderWorldPoints(Collider2D collider)
		{
			return collider switch
			{
				PolygonCollider2D polygon => GetPolygonPoints(polygon),
				BoxCollider2D box => GetBoxPoints(box),
				CapsuleCollider2D capsule => ApproximateCapsule(capsule, CAPSULE_APPROXIMATION_SEGMENTS),
				CircleCollider2D circle => ApproximateCircle(circle, CIRCLE_APPROXIMATION_SEGMENTS),
				_ => LogUnsupportedColliderType(collider)
			};
		}

		private static Vector2[] GetPolygonPoints(PolygonCollider2D polygon)
		{
			Vector2[] localPoints = polygon.points;
			Vector2[] worldPoints = new Vector2[localPoints.Length];

			for (int i = 0; i < localPoints.Length; i++)
			{
				worldPoints[i] = polygon.transform.TransformPoint(localPoints[i]);
			}

			return worldPoints;
		}

		private static Vector2[] GetBoxPoints(BoxCollider2D box)
		{
			Vector2 halfSize = box.size * 0.5f;

			Vector2[] localCorners = new Vector2[CELL_CORNER_COUNT]
			{
				new(-halfSize.x, -halfSize.y), // Bottom-left
				new(-halfSize.x, halfSize.y), // Top-left
				new(halfSize.x, halfSize.y), // Top-right
				new(halfSize.x, -halfSize.y), // Bottom-right
			};

			Vector2[] worldCorners = new Vector2[CELL_CORNER_COUNT];
			for (int i = 0; i < CELL_CORNER_COUNT; i++)
			{
				worldCorners[i] = box.transform.TransformPoint(localCorners[i] + box.offset);
			}

			return worldCorners;
		}

		private static Vector2[] ApproximateCircle(CircleCollider2D circle, int segments)
		{
			Vector2[] points = new Vector2[segments];
			float radius = circle.radius;
			float angleIncrement = (Mathf.PI * 2f) / segments;

			for (int i = 0; i < segments; i++)
			{
				float angle = i * angleIncrement;
				Vector2 localPoint = new(
					Mathf.Cos(angle) * radius,
					Mathf.Sin(angle) * radius
				);

				points[i] = circle.transform.TransformPoint(localPoint + circle.offset);
			}

			return points;
		}

		private static Vector2[] ApproximateCapsule(CapsuleCollider2D capsule, int segmentsPerHalf)
		{
			float radius = capsule.size.x * 0.5f;
			float centerHeight = capsule.size.y - (2f * radius);
			float topCircleY = centerHeight * 0.5f;
			float bottomCircleY = -centerHeight * 0.5f;

			List<Vector2> points = new(segmentsPerHalf * 2);

			AddSemicircle(points, capsule, radius, topCircleY, segmentsPerHalf, isTop: true);
			AddSemicircle(points, capsule, radius, bottomCircleY, segmentsPerHalf, isTop: false);

			return points.ToArray();
		}

		private static void AddSemicircle(
			List<Vector2> points,
			CapsuleCollider2D capsule,
			float radius,
			float yOffset,
			int segments,
			bool isTop)
		{
			float angleIncrement = Mathf.PI / (segments - 1);

			for (int i = 0; i < segments; i++)
			{
				float angle = i * angleIncrement;
				float x = Mathf.Cos(angle) * radius;
				float y = Mathf.Sin(angle) * radius;

				Vector2 localPoint = isTop ? new Vector2(x, y + yOffset) : new Vector2(-x, -y + yOffset);

				points.Add(capsule.transform.TransformPoint(localPoint + capsule.offset));
			}
		}

		private static Vector2[] LogUnsupportedColliderType(Collider2D collider)
		{
			Debug.LogWarning($"Unsupported collider type: {collider.GetType().Name}. Supported types: PolygonCollider2D, BoxCollider2D, CircleCollider2D, CapsuleCollider2D");
			return null;
		}

		#endregion

		#region Private Helpers - Polygon Math

		private static float ComputePolygonOverlapRatio(
			Vector2[] colliderPolygon,
			Vector2[] cellPolygon,
			int samplesPerAxis = DEFAULT_OVERLAP_SAMPLE_RESOLUTION)
		{
			int samplesInside = CountSamplesInsidePolygon(colliderPolygon, cellPolygon, samplesPerAxis);
			int totalSamples = samplesPerAxis * samplesPerAxis;
			return (float)samplesInside / totalSamples;
		}

		private static int CountSamplesInsidePolygon(
			Vector2[] colliderPolygon,
			Vector2[] cellPolygon,
			int samplesPerAxis)
		{
			Vector2 cellMin = GetMinCorner(cellPolygon);
			Vector2 cellMax = GetMaxCorner(cellPolygon);

			float sampleStepX = (cellMax.x - cellMin.x) / (samplesPerAxis - 1);
			float sampleStepY = (cellMax.y - cellMin.y) / (samplesPerAxis - 1);

			int insideCount = 0;

			for (int ix = 0; ix < samplesPerAxis; ix++)
			{
				for (int iy = 0; iy < samplesPerAxis; iy++)
				{
					Vector2 samplePoint = new(
						cellMin.x + (ix * sampleStepX),
						cellMin.y + (iy * sampleStepY)
					);

					if (IsPointInPolygon(samplePoint, colliderPolygon))
					{
						insideCount++;
					}
				}
			}

			return insideCount;
		}

		private static Vector2 GetMinCorner(Vector2[] polygon)
		{
			float minX = float.MaxValue;
			float minY = float.MaxValue;

			foreach (Vector2 point in polygon)
			{
				if (point.x < minX) minX = point.x;
				if (point.y < minY) minY = point.y;
			}

			return new Vector2(minX, minY);
		}

		private static Vector2 GetMaxCorner(Vector2[] polygon)
		{
			float maxX = float.MinValue;
			float maxY = float.MinValue;

			foreach (Vector2 point in polygon)
			{
				if (point.x > maxX) maxX = point.x;
				if (point.y > maxY) maxY = point.y;
			}

			return new Vector2(maxX, maxY);
		}

		private static bool IsPointInPolygon(Vector2 point, Vector2[] polygon)
		{
			bool isInside = false;
			int vertexCount = polygon.Length;

			for (int i = 0; i < vertexCount; i++)
			{
				int j = (i == 0) ? vertexCount - 1 : i - 1;

				Vector2 currentVertex = polygon[i];
				Vector2 previousVertex = polygon[j];

				if (DoesEdgeCrossLine(point, currentVertex, previousVertex))
				{
					isInside = !isInside;
				}
			}

			return isInside;
		}

		private static bool DoesEdgeCrossLine(Vector2 point, Vector2 v1, Vector2 v2)
		{
			bool verticesStraddleHorizontal = (v1.y > point.y) != (v2.y > point.y);

			if (!verticesStraddleHorizontal)
				return false;

			float intersectionX = v2.x + (point.y - v2.y) * (v1.x - v2.x) / (v1.y - v2.y);
			return point.x < intersectionX;
		}

		#endregion
	}
}