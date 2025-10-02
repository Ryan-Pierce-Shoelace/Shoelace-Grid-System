using System.Collections.Generic;
using UnityEngine;

namespace Shoelace.GridSystem.Data
{
	public class ChunkDataGrid<T> where T : IChunkData
	{
		#region Variables and Initialization

		private readonly int chunkSize;
		private readonly int width;
		private readonly int height;
		private readonly Dictionary<Vector2Int, GridChunk<T>> chunks;
		private readonly Dictionary<Vector2Int, ChunkRuntimeState> runtimeStates;

		readonly float cellSize;
		readonly Vector3 worldOrigin;

		public ChunkDataGrid(int width, int height, int chunkSize, float cellSize, Vector3 worldOrigin)
		{
			this.width = width;
			this.height = height;
			this.chunkSize = chunkSize;

			this.cellSize = cellSize;
			this.worldOrigin = worldOrigin;

			chunks = new Dictionary<Vector2Int, GridChunk<T>>();
			runtimeStates = new Dictionary<Vector2Int, ChunkRuntimeState>();

			int chunksX = Mathf.CeilToInt(width / chunkSize);
			int chunksY = Mathf.CeilToInt(height / chunkSize);

			for (int x = 0; x < chunksX; x++)
			{
				for (int y = 0; y < chunksY; y++)
				{
					Vector2Int coord = new(x, y);
					chunks[coord] = new GridChunk<T>(coord, chunkSize, cellSize, worldOrigin);
				}
			}
		}

		#endregion

		#region Set & Get

		public T this[int x, int y]
		{
			get => GetValue(x, y);
			set => SetValue(x, y, value);
		}

		private T GetValue(int x, int y)
		{
			if (!IsValid(x, y))
			{
				//Debug.Log("Cell Not Valid : " + X + ", " + Y);
				return default;
			}

			Vector2Int chunkCoord = GetChunkCoord(x, y);
			if (chunks.TryGetValue(chunkCoord, out GridChunk<T> chunk) && chunk.TryGetLocal(x, y, out int localX, out int localY))
			{
				return chunk[localX, localY];
			}

			return default;
		}

		private void SetValue(int x, int y, T value)
		{
			if (!IsValid(x, y))
			{
				//Debug.Log("Cell Not Valid : " +  X + ", " + Y);
				return;
			}

			Vector2Int chunkCoord = GetChunkCoord(x, y);
			if (!chunks.TryGetValue(chunkCoord, out GridChunk<T> chunk))
			{
				chunk = new GridChunk<T>(chunkCoord, chunkSize, cellSize, worldOrigin);
				chunks[chunkCoord] = chunk;
			}

			if (chunk.TryGetLocal(x, y, out int localX, out int localY))
			{
				chunk[localX, localY] = value;
			}
			else
			{
				Debug.Log("Chunk invalid");
			}
		}

		#endregion

		#region Helpers

		public Vector2Int GetChunkCoord(int x, int y)
		{
			return new Vector2Int(Mathf.FloorToInt((float)x / chunkSize), Mathf.FloorToInt((float)y / chunkSize));
		}

		public Vector2Int GetLocalCellCoord(Vector2Int chunkCoord, int x, int y)
		{
			return new Vector2Int(x - (chunkCoord.x * chunkSize), y - (chunkCoord.y * chunkSize));
		}

		private bool IsValid(int x, int y)
		{
			return x >= 0 && x < width && y >= 0 && y < height;
		}

		public IEnumerable<GridChunk<T>> GetAllChunks() => chunks.Values;

		public bool TryGetChunk(int x, int y, out GridChunk<T> chunk)
		{
			Vector2Int chunkCoord = GetChunkCoord(x, y);
			return chunks.TryGetValue(chunkCoord, out chunk);
		}

		#endregion

		#region Chunk Visibility Time Management

		public void BeginFrame()
		{
			foreach (KeyValuePair<Vector2Int, ChunkRuntimeState> kvp in runtimeStates)
				kvp.Value.BeginFrame();
		}

		public void EndFrame()
		{
			foreach (KeyValuePair<Vector2Int, ChunkRuntimeState> kvp in runtimeStates)
				kvp.Value.EndFrame();
		}

		public void MarkVisible(Vector2Int chunkCoord)
		{
			GetRuntimeState(chunkCoord).IsVisibleThisFrame = true;
		}

		public ChunkRuntimeState GetRuntimeState(Vector2Int chunkCoord)
		{
			if (!runtimeStates.TryGetValue(chunkCoord, out ChunkRuntimeState state))
			{
				state = new ChunkRuntimeState();
				runtimeStates[chunkCoord] = state;
			}

			return state;
		}

		#endregion
	}
}