using System;
using UnityEngine;

namespace ShoelaceStudios.GridSystem.Utils
{
	[Serializable]
	public struct GridEdge : IEquatable<GridEdge>
	{
		public enum CellEdge
		{
			Top,
			Bottom,
			Left,
			Right
		}

		public Vector2Int Cell { get; }
		public CellEdge Edge { get; }

		public GridEdge(Vector2Int cell, CellEdge edge)
		{
			Cell = cell;
			Edge = edge;
		}

		#region Equality

		public bool Equals(GridEdge other)
		{
			return Cell.Equals(other.Cell) && Edge == other.Edge;
		}

		public override bool Equals(object obj)
		{
			return obj is GridEdge other && Equals(other);
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(Cell, (int)Edge);
		}

		#endregion

		public GridEdge GetOppositeGridEdge()
		{
			return new GridEdge(GetNeighborCell(), GetOppositeEdge());
		}

		public Vector2Int GetNeighborCell()
		{
			return Edge switch
			{
				CellEdge.Top => Cell + Vector2Int.up,
				CellEdge.Bottom => Cell + Vector2Int.down,
				CellEdge.Left => Cell + Vector2Int.left,
				CellEdge.Right => Cell + Vector2Int.right,
				_ => throw new ArgumentOutOfRangeException()
			};
		}

		public CellEdge GetOppositeEdge()
		{
			return Edge switch
			{
				CellEdge.Top => CellEdge.Bottom,
				CellEdge.Bottom => CellEdge.Top,
				CellEdge.Left => CellEdge.Right,
				CellEdge.Right => CellEdge.Left,
				_ => throw new ArgumentOutOfRangeException()
			};
		}


		public Vector3 GetEdgeMiddle(IWorldGrid grid)
		{
			Vector3 cellCenter = grid.GetWorldFromCell(Cell.x, Cell.y);
			float half = grid.CellSize * 0.5f;

			return Edge switch
			{
				CellEdge.Top => cellCenter + new Vector3(0, half, 0),
				CellEdge.Bottom => cellCenter + new Vector3(0, -half, 0),
				CellEdge.Left => cellCenter + new Vector3(-half, 0, 0),
				CellEdge.Right => cellCenter + new Vector3(half, 0, 0),
				_ => throw new ArgumentOutOfRangeException()
			};
		}

		public void GetWorldVerts(IWorldGrid grid, Vector3[] output)
		{
			if (output == null || output.Length < 2)
			{
				Debug.LogError("[GridEdge] Output array must have length >= 2.");
				return;
			}

			Vector3 center = grid.GetWorldFromCell(Cell.x, Cell.y);
			float half = grid.CellSize * 0.5f;

			Vector3 bl = center + new Vector3(-half, -half, 0);
			Vector3 br = center + new Vector3(half, -half, 0);
			Vector3 tl = center + new Vector3(-half, half, 0);
			Vector3 tr = center + new Vector3(half, half, 0);

			switch (Edge)
			{
				case CellEdge.Top:
					output[0] = tl;
					output[1] = tr;
					break;
				case CellEdge.Bottom:
					output[0] = bl;
					output[1] = br;
					break;
				case CellEdge.Left:
					output[0] = bl;
					output[1] = tl;
					break;
				case CellEdge.Right:
					output[0] = br;
					output[1] = tr;
					break;
				default:
					output[0] = bl;
					output[1] = br;
					break;
			}
		}

		public Vector3[] ToWorldVerts(IWorldGrid grid)
		{
			Vector3[] verts = new Vector3[2];
			GetWorldVerts(grid, verts);
			return verts;
		}
	}
}