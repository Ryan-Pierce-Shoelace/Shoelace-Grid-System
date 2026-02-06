using System.Collections.Generic;
using UnityEngine;

namespace ShoelaceStudios.GridSystem
{
	public class ChunkDataGrid<T> where T : IChunkData
	{
		#region Variables and Initialization

		private readonly int chunkSize;
		private readonly int width;
		private readonly int height;
		private readonly float cellSize;
		private readonly Vector3 worldOrigin;

		private readonly Dictionary<Vector2Int, GridChunk<T>> chunks;
		private readonly Dictionary<Vector2Int, ChunkRuntimeState> runtimeStates;

		public int Width => width;
		public int Height => height;
		public int ChunkSize => chunkSize;


		public ChunkDataGrid(int width, int height, int chunkSize, float cellSize, Vector3 worldOrigin)
		{
			this.width = width;
			this.height = height;
			this.chunkSize = chunkSize;
			this.cellSize = cellSize;
			this.worldOrigin = worldOrigin;

			chunks = new Dictionary<Vector2Int, GridChunk<T>>();
			runtimeStates = new Dictionary<Vector2Int, ChunkRuntimeState>();
		}

		#endregion

		#region Set & Get

		public T this[int x, int y]
		{
			get => GetCell(x, y);
			set => SetCell(x, y, value);
		}

		public T this[Vector2Int cell]
		{
			get => GetCell(cell.x, cell.y);
			set => SetCell(cell.x, cell.y, value);
		}

		public T GetCell(int x, int y)
		{
			if (!IsValidCell(x, y))
				return default;

			ChunkCoordinate coord = ChunkCoordinate.FromGlobal(x, y, chunkSize);

			if (!chunks.TryGetValue(coord.ChunkIndex, out GridChunk<T> chunk))
				return default;

			return chunk[coord.LocalCell.x, coord.LocalCell.y];
		}

		public void SetCell(int x, int y, T value)
		{
			if (!IsValidCell(x, y))
				return;

			ChunkCoordinate coord = ChunkCoordinate.FromGlobal(x, y, chunkSize);

			if (!chunks.TryGetValue(coord.ChunkIndex, out GridChunk<T> chunk))
			{
				chunk = new GridChunk<T>(coord.ChunkIndex, chunkSize, cellSize, worldOrigin);
				chunks[coord.ChunkIndex] = chunk;
			}

			chunk[coord.LocalCell.x, coord.LocalCell.y] = value;
		}

		public bool TryGetCell(int x, int y, out T value)
		{
			if (!IsValidCell(x, y))
			{
				value = default;
				return false;
			}

			ChunkCoordinate coord = ChunkCoordinate.FromGlobal(x, y, chunkSize);

			if (!chunks.TryGetValue(coord.ChunkIndex, out GridChunk<T> chunk))
			{
				value = default;
				return false;
			}

			value = chunk[coord.LocalCell.x, coord.LocalCell.y];
			return true;
		}

		#endregion

		#region Validation

		public bool IsValidCell(int x, int y)
		{
			return x >= 0 && x < width && y >= 0 && y < height;
		}

		public bool IsValidCell(Vector2Int cell)
		{
			return IsValidCell(cell.x, cell.y);
		}

		#endregion

		#region Chunk Queries

		public Vector2Int GetChunkIndex(int x, int y)
		{
			return new Vector2Int(
				Mathf.FloorToInt((float)x / chunkSize),
				Mathf.FloorToInt((float)y / chunkSize)
			);
		}

		public bool TryGetChunk(int x, int y, out GridChunk<T> chunk)
		{
			Vector2Int chunkIndex = GetChunkIndex(x, y);
			return chunks.TryGetValue(chunkIndex, out chunk);
		}

		public bool TryGetChunk(Vector2Int chunkIndex, out GridChunk<T> chunk)
		{
			return chunks.TryGetValue(chunkIndex, out chunk);
		}

		public IEnumerable<GridChunk<T>> GetAllChunks() => chunks.Values;

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
			if (runtimeStates.TryGetValue(chunkCoord, out ChunkRuntimeState state)) return state;

			state = new ChunkRuntimeState();
			runtimeStates[chunkCoord] = state;

			return state;
		}

		#endregion
	}
}