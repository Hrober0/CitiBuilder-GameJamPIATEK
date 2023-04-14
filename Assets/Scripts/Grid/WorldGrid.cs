using System.Collections;
using System.Collections.Generic;
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
        }

        public GCell GetCell(int x, int y)
        {
            if (x < 0 || x >= _gridX || y < 0 || y >= _gridY)
            {
                Debug.LogError($"Cord out of grid x: {x} y: {y}");
                return null;
            }

            return _cells[x, y];
        }
        public GCell GetCell(Vector2Int pos) => GetCell(pos.x, pos.y);
        public GCell GetCell(Vector2 pos) => GetCell(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y));


        private void OnDrawGizmos()
        {
            Gizmos.DrawLine(_gridStart, _gridStart + Vector3.right * _gridX);
            Gizmos.DrawLine(_gridStart, _gridStart + Vector3.forward * _gridY);
        }
    }
}