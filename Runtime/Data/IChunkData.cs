using UnityEngine;

namespace ShoelaceStudios.GridSystem
{
	public interface IChunkData
	{
		public Vector2Int ParentChunkCoord { get; }
		public string GetChunkDebug();
	}
}