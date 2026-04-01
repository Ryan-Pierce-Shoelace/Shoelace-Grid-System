namespace ShoelaceStudios.GridSystem
{
	public readonly struct FloodFillParams
	{
		public readonly int MaxSteps;
		public readonly float MaxRadius;
		public readonly bool StopAtWalls;
		public readonly bool Diagonals;
		public readonly bool TrackDepth;
		public readonly bool[] RegionMask;

		public bool HasStepLimit => MaxSteps > 0;

		public bool HasRadiusLimit => MaxRadius > 0f;

		public bool HasRegionMask => RegionMask != null;

		private FloodFillParams(int maxSteps, float maxRadius, bool stopAtWalls, bool diagonals, bool trackDepth, bool[] regionMask)
		{
			MaxSteps = maxSteps;
			MaxRadius = maxRadius;
			StopAtWalls = stopAtWalls;
			Diagonals = diagonals;
			TrackDepth = trackDepth;
			RegionMask = regionMask;
		}

		public static FloodFillParams Unlimited(bool stopAtWalls = true, bool diagonals = false, bool trackDepth = false)
		{
			return new FloodFillParams(0, 0f, stopAtWalls, diagonals, trackDepth, null);
		}

		public static FloodFillParams WithSteps(int steps, bool stopAtWalls = true, bool diagonals = false, bool trackDepth = false)
		{
			return new FloodFillParams(steps, 0f, stopAtWalls, diagonals, trackDepth, null);
		}

		public static FloodFillParams WithRadius(float radius, bool stopAtWalls = true, bool diagonals = false, bool trackDepth = false)
		{
			return new FloodFillParams(0, radius, stopAtWalls, diagonals, trackDepth, null);
		}

		public static FloodFillParams WithStepsAndRadius(int steps, float radius, bool stopAtWalls = true, bool diagonals = false, bool trackDepth = false)
		{
			return new FloodFillParams(steps, radius, stopAtWalls, diagonals, trackDepth, null);
		}

		public static FloodFillParams WithRegionMask(bool[] mask, bool stopAtWalls = true, bool diagonals = false, bool trackDepth = false)
		{
			return new FloodFillParams(0, 0f, stopAtWalls, diagonals, trackDepth, mask);
		}
	}
}