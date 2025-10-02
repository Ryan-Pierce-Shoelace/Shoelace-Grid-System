using UnityEngine;

namespace Shoelace.GridSystem.Data
{
	public abstract class GameGrid : MonoBehaviour
	{
		[SerializeField] protected WorldGridManager gridManager;
		private int height;
		private int width;
		public abstract void Generate(int width, int height);

		public void SetHeight(int newHeight)
		{
			height = newHeight;
		}

		public void SetWidth(int newWidth)
		{
			width = newWidth;
		}

		protected bool IsWithinGridBounds(Vector3Int position)
		{
			return position is { x: >= 0, y: >= 0 } && position.x < height && position.y < width;
		}
	}
}