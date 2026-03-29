using System;
using System.Collections.Generic;
using UnityEngine;

namespace ShoelaceStudios.GridSystem.Utils
{
	public static class GridColliderUtils
	{
		#region Constants

		private const int MIN_POLYGON_POINTS = 3;
		private const int DEFAULT_OVERLAP_SAMPLE_RESOLUTION = 3;
		private const int CIRCLE_APPROXIMATION_SEGMENTS = 12;
		private const int CAPSULE_APPROXIMATION_SEGMENTS = 8;
		private const int CELL_CORNER_COUNT = 4;

		#endregion

		#region Collider Points

		public static bool TryGetColliderPoints(Collider2D collider, out Vector2[] points)
		{
			if (!IsValidCollider(collider))
			{
				points = null;
				return false;
			}

			points = GetColliderWorldPoints(collider);
			return IsValidPolygon(points);
		}

		private static Vector2[] GetColliderWorldPoints(Collider2D collider)
		{
			if (collider == null) return null;

			Type type = collider.GetType();

			if (type == typeof(PolygonCollider2D))
				return GetPolygonPoints((PolygonCollider2D)collider);

			if (type == typeof(BoxCollider2D))
				return GetBoxPoints((BoxCollider2D)collider);

			if (type == typeof(CapsuleCollider2D))
				return ApproximateCapsule((CapsuleCollider2D)collider, CAPSULE_APPROXIMATION_SEGMENTS);

			if (type == typeof(CircleCollider2D))
				return ApproximateCircle((CircleCollider2D)collider, CIRCLE_APPROXIMATION_SEGMENTS);

			return LogUnsupportedColliderType(collider);
		}

		private static Vector2[] GetPolygonPoints(PolygonCollider2D polygon)
		{
			Vector2[] localPoints = polygon.points;
			Vector2[] worldPoints = new Vector2[localPoints.Length];

			for (int i = 0; i < localPoints.Length; i++) worldPoints[i] = polygon.transform.TransformPoint(localPoints[i]);

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
				new(halfSize.x, -halfSize.y) // Bottom-right
			};

			Vector2[] worldCorners = new Vector2[CELL_CORNER_COUNT];

			for (int i = 0; i < CELL_CORNER_COUNT; i++) worldCorners[i] = box.transform.TransformPoint(localCorners[i] + box.offset);

			return worldCorners;
		}

		private static Vector2[] ApproximateCircle(CircleCollider2D circle, int segments)
		{
			Vector2[] points = new Vector2[segments];
			float angleIncrement = Mathf.PI * 2f / segments;

			for (int i = 0; i < segments; i++)
			{
				float angle = i * angleIncrement;
				Vector2 local = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * circle.radius;
				points[i] = circle.transform.TransformPoint(local + circle.offset);
			}

			return points;
		}

		private static Vector2[] ApproximateCapsule(CapsuleCollider2D capsule, int segmentsPerHalf)
		{
			Vector2 size = capsule.size;
			float radius = size.x * 0.5f;
			float centerHeight = size.y - 2f * radius;
			float topCircleY = centerHeight * 0.5f;
			float bottomCircleY = -centerHeight * 0.5f;

			List<Vector2> points = new(segmentsPerHalf * 2);

			AddSemicircle(points, capsule, radius, topCircleY, segmentsPerHalf, true);
			AddSemicircle(points, capsule, radius, bottomCircleY, segmentsPerHalf, false);

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

		#endregion

		#region Geometry

		public static Vector2[] GetCellCorners(Vector3 cellCenter, float cellSize)
		{
			float half = cellSize * 0.5f;
			return new Vector2[CELL_CORNER_COUNT]
			{
				new(cellCenter.x - half, cellCenter.y - half), //Bot left
				new(cellCenter.x + half, cellCenter.y - half), //Bot right
				new(cellCenter.x + half, cellCenter.y + half), //Top right
				new(cellCenter.x - half, cellCenter.y + half) // top left
			};
		}

		public static float ComputePolygonOverlapRatio(
			Vector2[] colliderPolygon,
			Vector2[] cellPolygon,
			int samplesPerAxis = DEFAULT_OVERLAP_SAMPLE_RESOLUTION)
		{
			int samplesInside = CountSamplesInsidePolygon(colliderPolygon, cellPolygon, samplesPerAxis);
			int totalSamples = samplesPerAxis * samplesPerAxis;
			return (float)samplesInside / totalSamples;
		}

		#endregion


		#region Polygon Math

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
			for (int iy = 0; iy < samplesPerAxis; iy++)
			{
				Vector2 samplePoint = new(
					cellMin.x + ix * sampleStepX,
					cellMin.y + iy * sampleStepY
				);

				if (IsPointInPolygon(samplePoint, colliderPolygon)) insideCount++;
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
				int j = i == 0 ? vertexCount - 1 : i - 1;

				Vector2 currentVertex = polygon[i];
				Vector2 previousVertex = polygon[j];

				if (DoesEdgeCrossLine(point, currentVertex, previousVertex)) isInside = !isInside;
			}

			return isInside;
		}

		private static bool DoesEdgeCrossLine(Vector2 point, Vector2 v1, Vector2 v2)
		{
			bool verticesStraddleHorizontal = v1.y > point.y != v2.y > point.y;

			if (!verticesStraddleHorizontal)
				return false;

			float intersectionX = v2.x + (point.y - v2.y) * (v1.x - v2.x) / (v1.y - v2.y);
			return point.x < intersectionX;
		}

		#endregion


		#region private Validation

		private static bool IsValidCollider(Collider2D collider)
		{
			return collider != null;
		}

		private static bool IsValidPolygon(Vector2[] points)
		{
			return points is { Length: >= MIN_POLYGON_POINTS };
		}

		private static Vector2[] LogUnsupportedColliderType(Collider2D collider)
		{
			Debug.LogWarning($"[GridColliderUtils] Unsupported collider type: {collider.GetType().Name}");
			return null;
		}

		#endregion
	}
}