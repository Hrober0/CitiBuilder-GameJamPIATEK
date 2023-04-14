using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WordGrid
{
    public class WorldGrid : MonoBehaviour
    {
        [SerializeField, Min(1)] private int _gridX = 10;
        [SerializeField, Min(1)] private int _gridY = 10;

        public static readonly Vector3 GRID_START = Vector3.zero;
        public const float CELL_SIZE = 1;

        private GCell[,] _cells;


        public static Vector3 GetWorldPos(Vector2Int gridPos) => GRID_START + new Vector3(gridPos.x * CELL_SIZE, 0, gridPos.y * CELL_SIZE);
        public static Vector2Int GetGridPos(Vector3 worldPos) => new Vector2Int(Mathf.FloorToInt(worldPos.x / CELL_SIZE), Mathf.FloorToInt(worldPos.z / CELL_SIZE));

        public static WorldGrid Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null)
            {
                Debug.LogError($"Mulitple instances {nameof(WorldGrid)}");
                return;
            }

            Instance = this;

            _cells = new GCell[_gridX, _gridY];
            for (int y = 0; y < _gridY; y++)
                for (int x = 0; x < _gridX; x++)
                    _cells[x, y] = new();
        }

        public bool BelognsToGrid(int x, int y) => x >= 0 && x < _gridX && y >= 0 && y < _gridY;
        public bool BelognsToGrid(Vector2Int gridPos) => BelognsToGrid(gridPos.x, gridPos.y);

        public GCell GetCell(int x, int y)
        {
            if (!BelognsToGrid(x, y))
            {
                Debug.LogError($"Cord out of grid x: {x} y: {y}");
                return null;
            }

            return _cells[x, y];
        }
        public GCell GetCell(Vector2Int gridPos) => GetCell(gridPos.x, gridPos.y);
        public GCell GetCell(Vector3 worldPos) => GetCell(GetGridPos(worldPos));

        public bool TryGetCell(int x, int y, out GCell cell)
        {
            if (BelognsToGrid(x, y))
            {
                cell = _cells[x, y];
                return true;
            }

            cell = null;
            return false;
        }
        public bool TryGetCell(Vector2Int gridPos, out GCell cell) => TryGetCell(gridPos.x, gridPos.y, out cell);
        public bool TryGetCell(Vector3 gridPos, out GCell cell) => TryGetCell(GetGridPos(gridPos), out cell);


#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.DrawLine(GRID_START, GRID_START + Vector3.right * _gridX);
            Gizmos.DrawLine(GRID_START, GRID_START + Vector3.forward * _gridY);
            for (int y = 1; y <= _gridY; y++)
                Gizmos.DrawLine(GRID_START + new Vector3(0, 0, y * CELL_SIZE), GRID_START + new Vector3(_gridX, 0, y * CELL_SIZE));
            for (int x = 1; x <= _gridX; x++)
                Gizmos.DrawLine(GRID_START + new Vector3(x * CELL_SIZE, 0, 0), GRID_START + new Vector3(x * CELL_SIZE, 0, _gridY));
        }
    }
#endif
}