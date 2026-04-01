using System;
using System.Collections.Generic;
using ShoelaceStudios.GridSystem.Visibility;
using UnityEngine;

namespace ShoelaceStudios.GridSystem
{
	public class WorldPartition : AbstractPartition<WorldChunk>
	{
		public WorldPartition(IWorldGrid world, int chunkSize)
			: base(world.Width, world.Height, chunkSize)
		{
			for (int cx = 0; cx < ChunksX; cx++)
			for (int cy = 0; cy < ChunksY; cy++)
			{
				Vector2Int index = new(cx, cy);
				chunks[index] = new WorldChunk(index, chunkSize, world);
			}
		}

		#region Visibility

		public void UpdateVisibility(ChunkVisibilityTracker tracker, Camera cam, Action<WorldChunk> onBecameVisible = null, Action<WorldChunk> onBecameInvisible = null)
		{
			foreach (WorldChunk chunk in chunks.Values)
			{
				chunk.UpdateFrame(tracker.IsVisibleNow(cam, chunk.Index));

				if (chunk.BecameVisible) onBecameVisible?.Invoke(chunk);
				if (chunk.BecameInvisible) onBecameInvisible?.Invoke(chunk);
			}
		}

		#endregion

		public void ForEachVisibleChunk(Camera cam, Action<WorldChunk> action)
		{
			if (cam == null) return;

			Plane[] frustum = GeometryUtility.CalculateFrustumPlanes(cam);
			foreach (WorldChunk chunk in chunks.Values)
				if (GeometryUtility.TestPlanesAABB(frustum, chunk.WorldBounds))
					action(chunk);
		}

		public HashSet<Vector2Int> GetVisibleChunkIndices(Camera cam)
		{
			HashSet<Vector2Int> visible = new();
			if (cam == null) return visible;

			Plane[] frustum = GeometryUtility.CalculateFrustumPlanes(cam);
			foreach (WorldChunk chunk in chunks.Values)
				if (GeometryUtility.TestPlanesAABB(frustum, chunk.WorldBounds))
					visible.Add(chunk.Index);
			return visible;
		}
	}
}