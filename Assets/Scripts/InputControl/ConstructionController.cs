using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GridObjects;
using WordGrid;

namespace InputControll
{
    public class ConstructionController : MonoBehaviour
    {
        private GridObjectTypeSO _selectedObject = null;

        

        private void Update()
        {
            if (_selectedObject == null)
                return;
        }


        public void SetObject(GridObjectTypeSO selectedObject)
        {
            _selectedObject = selectedObject;
        }

        public void BuildObject(Vector2Int gridPos, GridObject obj)
        {
            if (!CanBuildObjectAt(gridPos, obj))
            {
                Debug.LogError($"Attempted to build {obj.name} object at {gridPos}");
                return;
            }

            foreach (var field in obj.Fields)
            {
                WorldGrid.Instance.GetCell(field + gridPos).GridObject = obj;
            }
        }

        public bool CanBuildObjectAt(Vector2Int gridPos, GridObject obj)
        {
            foreach (var field in obj.Fields)
            {
                if (!WorldGrid.Instance.TryGetCell(field + gridPos, out var cell))
                    return false;

                if (cell.GridObject != null)
                    return false;
            }

            return true;
        }
    }
}