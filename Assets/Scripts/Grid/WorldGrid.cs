using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

namespace WordGrid
{
    public class WorldGrid : MonoBehaviour
    {
        public RectInt GridSize => new RectInt(Vector2Int.zero, new Vector2Int(_gridX, _gridY));

        public int GridX => _gridX;
        public int GridY => _gridY;

        [SerializeField, Min(1)] private int _gridX = 10;
        [SerializeField, Min(1)] private int _gridY = 10;

        // DO NOT CHANGE UNLESS YOU WANT TO BREAK EVERYTHING
        public readonly Vector3 _gridStart = Vector3.zero;

        private GCell[,] _cells;

        public static WorldGrid Instance { get; private set; }

        private void Awake()
        {
            Assert.IsNull(Instance, $"Mulitple instances {nameof(WorldGrid)}");
            Instance = this;

            _cells = new GCell[_gridX, _gridY];
            for (int y = 0; y < _gridY; y++)
                for (int x = 0; x < _gridX; x++)
                    _cells[x, y] = new();
        }

        public bool BelognsToGrid(int x, int y) => x >= 0 && x < _gridX && y >= 0 && y < _gridY;

        public GCell GetCell(int x, int y)
        {
            if (!BelognsToGrid(x, y))
            {
                Debug.LogError($"Cord out of grid x: {x} y: {y}");
                return null;
            }

            return _cells[x, y];
        }
        public GCell GetCell(Vector2Int pos) => GetCell(pos.x, pos.y);
        public GCell GetCell(Vector2 pos) => GetCell(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y));

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

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.DrawLine(_gridStart, _gridStart + Vector3.right * _gridX);
            Gizmos.DrawLine(_gridStart, _gridStart + Vector3.forward * _gridY);
            for (int y = 1; y <= _gridY; y++)
                Gizmos.DrawLine(_gridStart + new Vector3(0, 0, y), _gridStart + new Vector3(_gridX, 0, y));
            for (int x = 1; x <= _gridX; x++)
                Gizmos.DrawLine(_gridStart + new Vector3(x, 0, 0), _gridStart + new Vector3(x, 0, _gridY));

            if (_cells != null)
            {
                for (int y = 0; y < _gridY; y++)
                    for (int x = 0; x < _gridX; x++)
                    {
                        var cell = GetCell(x, y);
                        if (cell.GridObject != null)
                            Handles.Label(_gridStart + new Vector3(0.1f, 0, 0.5f) + new Vector3(x, 0, y), cell.GridObject.DisplayedName);
                    }
            }
        }
    }
#endif
}