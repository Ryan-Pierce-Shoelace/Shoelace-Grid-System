using System;

namespace ShoelaceStudios.GridSystem.Core
{
	[Flags]
	public enum CellFlags : byte
	{
		None = 0,
		Blocked = 1 << 0,
		Slow = 1 << 1,
	}
}