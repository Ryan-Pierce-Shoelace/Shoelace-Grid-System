namespace ShoelaceStudios.GridSystem
{
	public struct FloodFillParams
	{
		public int MaxSteps;
		public float MaxRadius;
		public bool StopAtWalls;

		public static FloodFillParams UnlimitedSteps(bool stopAtWalls = true)
		{
			return new FloodFillParams
			{
				MaxSteps = 0,
				MaxRadius = 0,
				StopAtWalls = stopAtWalls
			};
		}

		public static FloodFillParams WithSteps(int steps, bool stopAtWalls = true)
		{
			return new FloodFillParams
			{
				MaxSteps = steps,
				MaxRadius = 0,
				StopAtWalls = stopAtWalls
			};
		}

		public static FloodFillParams WithRadius(float radius, bool stopAtWalls = true)
		{
			return new FloodFillParams
			{
				MaxSteps = 0,
				MaxRadius = radius,
				StopAtWalls = stopAtWalls
			};
		}

		public bool HasStepLimit => MaxSteps > 0;
		public bool HasRadiusLimit => MaxRadius > 0f;
	}
}