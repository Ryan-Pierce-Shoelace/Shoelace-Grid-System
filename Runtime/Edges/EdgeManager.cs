using UnityEngine;

namespace ShoelaceStudios.GridSystem.Edges
{
    public class EdgeManager : MonoBehaviour, IEdgeSystem
    {
        private EdgeRegistry registry;

        public bool IsInitialized { get; private set; }

        public void Initialize(IWorldGrid worldGrid)
        {
            if (IsInitialized) return;

            registry = new EdgeRegistry(worldGrid);
            IsInitialized = true;

            OnInitialized();
        }

        protected virtual void OnInitialized()
        {
        }

        #region Edges

        public EdgeFlags GetEdge(int x, int y, EdgeDirection direction)
        {
            return (EdgeFlags)registry.GetEdge(x, y, direction);
        }

        public EdgeFlags GetEdge(Vector2Int cell, EdgeDirection direction)
        {
            return (EdgeFlags)registry.GetEdge(cell, direction);
        }

        public EdgeFlags GetEdge(Vector2Int from, Vector2Int to)
        {
            return (EdgeFlags)registry.GetEdge(from, to);
        }

        public bool HasEdgeFlag(int x, int y, EdgeDirection direction, EdgeFlags flag)
        {
            return registry.HasFlag(x, y, direction, (byte)flag);
        }

        public bool HasEdgeFlag(Vector2Int cell, EdgeDirection direction, EdgeFlags flag)
        {
            return registry.HasFlag(cell, direction, (byte)flag);
        }

        public bool HasEdgeFlag(Vector2Int from, Vector2Int to, EdgeFlags flag)
        {
            return registry.HasFlag(from, to, (byte)flag);
        }

        public void SetEdgeFlag(int x, int y, EdgeDirection direction, EdgeFlags flag)
        {
            registry.SetFlag(x, y, direction, (byte)flag);
        }

        public void SetEdgeFlag(Vector2Int cell, EdgeDirection direction, EdgeFlags flag)
        {
            registry.SetFlag(cell, direction, (byte)flag);
        }

        public void SetEdgeFlag(Vector2Int from, Vector2Int to, EdgeFlags flag)
        {
            registry.SetFlag(from, to, (byte)flag);
        }

        public void ClearEdgeFlag(int x, int y, EdgeDirection direction, EdgeFlags flag)
        {
            registry.ClearFlag(x, y, direction, (byte)flag);
        }

        public void ClearEdgeFlag(Vector2Int cell, EdgeDirection direction, EdgeFlags flag)
        {
            registry.ClearFlag(cell, direction, (byte)flag);
        }

        public void ClearEdgeFlag(Vector2Int from, Vector2Int to, EdgeFlags flag)
        {
            registry.ClearFlag(from, to, (byte)flag);
        }

        public void ClearAllEdgesOfFlag(EdgeFlags flag)
        {
            registry.ClearAllEdgesOfFlag((byte)flag);
        }

        public void ClearAllEdges()
        {
            registry.ClearAllEdges();
        }

        #endregion

        #region Corners

        public CornerFlags GetCorner(int x, int y, CellCorner corner)
        {
            return (CornerFlags)registry.GetCorner(x, y, corner);
        }

        public CornerFlags GetCorner(Vector2Int cell, CellCorner corner)
        {
            return (CornerFlags)registry.GetCorner(cell, corner);
        }

        public bool HasCornerFlag(int x, int y, CellCorner corner, CornerFlags flag)
        {
            return registry.HasCornerFlag(x, y, corner, (byte)flag);
        }

        public bool HasCornerFlag(Vector2Int cell, CellCorner corner, CornerFlags flag)
        {
            return registry.HasCornerFlag(cell, corner, (byte)flag);
        }

        public void SetCornerFlag(int x, int y, CellCorner corner, CornerFlags flag)
        {
            registry.SetCornerFlag(x, y, corner, (byte)flag);
        }

        public void SetCornerFlag(Vector2Int cell, CellCorner corner, CornerFlags flag)
        {
            registry.SetCornerFlag(cell, corner, (byte)flag);
        }

        public void ClearCornerFlag(int x, int y, CellCorner corner, CornerFlags flag)
        {
            registry.ClearCornerFlag(x, y, corner, (byte)flag);
        }

        public void ClearCornerFlag(Vector2Int cell, CellCorner corner, CornerFlags flag)
        {
            registry.ClearCornerFlag(cell, corner, (byte)flag);
        }

        public void ClearAllCornersOfFlag(CornerFlags flag)
        {
            registry.ClearAllCornersOfFlag((byte)flag);
        }

        public void ClearAllCorners()
        {
            registry.ClearAllCorners();
        }

        public int GetCornerIndex(int x, int y, CellCorner corner)
        {
            return registry.GetCornerIndex(x, y, corner);
        }

        public int GetCornerIndex(Vector2Int cell, CellCorner corner)
        {
            return registry.GetCornerIndex(cell, corner);
        }

        #endregion

        #region Traversal

        public bool CanCrossEdge(int fromX, int fromY, int toX, int toY)
        {
            return registry.CanCrossEdge(fromX, fromY, toX, toY, (byte)EdgeFlags.Blocked, (byte)EdgeFlags.OneWay);
        }

        public bool CanCrossEdge(Vector2Int from, Vector2Int to)
        {
            return registry.CanCrossEdge(from, to, (byte)EdgeFlags.Blocked, (byte)EdgeFlags.OneWay);
        }

        #endregion
    }
}
