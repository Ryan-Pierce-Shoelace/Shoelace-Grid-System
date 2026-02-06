namespace ShoelaceStudios.GridSystem
{
	public readonly struct FloodFillParams
	{
		private readonly int maxSteps;
		private readonly float maxRadius;
		private readonly bool stopAtWalls;

		public int MaxSteps => maxSteps;
		public float MaxRadius => maxRadius;
		public bool StopAtWalls => stopAtWalls;

		public bool HasStepLimit => maxSteps > 0;
		public bool HasRadiusLimit => maxRadius > 0f;

		private FloodFillParams(int maxSteps = 0, float maxRadius = 0f, bool stopAtWalls = true)
		{
			this.maxSteps = maxSteps > 0 ? maxSteps : 0;
			this.maxRadius = maxRadius > 0f ? maxRadius : 0f;
			this.stopAtWalls = stopAtWalls;
		}

		public static FloodFillParams Unlimited(bool stopAtWalls = true)
		{
			return new FloodFillParams(0, 0f, stopAtWalls);
		}

		public static FloodFillParams WithSteps(int steps, bool stopAtWalls = true)
		{
			return new FloodFillParams(steps, 0f, stopAtWalls);
		}

		public static FloodFillParams WithRadius(float radius, bool stopAtWalls = true)
		{
			return new FloodFillParams(0, radius, stopAtWalls);
		}

		public static FloodFillParams WithStepsAndRadius(int steps, float radius, bool stopAtWalls = true)
		{
			return new FloodFillParams(steps, radius, stopAtWalls);
		}
	}
}