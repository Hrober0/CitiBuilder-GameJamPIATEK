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
                return new GCell[] { WorldGrid.Instance.GetCell(transform.position) };
            }
        }
    }
}
