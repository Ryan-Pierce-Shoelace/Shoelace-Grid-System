namespace Shoelace.GridSystem.Data
{
    [System.Serializable]
    public class DataGrid<T>
    {
        private readonly T[,] gridData;
        private readonly int width;
        private readonly int height;

        public DataGrid(int width, int height)
        {
            this.width = width;
            this.height = height;
            gridData = new T[width, height];
        }

        public T this[int x, int y]
        {
            get => GetValue(x, y);
            set => SetValue(x, y, value);
        }

        public T GetValue(int x, int y)
        {
            return !IsValid(x, y) ? default : gridData[x, y];
        }

        public void SetValue(int x, int y, T value)
        {
            if(!IsValid(x, y)) { return; }

            gridData[x, y] = value;
        }

        private bool IsValid(int x, int y)
        {
            return x >= 0 && x < width && y >= 0 && y < height;
        }
    }
}
