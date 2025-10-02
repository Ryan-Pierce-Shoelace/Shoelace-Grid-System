using System;
using System.Collections.Generic;
using Shoelace.GridSystem.Data;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityUtils;

namespace Shoelace.GridSystem
{
	public class WorldGridManager : Singleton<WorldGridManager>
	{
		[Header("Tilemap Refs")]
		[SerializeField] private Tilemap wallTileMap;
		[SerializeField] private Grid grid;
		[SerializeField] private TileBase wallTile;
		[Header("Grid Settings")]
		[SerializeField] private int gridWidth;
		[SerializeField] private int gridHeight;
		[SerializeField] private bool buildPerimeterWall;
		[SerializeField] private GridSettingsSO settings;

		[Header("GameGrids")]
		public int GridWidth => gridWidth;

		public int GridHeight => gridHeight;
		public Vector2 TotalWorldSize => new(gridWidth, gridHeight);
		public float CellSize => grid.cellSize.x;
		public Vector3 WorldPlacementOffset => settings.WorldPlacementOffset(gridHeight, gridWidth, transform);
		public HashSet<Vector2Int> WallPositions;

		#region Setup

		public void CreateGridMap()
		{
			if (buildPerimeterWall)
			{
				GenerateGridPerimeter();
			}
			GatherGridData();
		}



		private void GatherGridData()
		{
			WallPositions = new HashSet<Vector2Int>();
			
			for (int x = 0; x < gridWidth; x++)
			{
				for (int y = 0; y < gridHeight; y++)
				{
					Vector3Int tilePosition = new Vector3Int(x, y, 0);

					if (wallTileMap.HasTile(tilePosition))
					{
						WallPositions.Add((Vector2Int)tilePosition);
					}
				}
			}
		}

		private void GenerateGridPerimeter()
		{
			for (int x = 0; x < gridWidth; x++)
			{
				for (int y = 0; y < gridHeight; y++)
				{
					Vector3Int tilePosition = new Vector3Int(x, y, 0);
					if (IsBorderTile(x, y))
					{
						wallTileMap.SetTile(tilePosition, wallTile);
					}
				}
			}
		}

		#endregion

		#region Helpers

		public void ForEachGridPosition(Action<int, int> action)
		{
			for (int x = 0; x < gridWidth; x++)
			{
				for (int y = 0; y < gridHeight; y++)
				{
					action(x, y);
				}
			}
		}

		public void ForEachNeighbors(int posX, int posY, Action<int, int> action)
		{
			for (int x = posX - 1; x <= posX + 1; x++)
			{
				for (int y = posY - 1; y <= posY + 1; y++)
				{
					if (x == posX && y == posY)
						continue;

					if (!IsValidCell(x, y))
					{
						continue;
					}

					action(x, y);
				}
			}
		}

		private bool IsBorderTile(int x, int y)
		{
			return x == 0 || y == 0 || x == gridWidth - 1 || y == gridHeight - 1;
		}

		public Vector3 CellToWorldSpace(Vector2Int coord)
		{
			return CellToWorldSpace(coord.x, coord.y);
		}

		public Vector3 CellToWorldSpace(int x, int y)
		{
			return new Vector3(x, y, 0) * CellSize + (new Vector3(1, 1, 0) * CellSize * .5f);
		}

		public Vector2Int GetCell(Vector3 worldPosition)
		{
			Vector3Int cellPosition = grid.WorldToCell(worldPosition);
			cellPosition.z = 0;
			return IsValidCell(cellPosition.x, cellPosition.y) ? (Vector2Int)cellPosition : default;
		}

		public bool IsValidCell(Vector2Int coord)
		{
			return IsValidCell(coord.x, coord.y);
		}

		public bool IsValidCell(int x, int y)
		{
			return x >= 0 && x < gridWidth && y >= 0 && y < gridHeight;
		}

		public bool IsWallCell(Vector2Int coord)
		{
			return WallPositions.Contains(coord);
		}

		public bool IsWallCell(int x, int y)
		{
			return IsWallCell(new Vector2Int(x, y));
		}

		public void AddWallCell(Vector2Int coord)
		{
			if (!IsWallCell(coord))
			{
				WallPositions.Add(coord);
				Debug.DrawLine(CellToWorldSpace(coord) + (Vector3.one * CellSize * .5f), CellToWorldSpace(coord) + (Vector3.one * CellSize * .5f), Color.red, 10f);
			}
		}

		public void RemoveWallCell(Vector2Int coord)
		{
			if (IsWallCell(coord))
			{
				WallPositions.Remove(coord);
			}
		}
		
		public void GetGridSpaceDimensions(out float width, out float height)
		{
			if (gridWidth == 0 || gridHeight == 0)
			{
				CreateGridMap();
			}

			width = (gridWidth) * CellSize;
			height = (gridHeight) * CellSize;
		}

		#endregion
	}
}