using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GridObjects;
using Grids;
using System;

namespace InputControll
{
    public class ConstructionController : MonoBehaviour
    {
        [SerializeField] private Camera _camera;
        [SerializeField] private LayerMask _colisionLayers;

        [Header("construction")]
        [SerializeField] private Transform _objectsParent;
        [SerializeField] private Transform _objectVisualizationParent;

        [Header("places")]
        [SerializeField] private Transform _placesParent;
        [SerializeField] private GameObject _placePattern;


        private GridObject _objectVisualization = null;
        private Coroutine _constructionUpdater = null;

        private readonly List<GameObject> _activePlaces = new();
        private readonly List<GameObject> _unactivePlaces = new();


        public event Action<GridObject> OnGridObjectSelected;

        public void SetObject(GridObject selectedObject)
        {
            if (_objectVisualization != null)
            {
                Destroy(_objectVisualization.gameObject);
                _objectVisualization = null;
            }

            SetAvailableToBuildPlaces(selectedObject);

            if (selectedObject != null)
            {
                _objectVisualization = Instantiate(selectedObject, _objectVisualizationParent);
                _objectVisualization.name = $"{selectedObject}-visualization";
                _objectVisualization.gameObject.SetActive(false);

                if (_constructionUpdater == null)
                    _constructionUpdater = StartCoroutine(ConstructionUpdate());
            }

            OnGridObjectSelected?.Invoke(selectedObject);
        }

        public void BuildObject(Vector2Int gridPos, GridObject objPattern, bool chack=true)
        {
            if (chack && !CanBuildObjectAt(gridPos, objPattern))
            {
                Debug.LogError($"Attempted to build {objPattern.name} object at {gridPos}");
                return;
            }

            var newObj = Instantiate(objPattern, _objectsParent);
            newObj.transform.position = WorldGrid.GetWorldPos(gridPos);
            newObj.name = $"{objPattern}({gridPos.x},{gridPos.y})";

            foreach (var field in newObj.Fields)
                WorldGrid.Instance.GetCell(field + gridPos).GridObject = newObj;

            newObj.Place();
        }
        
        public bool CanBuildObjectAt(Vector2Int gridPos, GridObject obj)
        {
            foreach (var field in obj.Fields)
            {
                if (!WorldGrid.Instance.TryGetCell(field + gridPos, out var cell))
                    return false;

                if (cell.GridObject != null)
                    return false;

                if (!HasAtLeastOneBuild(gridPos, obj.ReqiredObjects))
                    return false;
            }

            return true;
        }
        private bool HasAtLeastOneBuild(Vector2Int gridPos, IReadOnlyList<GridObjectTypeSO> types)
        {
            if (types.Count == 0)
                return true;

            var offsets = new[] { Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left };
            var typesList = new List<GridObjectTypeSO>(types);
            foreach (var offset in offsets)
                if (WorldGrid.Instance.TryGetCell(gridPos + offset, out var cell)
                    && cell.GridObject != null
                    && typesList.Contains(cell.GridObject.Type))
                    return true;

            return false;
        }


        private void SetAvailableToBuildPlaces(GridObject obj)
        {
            foreach (var place in _activePlaces)
                place.SetActive(false);

            _unactivePlaces.AddRange(_activePlaces);
            _activePlaces.Clear();

            if (obj == null)
                return;

            foreach (var field in WorldGrid.Instance.GridSize.allPositionsWithin)
            {
                if (CanBuildObjectAt(field, obj))
                {
                    var place = GetUnusedPlace();
                    place.transform.position = WorldGrid.GetWorldPos(field) + new Vector3(WorldGrid.CELL_SIZE / 2f, 0, WorldGrid.CELL_SIZE / 2f);
                    place.SetActive(true);
                    _activePlaces.Add(place);
                }
            }

            GameObject GetUnusedPlace()
            {
                GameObject place;
                if (_unactivePlaces.Count > 0)
                {
                    place = _unactivePlaces[0];
                    _unactivePlaces.RemoveAt(0);
                    return place;
                }

                place = Instantiate(_placePattern, _placesParent);
                return place;
            }
        }

        //Dafuq is this?
        private IEnumerator ConstructionUpdate()
        {
            WorldGrid grid = WorldGrid.Instance;
            while (_objectVisualization != null)
            {
                var gridPos = GetMouseGridPosition();

                // update visualizatio position
                if (IsInGrid(gridPos))
                {
                    _objectVisualization.transform.position = WorldGrid.GetWorldPos(gridPos);
                    _objectVisualization.gameObject.SetActive(true);
                }
                else
                {
                    _objectVisualization.gameObject.SetActive(false);
                }
                
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
            Ray ray = Camera.main.ScreenPointToRay(InputManager.MousePosition);
            if (!Physics.Raycast(ray, out RaycastHit hitData, 1000, _colisionLayers))
                return -Vector2Int.one;
            var worldPos = hitData.point;
            return WorldGrid.GetGridPos(worldPos);
        }
    }
}