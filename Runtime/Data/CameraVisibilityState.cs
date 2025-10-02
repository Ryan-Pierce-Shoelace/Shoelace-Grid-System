using System.Collections.Generic;
using UnityEngine;

namespace Shoelace.GridSystem.Data
{
    public class CameraVisibilityState
    {
        public HashSet<Vector2Int> Current = new();
        private HashSet<Vector2Int> previous = new();

        public void Swap()
        {
            (previous, Current) = (Current, previous);
            Current.Clear();
        }

        public bool IsVisibleNow(Vector2Int chunk) => Current.Contains(chunk);
        public bool WasVisible(Vector2Int chunk) => previous.Contains(chunk);
        public bool BecameVisible(Vector2Int chunk) => IsVisibleNow(chunk) && !WasVisible(chunk);
        public bool BecameInvisible(Vector2Int chunk) => !IsVisibleNow(chunk) && WasVisible(chunk);
    }
}
