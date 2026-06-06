using ShoelaceStudios.GridSystem.Edges;
using UnityEngine;

namespace ShoelaceStudios.GridSystem
{
    public interface IEdgeSystem
    {
        EdgeFlags GetEdge(int x, int y, EdgeDirection direction);
        EdgeFlags GetEdge(Vector2Int cell, EdgeDirection direction);
        EdgeFlags GetEdge(Vector2Int from, Vector2Int to);

        bool HasEdgeFlag(int x, int y, EdgeDirection direction, EdgeFlags flag);
        bool HasEdgeFlag(Vector2Int cell, EdgeDirection direction, EdgeFlags flag);

        void SetEdgeFlag(int x, int y, EdgeDirection direction, EdgeFlags flag);
        void SetEdgeFlag(Vector2Int cell, EdgeDirection direction, EdgeFlags flag);

        void ClearEdgeFlag(int x, int y, EdgeDirection direction, EdgeFlags flag);
        void ClearEdgeFlag(Vector2Int cell, EdgeDirection direction, EdgeFlags flag);

        void ClearAllEdges();
        void ClearAllEdgesOfFlag(EdgeFlags flag);

        CornerFlags GetCorner(int x, int y, CellCorner corner);
        CornerFlags GetCorner(Vector2Int cell, CellCorner corner);

        bool HasCornerFlag(int x, int y, CellCorner corner, CornerFlags flag);
        bool HasCornerFlag(Vector2Int cell, CellCorner corner, CornerFlags flag);

        void SetCornerFlag(int x, int y, CellCorner corner, CornerFlags flag);
        void SetCornerFlag(Vector2Int cell, CellCorner corner, CornerFlags flag);

        void ClearCornerFlag(int x, int y, CellCorner corner, CornerFlags flag);
        void ClearCornerFlag(Vector2Int cell, CellCorner corner, CornerFlags flag);

        void ClearAllCorners();
        void ClearAllCornersOfFlag(CornerFlags flag);
        int GetCornerIndex(int x, int y, CellCorner corner);


        bool CanCrossEdge(int fromX, int fromY, int toX, int toY);
        bool CanCrossEdge(Vector2Int from, Vector2Int to);
    }
}
