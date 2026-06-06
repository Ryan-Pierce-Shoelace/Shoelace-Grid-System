using System.Collections.Generic;
using UnityEngine;

namespace ShoelaceStudios.GridSystem.Visibility
{
    public class ChunkVisibilityTracker
    {
        private readonly Dictionary<Camera, CameraVisibilityState> visibilityStates = new();

        public void BeginFrame(Camera cam)
        {
            if (!visibilityStates.TryGetValue(cam, out CameraVisibilityState state))
                visibilityStates[cam] = state = new CameraVisibilityState();

            state.Swap();
        }

        public bool IsVisibleNow(Camera cam, Vector2Int chunkOrigin)
        {
            return visibilityStates.TryGetValue(cam, out CameraVisibilityState state) && state.IsVisibleNow(chunkOrigin);
        }

        public bool WasVisible(Camera cam, Vector2Int chunkOrigin)
        {
            return visibilityStates.TryGetValue(cam, out CameraVisibilityState state) && state.WasVisible(chunkOrigin);
        }

        public bool BecameVisible(Camera cam, Vector2Int chunkOrigin)
        {
            return visibilityStates.TryGetValue(cam, out CameraVisibilityState state) && state.BecameVisible(chunkOrigin);
        }

        public bool BecameInvisible(Camera cam, Vector2Int chunkOrigin)
        {
            return visibilityStates.TryGetValue(cam, out CameraVisibilityState state) && state.BecameInvisible(chunkOrigin);
        }

        public void EvaluatePartition(Camera cam, WorldPartition partition)
        {
            if (cam == null || partition == null) return;
            if (!visibilityStates.TryGetValue(cam, out CameraVisibilityState state)) return;

            Plane[] frustum = GeometryUtility.CalculateFrustumPlanes(cam);

            foreach (WorldChunk chunk in partition.GetAllChunks())
                if (GeometryUtility.TestPlanesAABB(frustum, chunk.WorldBounds))
                    state.Register(chunk.Index);
        }


        #region Visibility Queries

        public IReadOnlyCollection<Vector2Int> GetVisibleIndices(Camera cam)
        {
            return visibilityStates.TryGetValue(cam, out CameraVisibilityState state)
                ? state.CurrentIndices
                : System.Array.Empty<Vector2Int>() as IReadOnlyCollection<Vector2Int>;
        }

        #endregion

        #region Camera Management

        public void TrackCamera(Camera cam)
        {
            if (cam != null && !visibilityStates.ContainsKey(cam))
                visibilityStates[cam] = new CameraVisibilityState();
        }

        public void UntrackCamera(Camera cam)
        {
            visibilityStates.Remove(cam);
        }

        public void Clear()
        {
            visibilityStates.Clear();
        }

        #endregion
    }
}
