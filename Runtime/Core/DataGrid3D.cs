using System;
using ShoelaceStudios.GridSystem.Utils;

namespace ShoelaceStudios.GridSystem.Core
{
	public class DataGrid3D<T>
	{
		public int Width => width;
		public int Height => height;
		public int Depth => depth;

		private readonly T[,,] gridData;
		private readonly int width;
		private readonly int height;
		private readonly int depth;

		public DataGrid3D(int gridWidth, int gridHeight, int gridDepth)
		{
			width = gridWidth;
			height = gridHeight;
			depth = gridDepth;
			gridData = new T[width, height, depth];
		}

		public T GetValue(int x, int y, int z)
		{
			return IsValid(x, y, z) ? gridData[x, y, z] : throw new ArgumentOutOfRangeException($"Cell ({x},{y}, {z}) out of bounds ({width}x{height}x{depth}).");
		}

		public void SetValue(int x, int y, int z, T value)
		{
			if (!IsValid(x, y, z))
			{
				throw new ArgumentOutOfRangeException($"Cell ({x},{y}, {z}) out of bounds ({width}x{height}x{depth}).");
			}

			gridData[x, y, z] = value;
		}

		public void Clear()
		{
			Array.Clear(gridData, 0, gridData.Length);
		}

		private bool IsValid(int x, int y, int z)
		{
			return GridMath.ValidateDimension(x, width) &&
			       GridMath.ValidateDimension(y, height) &&
			       GridMath.ValidateDimension(z, depth);
		}



		public void ForEachCell(Action<int, int, int> action)
		{
			for (int x = 0; x < width; x++)
			{
				for (int y = 0; y < height; y++)
				{
					for (int z = 0; z < depth; z++)
					{
						action(x, y, z);
					}
				}
			}
		}
	}
}