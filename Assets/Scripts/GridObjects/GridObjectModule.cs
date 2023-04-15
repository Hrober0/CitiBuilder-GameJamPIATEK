using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace GridObjects
{
    [RequireComponent(typeof(GridObject))]
    public abstract class GridObjectModule : MonoBehaviour
    {
        public GridObject GridObject { get; private set; }

        public virtual void OnBuildingConstructed()
        {
            GridObject = GetComponent<GridObject>();
        }
        public virtual void OnBuildingDestroyed() { }
    }
}