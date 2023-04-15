using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GridObjects;
using Grids;
using System;
using GameSystems;

namespace InputControll
{
    public class ConstructionController : GameSystem
    {
        private class PlacesPool
        {
            private readonly GameObject _placePattern;
            private readonly Transform _placesParent;

            private readonly Dictionary<Vector2Int, GameObject> _activePlaces = new();
            private readonly List<GameObject> _unactivePlaces = new();


            public PlacesPool(GameObject placePattern, Transform placesParent)
            {
                _placePattern = placePattern;
                _placesParent = placesParent;
            }


            public void Set(Vector2Int field)
            {
                if (_activePlaces.ContainsKey(field))
                    return;

                GameObject place;
                if (_unactivePlaces.Count > 0)
                {
                    place = _unactivePlaces[0];
                    _unactivePlaces.RemoveAt(0);
                }
                else
                {
                    place = Instantiate(_placePattern, _placesParent);
                }

                place.transform.position = WorldGrid.GetWorldPos(field) + new Vector3(WorldGrid.CELL_SIZE / 2f, 0, WorldGrid.CELL_SIZE / 2f);

                _activePlaces.Add(field, place);
                place.SetActive(true);
            }
            public void Hide(Vector2Int field)
            {
                if (!_activePlaces.TryGetValue(field, out var place))
                    return;

                place.SetActive(false);
                _unactivePlaces.Add(place);
                _activePlaces.Remove(field);
            }
            public void HideAll()
            {
                foreach (var item in _activePlaces)
                {
                    item.Value.SetActive(false);
                    _unactivePlaces.Add(item.Value);
                }
                _activePlaces.Clear();
            }
        }

        [SerializeField] private Camera _camera;
        [SerializeField] private LayerMask _colisionLayers;

        [Header("construction")]
        [SerializeField] private Transform _objectsParent;
        [SerializeField] private Transform _objectVisualizationParent;

        [Header("places")]
        [SerializeField] private Transform _placesParent;
        [SerializeField] private GameObject _placePattern;
        [SerializeField] private GameObject _placeIncorrectPattern;


        private GridObject _objectVisualization = null;
        private Coroutine _constructionUpdater = null;


        private PlacesPool _places;
        private PlacesPool _placesIncorrect;


        public event Action<GridObject> OnBuildingBuild;
        public event Action<GridObject> OnBuildingSelected;

        private Vector2Int? _lastUpdatePos;

        protected override void InitSystem()
        {
            _places = new(_placePattern, _placesParent);
            _placesIncorrect = new(_placeIncorrectPattern, _placesParent);
        }
        protected override void DeinitSystem() { }


        public void SetObject(GridObject selectedObject)
        {
            if (_objectVisualization != null)
            {
                Destroy(_objectVisualization.gameObject);
                _objectVisualization = null;

                InputManager.PrimaryAction.Ended -= TryBuild;
            }

            if (selectedObject != null)
            {
                _objectVisualization = Instantiate(selectedObject, _objectVisualizationParent);
                _objectVisualization.name = selectedObject.name;

                _lastUpdatePos = null;

                if (_constructionUpdater == null)
                    _constructionUpdater = StartCoroutine(ConstructionVisualizationUpdate());

                InputManager.PrimaryAction.Ended += TryBuild;
            }

            OnBuildingSelected?.Invoke(selectedObject);
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

            OnBuildingBuild?.Invoke(newObj);
        }
        
        public bool CanBuildObjectAt(Vector2Int gridPos, GridObject obj)
        {
            bool hasReqiredBuilding = false;
            foreach (var field in obj.Fields)
            {
                var currField = field + gridPos;

                if (!WorldGrid.Instance.TryGetCell(currField, out var cell))
                    return false;

                if (cell.GridObject != null)
                    return false;

                if (!hasReqiredBuilding)
                    hasReqiredBuilding = HasAtLeastOneBuild(currField, obj.ReqiredObjects);
            }

            return hasReqiredBuilding;
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


        private void UpdateAvailableToBuildPlaces(GridObject obj, Vector2Int buildingPos)
        {
            if (_lastUpdatePos.HasValue && _lastUpdatePos.Value == buildingPos)
                return;

            _lastUpdatePos = buildingPos;

            _places.HideAll();
            _placesIncorrect.HideAll();

            if (obj == null)
                return;


            List<Vector2Int> incorrectFields = new();
            if (!CanBuildObjectAt(buildingPos, obj))
            {
                foreach (var f in obj.Fields)
                {
                    var field = buildingPos + f;
                    incorrectFields.Add(field);
                    _placesIncorrect.Set(field);
                }
            }
            

            foreach (var checkedField in WorldGrid.Instance.GridSize.allPositionsWithin)
            {
                if (CanBuildObjectAt(checkedField, obj))
                {
                    foreach (var objField in obj.Fields)
                    {
                        var currField = checkedField + objField;
                        if (!incorrectFields.Contains(currField))
                            _places.Set(currField);
                    }
                }
            }
        }


        private void TryBuild()
        {
            var gridPos = GetMouseGridPosition();
            if (CanBuildObjectAt(gridPos, _objectVisualization))
            {
                var buildgToBuild = _objectVisualization;
                SetObject(null);
                BuildObject(gridPos, buildgToBuild);
            }
            else
            {
                Debug.Log("Nope");
            }
        }

        private IEnumerator ConstructionVisualizationUpdate()
        {
            WorldGrid grid = WorldGrid.Instance;
            while (_objectVisualization != null)
            {
                var gridPos = GetMouseGridPosition();

                _objectVisualization.transform.position = WorldGrid.GetWorldPos(gridPos);

                UpdateAvailableToBuildPlaces(_objectVisualization, gridPos);

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