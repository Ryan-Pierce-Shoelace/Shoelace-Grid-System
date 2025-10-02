using UnityEngine;

namespace Shoelace.GridSystem.Data
{
    [CreateAssetMenu(fileName = "New GridSettings", menuName = "GridSystem/GridSettings")]
    public class GridSettingsSO : ScriptableObject
    {
        public float CellSize;
        public Vector2 CellAnchor;

        public Vector3 WorldPlacementOffset(int height, int width, Transform gridTransform)
        {
            return new Vector3(
                (width * CellSize * 0.5f) + gridTransform.position.x,
                (height * CellSize * 0.5f) + gridTransform.position.y,
                gridTransform.position.z
            );
        }
    }
}
