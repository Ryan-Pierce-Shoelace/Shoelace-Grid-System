namespace ShoelaceStudios.GridSystem
{
    public class ChunkRuntimeState
    {
        public float TimeAccumulator = 0f;
        public bool WasVisibleLastFrame = false;
        public bool IsVisibleThisFrame = false;

        public void BeginFrame() => IsVisibleThisFrame = false;

        public void AddDelta(float deltaTime) => TimeAccumulator += deltaTime;

        public bool BecameVisible() => IsVisibleThisFrame && !WasVisibleLastFrame;
        public bool BecameInvisible() => !IsVisibleThisFrame && WasVisibleLastFrame;

        public float ConsumeAccumulatedTime()
        {
            float t = TimeAccumulator;
            TimeAccumulator = 0f;
            return t;
        }

        public void EndFrame()
        {
            WasVisibleLastFrame = IsVisibleThisFrame;
        }
    }
}
