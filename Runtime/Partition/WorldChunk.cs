using System;
using System.Collections.Generic;
using ShoelaceStudios.GridSystem.FloodFill;
using ShoelaceStudios.GridSystem.Utils;
using UnityEngine;

namespace ShoelaceStudios.GridSystem.Partition
{
	public class WorldChunk : AbstractChunk
	{
		private readonly IWorldGrid world;

		public Bounds WorldBounds { get; }
		public bool IsVisible { get; private set; }
		public bool WasVisibleLastFrame { get; private set; }

		public bool BecameVisible => IsVisible && !WasVisibleLastFrame;
		public bool BecameInvisible => !IsVisible && WasVisibleLastFrame;

		public WorldChunk(Vector2Int index, int chunkSize, IWorldGrid world)
			: base(index, chunkSize, world.Width, world.Height)
		{
			this.world = world;

			Vector3 bottomLeft = world.GetWorldFromCell(MinX, MinY);
			Vector3 topRight = world.GetWorldFromCell(MaxX, MaxY);

			WorldBounds = new Bounds(
				(bottomLeft + topRight) * 0.5f,
				new Vector3(
					(MaxX - MinX + 1) * world.CellSize,
					(MaxY - MinY + 1) * world.CellSize,
					1f));
		}

		public void UpdateFrame(bool visibleThisFrame)
		{
			WasVisibleLastFrame = IsVisible;
			IsVisible = visibleThisFrame;
		}

		public bool HasAny(Func<Vector2Int, bool> predicate)
		{
			for (int x = MinX; x <= MaxX; x++)
			for (int y = MinY; y <= MaxY; y++)
				if (predicate(new Vector2Int(x, y)))
					return true;

			return false;
		}

		public HashSet<Vector2Int> FloodFill(Vector2Int start, FloodFillParams parameters)
		{
			HashSet<Vector2Int> region = new();
			ForEachCell((x, y) =>
			{
				if (world.IsValidCell(x, y))
					region.Add(new Vector2Int(x, y));
			});
			return GridFloodFill.Execute(world, start, parameters, region);
		}
	}
}