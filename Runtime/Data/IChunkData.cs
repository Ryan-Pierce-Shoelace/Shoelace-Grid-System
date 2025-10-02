using UnityEngine;

namespace Shoelace.GridSystem.Data
{
	public interface IChunkData
	{
		public Vector2Int ParentChunkCoord { get; }
		public string GetChunkDebug();
	}
}