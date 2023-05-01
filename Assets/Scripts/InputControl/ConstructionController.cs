using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GridObjects;
using Grids;
using System;
using GameSystems;
using UnityEngine.EventSystems;

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


        [SerializeField] private LayerMask _colisionLayers;

        [Header("construction")]
        [SerializeField] private Transform _objectsParent;
        [SerializeField] private Transform _objectVisualizationParent;

        [Header("places")]
        [SerializeField] private Transform _placesParent;
        [SerializeField] private GameObject _placePattern;
        [SerializeField] private GameObject _placeIncorrectPattern;


        private GridObject _selectedObject = null;
        private GridObject _objectVisualization = null;
        private Coroutine _constructionUpdater = null;


        private PlacesPool _places;
        private PlacesPool _placesIncorrect;


        private WorldGrid _worldGrid;
        private InputManager _inputManager;


        public event Action<(GridObject placedBuilding, GridObject objectPattern)> OnBuildingBuild;

        public event Action<GridObject> OnBuildingSelected;
        public GridObject SelectedBuilding => _selectedObject;

        private Vector2Int? _lastUpdatePos;

        protected override void InitSystem()
        {
            _worldGrid = _systems.Get<WorldGrid>();

            _places = new(_placePattern, _placesParent);
            _placesIncorrect = new(_placeIncorrectPattern, _placesParent);

            _inputManager = _systems.Get<InputManager>();
        }
        protected override void DeinitSystem() { }


        public void SetObject(GridObject selectedObject)
        {
            _selectedObject = selectedObject;

            if (_objectVisualization != null)
            {
                Destroy(_objectVisualization.gameObject);
                _objectVisualization = null;

                UpdateAvailableToBuildPlaces(null, Vector2Int.zero);

                _inputManager.PrimaryAction.Ended -= TryBuild;
            }

            if (selectedObject != null)
            {
                _objectVisualization = Instantiate(selectedObject, _objectVisualizationParent);
                _objectVisualization.name = selectedObject.name;

                _lastUpdatePos = null;

                if (_constructionUpdater == null)
                    _constructionUpdater = StartCoroutine(ConstructionVisualizationUpdate());

                _inputManager.PrimaryAction.Ended += TryBuild;
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
                _worldGrid.GetCell(field + gridPos).GridObject = newObj;

            newObj.Place();

            OnBuildingBuild?.Invoke((newObj, objPattern));
        }
        
        public bool CanBuildObjectAt(Vector2Int gridPos, GridObject obj)
        {
            bool hasReqiredBuilding = false;
            foreach (var field in obj.Fields)
            {
                var currField = field + gridPos;

                if (!_worldGrid.TryGetCell(currField, out var cell))
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
                if (_worldGrid.TryGetCell(gridPos + offset, out var cell)
                    && cell.GridObject != null
                    && typesList.Contains(cell.GridObject.Type))
                    return true;

            return false;
        }


        private void UpdateAvailableToBuildPlaces(GridObject obj, Vector2Int buildingPos)
        {
            _places.HideAll();
            _placesIncorrect.HideAll();


            if (obj == null)
                return;

            List<Vector2Int> incorrectFields = new();
            if (!IsPointerOverUI && !CanBuildObjectAt(buildingPos, obj))
            {
                foreach (var f in obj.Fields)
                {
                    var field = buildingPos + f;
                    incorrectFields.Add(field);
                    _placesIncorrect.Set(field);
                }
            }
            

            foreach (var checkedField in _worldGrid.GridSize.allPositionsWithin)
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
            if (IsPointerOverUI)
                return;

            var gridPos = GetMouseGridPosition();
            if (CanBuildObjectAt(gridPos, _objectVisualization))
            {
                var buildgToBuild = _selectedObject;
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
            while (_objectVisualization != null)
            {
                var gridPos = GetMouseGridPosition();

                if (_lastUpdatePos == null || _lastUpdatePos.Value != gridPos)
                {
                    _lastUpdatePos = gridPos;

                    _objectVisualization.gameObject.SetActive(!IsPointerOverUI);
                    _objectVisualization.transform.position = WorldGrid.GetWorldPos(gridPos);

                    UpdateAvailableToBuildPlaces(_objectVisualization, gridPos);
                } 

                yield return null;
            }

            _constructionUpdater = null;
        }

        private Vector2Int GetMouseGridPosition()
        {
            Ray ray = Camera.main.ScreenPointToRay(_inputManager.MousePosition);
            if (!Physics.Raycast(ray, out RaycastHit hitData, 1000, _colisionLayers))
                return -Vector2Int.one;
            var worldPos = hitData.point;
            return WorldGrid.GetGridPos(worldPos);
        }

        private bool IsPointerOverUI => EventSystem.current.IsPointerOverGameObject();
    }
}