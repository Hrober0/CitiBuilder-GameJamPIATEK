using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WordGrid;

namespace GridObjects
{
    public class GridObject : MonoBehaviour
    {
        public GCell[] OccupiedTiles
        {
            get
            {
                //TODO change to support multicell
                return new GCell[] { WorldGrid.Instance.GetCell(new Vector2(transform.position.x, transform.position.z)) };
            }
        }
        [SerializeField] private string _name = "default";
        public string DisplayedName => _name;


        [SerializeField] private GridObjectTypeSO _type;
        public GridObjectTypeSO Type => _type;


        [SerializeField] private Vector2Int[] _fields = new Vector2Int[] { Vector2Int.zero };
        public IReadOnlyList<Vector2Int> Fields => _fields;
    }
}
