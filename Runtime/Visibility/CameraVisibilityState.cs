using System.Collections.Generic;
using UnityEngine;

namespace ShoelaceStudios.GridSystem.Visibility
{
	public class CameraVisibilityState
	{
		private HashSet<Vector2Int> current = new();
		private HashSet<Vector2Int> previous = new();
		public IReadOnlyCollection<Vector2Int> CurrentIndices => current;

		public void Swap()
		{
			(previous, current) = (current, previous);
			current.Clear();
		}

		public void Register(Vector2Int index) => current.Add(index);

		public bool IsVisibleNow(Vector2Int index) => current.Contains(index);
		public bool WasVisible(Vector2Int index) => previous.Contains(index);
		public bool BecameVisible(Vector2Int index) => IsVisibleNow(index) && !WasVisible(index);
		public bool BecameInvisible(Vector2Int index) => !IsVisibleNow(index) && WasVisible(index);
	}
}