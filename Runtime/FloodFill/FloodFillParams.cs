namespace ShoelaceStudios.GridSystem.FloodFill
{
	public readonly struct FloodFillParams
	{
		public int MaxSteps { get; }
		public float MaxRadius { get; }
		public bool StopAtWalls { get; }

		public bool HasStepLimit => MaxSteps > 0;
		public bool HasRadiusLimit => MaxRadius > 0f;

		private FloodFillParams(int maxSteps = 0, float maxRadius = 0f, bool stopAtWalls = true)
		{
			MaxSteps = maxSteps > 0 ? maxSteps : 0;
			MaxRadius = maxRadius > 0f ? maxRadius : 0f;
			StopAtWalls = stopAtWalls;
		}

		public static FloodFillParams Unlimited(bool stopAtWalls = true) => new(0, 0f, stopAtWalls);
		public static FloodFillParams WithSteps(int steps, bool stopAtWalls = true) => new(steps, 0f, stopAtWalls);
		public static FloodFillParams WithRadius(float radius, bool stopAtWalls = true) => new(0, radius, stopAtWalls);
		public static FloodFillParams WithStepsAndRadius(int steps, float radius, bool stopAtWalls = true) => new(steps, radius, stopAtWalls);
	}
}