using Grids;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GridObjects
{
    public class GridFitter : GridObjectModule
    {
        [SerializeField] private GridObjectTypeSO _type;

        [Flags]
        public enum Dir
        {
            Up =    1 << 0,
            Down =  1 << 1,
            Right = 1 << 2,
            Left =  1 << 3,
        }

        [SerializeField] private ObjDir[] _dirs;


        [Serializable]
        private struct ObjDir
        {
            public GameObject obj;
            public Dir dir;
            public int rotationOffset;

            public static bool operator ==(ObjDir s1, ObjDir s2)
            {
                return s1.Equals(s2);
            }

            public static bool operator !=(ObjDir s1, ObjDir s2)
            {
                return !s1.Equals(s2);
            }
        }


        private ObjDir? _curentState = null;

        public override void OnBuildingConstructed()
        {
            base.OnBuildingConstructed();

            UpdateShape();
        }

        public void UpdateShape()
        {
            if (GridObject.Fields.Count != 1)
            {
                Debug.LogWarning(GridObject.Fields.Count);
                return;
            }

            var grid = GameSystems.SystemsManager.Instance.Get<WorldGrid>();
            Vector2Int gridPos = GridObject.Fields[0] + WorldGrid.GetGridPos(transform.position);

            int mask = IsOnOffset(Vector2Int.up) * (int)Dir.Up
                + IsOnOffset(Vector2Int.down)    * (int)Dir.Down
                + IsOnOffset(Vector2Int.right)   * (int)Dir.Right
                + IsOnOffset(Vector2Int.left)    * (int)Dir.Left;

            int maxMaskValue = 1 << 4 - 1;
            if (mask == maxMaskValue)
                mask = -1;
            Dir dir = (Dir)mask;


            ObjDir? selectedObj = null;
            foreach (var item in _dirs)
            {
                if(item.dir == dir)
                {
                    selectedObj = item;
                    break;
                }
            }

            if (selectedObj == null)
            {
                Debug.LogError("Missing type");
                return;
            }

            if (_curentState == selectedObj)
                return;

            foreach (var item in _dirs)
                item.obj.SetActive(false);

            selectedObj.Value.obj.SetActive(true);
            selectedObj.Value.obj.transform.rotation = Quaternion.Euler(0, selectedObj.Value.rotationOffset, 0);

            _curentState = selectedObj;

            TryUpdate(Vector2Int.up);
            TryUpdate(Vector2Int.down);
            TryUpdate(Vector2Int.left);
            TryUpdate(Vector2Int.right);


            int IsOnOffset(Vector2Int offset) => IsTypeOnField(grid, gridPos + offset) ? 1 : 0;
            void TryUpdate(Vector2Int offset)
            {
                if (grid.TryGetCell(gridPos + offset, out var cell) && cell.GridObject != null && cell.GridObject.TryGetComponent(out GridFitter gridFitter))
                    gridFitter.UpdateShape();
            }
        }

        private bool IsTypeOnField(WorldGrid grid, Vector2Int gridPos) => grid.TryGetCell(gridPos, out var cell) && cell.GridObject != null && cell.GridObject.Type == _type;
    }
}