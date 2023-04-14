using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Grids;

namespace GridObjects
{
    public class GridObject : MonoBehaviour
    {
        [SerializeField] private string _name = "default";
        public string DisplayedName => _name;


        [SerializeField] private GridObjectTypeSO _type;
        public GridObjectTypeSO Type => _type;


        [SerializeField] private Vector2Int[] _fields = new Vector2Int[] { Vector2Int.zero };
        public IReadOnlyList<Vector2Int> Fields => _fields;


        [SerializeField] private GridObjectTypeSO[] _reqiredObjects = new GridObjectTypeSO[0];
        public IReadOnlyList<GridObjectTypeSO> ReqiredObjects => _reqiredObjects;


#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            foreach (var field in _fields)
            {
                Vector3 pos = transform.position + new Vector3(field.x, 0, field.y) * WorldGrid.CELL_SIZE;
                Gizmos.DrawLine(pos, pos + new Vector3(WorldGrid.CELL_SIZE, 0, WorldGrid.CELL_SIZE));
                Gizmos.DrawLine(pos + new Vector3(WorldGrid.CELL_SIZE, 0, 0), pos + new Vector3(0, 0, WorldGrid.CELL_SIZE));
            }

            UnityEditor.Handles.Label(transform.position + new Vector3(0, 0, WorldGrid.CELL_SIZE / 2), DisplayedName);
        }
#endif
    }
}
