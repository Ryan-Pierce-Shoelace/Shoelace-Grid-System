using System;
using UnityEngine;

namespace ShoelaceStudios.GridSystem
{
	public interface IGrid<T>
	{
		int Width { get; }
		int Height { get; }
		T GetValue(int x, int y);
		T GetValue(Vector2Int pos);
		void SetValue(int x, int y, T value);
		void SetValue(Vector2Int pos, T value);
		void Clear();
		void ForEachCell(Action<int, int> action);
		void ForEachCell(Action<int, int, T> action);
	}
}