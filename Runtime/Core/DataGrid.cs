using System;
using ShoelaceStudios.GridSystem.Utils;
using UnityEngine;

namespace ShoelaceStudios.GridSystem.Core
{
	public class DataGrid<T> : IGrid<T>, IDataLayer
	{
		//https://blog.tedd.no/2021/10/07/why-c-multidimensional-arrays-are-slow/
		//
		// After a day spent running over data and stuff we could store the data in a flat array here too and use that lookup trick with width for the index 
		// So data could just be T[] and the class figure out the indexs from the vector2 ints. 
		// so we can have a method like     int Index( x,  y) => y * width + x  and the n all the internal classes take the x and y and just spit out an index.
		// It make for each loops maybe faster? or just less overhead on them  

		//Personally I think we should skip this. unless we have a way to test both versions since the AP i cleaner now sicnwe the real wins come from the worldspace and flood fill
		// But i wanted to make this not in case you know more than me

		// I also read you can create  struct enumerator with a Tdata rray and an index int with a move next method and a T current method
		// This would let us make a foreach without passing action like we did
		//  we again store the width. and build the values. then we have liek, a GetEnumerator that return the struct
		// And we do a  enumerator = datagrid.GetEnumerator
		// while (enumerator.Next(){
		// Do something e.current
		// Or e.Current.x e.currnt.y and e.current.val splitouts since wee do flat array math. 
		// This could be a Non alloc version of foreach and then we keep the normal on for other situations. 


		// post https://www.reddit.com/r/csharp/comments/1fppqcf/til_you_can_forward_enumerators_to_a_foreach_with/
		// https://nede.dev/blog/preventing-unnecessary-allocation-in-net-collections

		//Also might be worht to get htat Zlinq git amend did since its a zero alloc linqu and would make this wayeaier to read


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
			return GridMath.ValidateDimension(x, width) &&
			       GridMath.ValidateDimension(y, height);
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