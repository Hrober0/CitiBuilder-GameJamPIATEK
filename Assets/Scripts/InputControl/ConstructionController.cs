using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GridObjects;
using WordGrid;

namespace InputControll
{
    public class ConstructionController : MonoBehaviour
    {
        [SerializeField] private Camera _camera;
        [SerializeField] private LayerMask _colisionLayers;

        [Header("transforms")]
        [SerializeField] private Transform _objectsParent;
        [SerializeField] private Transform _objectVisualizationParent;


        private GridObject _objectVisualization = null;
        private Coroutine _constructionUpdater = null;


        public void SetObject(GridObject selectedObject)
        {
            if (_objectVisualization != null)
            {
                Destroy(_objectVisualization);
                _objectVisualization = null;
            }

            if (selectedObject != null)
            {
                _objectVisualization = Instantiate(selectedObject, _objectVisualizationParent);
                _objectVisualization.name = $"{selectedObject}-visualization";
                _objectVisualization.transform.position = Vector3.one * -10;

                if (_constructionUpdater == null)
                    _constructionUpdater = StartCoroutine(ConstructionUpdate());
            }
        }

        public void BuildObject(Vector2Int gridPos, GridObject objPattern)
        {
            if (!CanBuildObjectAt(gridPos, objPattern))
            {
                Debug.LogError($"Attempted to build {objPattern.name} object at {gridPos}");
                return;
            }

            var newObj = Instantiate(objPattern, _objectsParent);
            newObj.transform.position = WorldGrid.GetWorldPos(gridPos);
            newObj.name = $"{objPattern}({gridPos.x},{gridPos.y})";

            foreach (var field in newObj.Fields)
                WorldGrid.Instance.GetCell(field + gridPos).GridObject = newObj;
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


        private IEnumerator ConstructionUpdate()
        {
            WorldGrid grid = WorldGrid.Instance;
            while (_objectVisualization != null)
            {
                var gridPos = GetMouseGridPosition();

                // update visualizatio position
                if (IsInGrid(gridPos))
                    _objectVisualization.transform.position = WorldGrid.GetWorldPos(gridPos);
                
                // handle build
                if (Input.GetMouseButtonUp(0))
                {
                    if (CanBuildObjectAt(gridPos, _objectVisualization))
                    {
                        BuildObject(gridPos, _objectVisualization);
                        SetObject(null);
                    }
                    else
                    {
                        Debug.Log("Nope");
                    }
                }

                yield return null;
            }

            _constructionUpdater = null;

            bool IsInGrid(Vector2Int gridPos)
            {
                foreach (var field in _objectVisualization.Fields)
                    if (!grid.BelognsToGrid(gridPos + field))
                        return false;
                return true;
            }
        }

        private Vector2Int GetMouseGridPosition()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (!Physics.Raycast(ray, out RaycastHit hitData, 1000, _colisionLayers))
                return -Vector2Int.one;
            var worldPos = hitData.point;
            return WorldGrid.GetGridPos(worldPos);
        }
    }
}