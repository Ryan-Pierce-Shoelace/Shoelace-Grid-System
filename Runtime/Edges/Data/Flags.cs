using System;

namespace ShoelaceStudios.GridSystem.Edges
{
    [Flags]
    public enum EdgeFlags : byte
    {
        None = 0,
        Blocked = 1 << 0,
        OneWay = 1 << 1,
        Breakable = 1 << 2,
        Door = 1 << 3,
        Window = 1 << 4,
        Stairs = 1 << 5
    }

    [Flags]
    public enum CornerFlags : byte
    {
        None = 0,
        Blocked = 1 << 0,
        Breakable = 1 << 1,
        Cover = 1 << 2
    }
}
