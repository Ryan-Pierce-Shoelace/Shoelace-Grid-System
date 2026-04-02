using System;
using ShoelaceStudios.GridSystem.Utils;
using UnityEngine;

namespace ShoelaceStudios.GridSystem.Core
{
	public class DataGrid<T> : IGrid<T>, IDataLayer
	{
		public string LayerName { get; }

		public int Width => width;
		public int Height => height;

		private readonly T[,] gridData;
		private readonly int width;
		private readonly int height;

		public DataGrid(int gridWidth, int gridHeight, string name = "")
		{
			width = gridWidth;
			height = gridHeight;
			LayerName = name;
			gridData = new T[width, height];
		}
		
		public T GetValue(int x, int y)
		{
			return IsValid(x, y) ? gridData[x, y] : throw new ArgumentOutOfRangeException($"Cell ({x},{y}) out of bounds ({width}x{height}).");
		}

		public T GetValue(Vector2Int cell)
		{
			return GetValue(cell.x, cell.y);
		}

		public void SetValue(int x, int y, T value)
		{
			if (!IsValid(x, y)) throw new ArgumentOutOfRangeException($"Cell ({x},{y}) out of bounds ({width}x{height}).");
			gridData[x, y] = value;
		}

		public void SetValue(Vector2Int cell, T value)
		{
			SetValue(cell.x, cell.y, value);
		}

		public void Clear()
		{
			Array.Clear(gridData, 0, gridData.Length);
		}

		private bool IsValid(int x, int y)
		{
			return GridMath.ValidateDimension(x, width) && GridMath.ValidateDimension(y, height);
		}
		
		public void ForEachCell(Action<int, int> action)
		{
			for (int x = 0; x < width; x++)
			for (int y = 0; y < height; y++)
				action(x, y);
		}

		public void ForEachCell(Action<int, int, T> action)
		{
			for (int x = 0; x < width; x++)
			for (int y = 0; y < height; y++)
				action(x, y, gridData[x, y]);
		}
	}
}