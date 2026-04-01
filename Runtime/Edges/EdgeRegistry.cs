using System;
using UnityEngine;

namespace ShoelaceStudios.GridSystem.Edges
{
	public class EdgeRegistry
	{
		private readonly byte[] southEdges;
		private readonly byte[] westEdges;
		private readonly byte[] corners;

		private readonly int width;
		private readonly int height;

		public int Width => width;

		public int Height => height;

		public EdgeRegistry(IWorldGrid grid) : this(grid.Width, grid.Height)
		{
		}

		public EdgeRegistry(int width, int height)
		{
			this.width = width;
			this.height = height;

			southEdges = new byte[width * (height + 1)];
			westEdges = new byte[(width + 1) * height];
			corners = new byte[(width + 1) * (height + 1)];
		}

		private int SouthIndex(int x, int y)
		{
			return y * width + x;
		}

		private int WestIndex(int x, int y)
		{
			return y * (width + 1) + x;
		}

		private int CornerIndex(int x, int y)
		{
			return y * (width + 1) + x;
		}

		private static EdgeDirection GetDirectionBetween(int fromX, int fromY, int toX, int toY)
		{
			int dx = toX - fromX;
			int dy = toY - fromY;

			if (dy == 1) return EdgeDirection.North;
			if (dy == -1) return EdgeDirection.South;
			if (dx == 1) return EdgeDirection.East;

			return EdgeDirection.West;
		}

		private int GetEdgeIndexFromDirection(int x, int y, EdgeDirection direction, out bool isHorizontal)
		{
			switch (direction)
			{
				case EdgeDirection.South:
					isHorizontal = true;
					return SouthIndex(x, y);
				case EdgeDirection.North:
					isHorizontal = true;
					return SouthIndex(x, y + 1);
				case EdgeDirection.West:
					isHorizontal = false;
					return WestIndex(x, y);
				case EdgeDirection.East:
					isHorizontal = false;
					return WestIndex(x + 1, y);
				default:
					isHorizontal = false;
					return -1;
			}
		}

		public int GetCornerIndex(int x, int y, CellCorner corner)
		{
			switch (corner)
			{
				case CellCorner.BottomLeft: return CornerIndex(x, y);
				case CellCorner.BottomRight: return CornerIndex(x + 1, y);
				case CellCorner.TopLeft: return CornerIndex(x, y + 1);
				case CellCorner.TopRight: return CornerIndex(x + 1, y + 1);
				default: return -1;
			}
		}

		public int GetCornerIndex(Vector2Int cell, CellCorner corner)
		{
			return GetCornerIndex(cell.x, cell.y, corner);
		}

		#region Edges

		public byte GetEdge(int x, int y, EdgeDirection direction)
		{
			int index = GetEdgeIndexFromDirection(x, y, direction, out bool isHorizontal);
			return isHorizontal ? southEdges[index] : westEdges[index];
		}

		public byte GetEdge(Vector2Int cell, EdgeDirection direction)
		{
			return GetEdge(cell.x, cell.y, direction);
		}

		public byte GetEdge(Vector2Int from, Vector2Int to)
		{
			return GetEdge(from.x, from.y, GetDirectionBetween(from.x, from.y, to.x, to.y));
		}

		public bool HasFlag(int x, int y, EdgeDirection direction, byte flag)
		{
			return (GetEdge(x, y, direction) & flag) != 0;
		}

		public bool HasFlag(Vector2Int cell, EdgeDirection direction, byte flag)
		{
			return HasFlag(cell.x, cell.y, direction, flag);
		}

		public bool HasFlag(Vector2Int from, Vector2Int to, byte flag)
		{
			return (GetEdge(from, to) & flag) != 0;
		}

		public void SetFlag(int x, int y, EdgeDirection direction, byte flag)
		{
			int index = GetEdgeIndexFromDirection(x, y, direction, out bool isHorizontal);
			if (isHorizontal)
				southEdges[index] |= flag;
			else
				westEdges[index] |= flag;
		}

