using System.Collections.Generic;
using UnityEngine;

namespace ShoelaceStudios.GridSystem
{
    public class ChunkVisibilityTracker
    {
        private readonly Dictionary<Camera, CameraVisibilityState> visibilityStates = new();
        public void BeginFrame(Camera cam)
        {
            if (!visibilityStates.TryGetValue(cam, out CameraVisibilityState state))
                visibilityStates[cam] = state = new CameraVisibilityState();

            state.Swap(); // Prepare for this frame's visibility tracking
        }

        public void RegisterVisibleChunk(Camera cam, Vector2Int chunkOrigin)
        {
            if (!visibilityStates.TryGetValue(cam, out CameraVisibilityState state))
                return;

            state.Current.Add(chunkOrigin);
        }

        public bool IsVisibleNow(Camera cam, Vector2Int chunkOrigin)
            => visibilityStates.TryGetValue(cam, out CameraVisibilityState state) && state.IsVisibleNow(chunkOrigin);

        public bool WasVisible(Camera cam, Vector2Int chunkOrigin)
            => visibilityStates.TryGetValue(cam, out CameraVisibilityState state) && state.WasVisible(chunkOrigin);

        public bool BecameVisible(Camera cam, Vector2Int chunkOrigin)
            => visibilityStates.TryGetValue(cam, out CameraVisibilityState state) && state.BecameVisible(chunkOrigin);

        public bool BecameInvisible(Camera cam, Vector2Int chunkOrigin)
            => visibilityStates.TryGetValue(cam, out CameraVisibilityState state) && state.BecameInvisible(chunkOrigin);

        public bool CalculateVisibility(Camera cam, Bounds chunkBounds)
        {
            if (cam == null) return false;

            Plane[] frustumPlanes = GeometryUtility.CalculateFrustumPlanes(cam);
            return GeometryUtility.TestPlanesAABB(frustumPlanes, chunkBounds);
        }
    }
}