		public void SetFlag(Vector2Int cell, EdgeDirection direction, byte flag)
		{
			SetFlag(cell.x, cell.y, direction, flag);
		}

		public void SetFlag(Vector2Int from, Vector2Int to, byte flag)
		{
			SetFlag(from.x, from.y, GetDirectionBetween(from.x, from.y, to.x, to.y), flag);
		}

		public void ClearFlag(int x, int y, EdgeDirection direction, byte flag)
		{
			byte inverseMask = (byte)~flag;
			int index = GetEdgeIndexFromDirection(x, y, direction, out bool isHorizontal);
			if (isHorizontal)
				southEdges[index] &= inverseMask;
			else
				westEdges[index] &= inverseMask;
		}

		public void ClearFlag(Vector2Int cell, EdgeDirection direction, byte flag)
		{
			ClearFlag(cell.x, cell.y, direction, flag);
		}

		public void ClearFlag(Vector2Int from, Vector2Int to, byte flag)
		{
			ClearFlag(from.x, from.y, GetDirectionBetween(from.x, from.y, to.x, to.y), flag);
		}

		public void ClearAllEdgesOfFlag(byte flag)
		{
			byte inverseMask = (byte)~flag;
			for (int i = 0; i < southEdges.Length; i++) southEdges[i] &= inverseMask;
			for (int i = 0; i < westEdges.Length; i++) westEdges[i] &= inverseMask;
		}

		public void ClearAllEdges()
		{
			Array.Clear(southEdges, 0, southEdges.Length);
			Array.Clear(westEdges, 0, westEdges.Length);
		}

		#endregion

		#region Corners

		public byte GetCorner(int x, int y, CellCorner corner)
		{
			return corners[GetCornerIndex(x, y, corner)];
		}

		public byte GetCorner(Vector2Int cell, CellCorner corner)
		{
			return GetCorner(cell.x, cell.y, corner);
		}

		public bool HasCornerFlag(int x, int y, CellCorner corner, byte flag)
		{
			return (GetCorner(x, y, corner) & flag) != 0;
		}

		public bool HasCornerFlag(Vector2Int cell, CellCorner corner, byte flag)
		{
			return HasCornerFlag(cell.x, cell.y, corner, flag);
		}

		public void SetCornerFlag(int x, int y, CellCorner corner, byte flag)
		{
			corners[GetCornerIndex(x, y, corner)] |= flag;
		}

		public void SetCornerFlag(Vector2Int cell, CellCorner corner, byte flag)
		{
			SetCornerFlag(cell.x, cell.y, corner, flag);
		}

		public void ClearCornerFlag(int x, int y, CellCorner corner, byte flag)
		{
			corners[GetCornerIndex(x, y, corner)] &= (byte)~flag;
		}

		public void ClearCornerFlag(Vector2Int cell, CellCorner corner, byte flag)
		{
			ClearCornerFlag(cell.x, cell.y, corner, flag);
		}

		public void ClearAllCornersOfFlag(byte flag)
		{
			byte inverseMask = (byte)~flag;
			for (int i = 0; i < corners.Length; i++) corners[i] &= inverseMask;
		}

		public void ClearAllCorners()
		{
			Array.Clear(corners, 0, corners.Length);
		}

		#endregion

		#region Traversal

		public bool CanCrossEdge(int fromX, int fromY, int toX, int toY, byte blockedFlag, byte oneWayFlag)
		{
			EdgeDirection direction = GetDirectionBetween(fromX, fromY, toX, toY);
			byte edge = GetEdge(fromX, fromY, direction);

			if ((edge & blockedFlag) != 0)
				return false;

			if ((edge & oneWayFlag) != 0)
				if (direction == EdgeDirection.North || direction == EdgeDirection.East)
					return false;

			return true;
		}

		public bool CanCrossEdge(Vector2Int from, Vector2Int to, byte blockedFlag, byte oneWayFlag)
		{
			return CanCrossEdge(from.x, from.y, to.x, to.y, blockedFlag, oneWayFlag);
		}

		#endregion
	}
}