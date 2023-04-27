using GameSystems;
using GridObjects;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Grids;
using static UnityEngine.Rendering.DebugUI;

namespace HeatSimulation
{
    public class HeatGenerator : GridObjectModule
    {
        public float HeatGeneration => heatGeneration;

        public IEnumerable<GCell> HeatedCells
        {
            get
            {
                if (!isBaked)
                {
                    BakeZone();
                    isBaked = true;
                }
                var worldGrid = SystemsManager.Instance.Get<WorldGrid>();
                var gridPos = GridObject.GridPos;
                foreach (var tile in heatingZone)
                {
                    var cell = worldGrid.GetCell(tile + gridPos);

                    yield return cell;
                }
            }
        }

        public int ZoneSize
        {
            get
            {
                if (!isBaked)
                {
                    BakeZone();
                    isBaked = true;
                }
                return heatingZone.Count;
            }
        }

        [Tooltip("Total heat generation of this generator")]
        [SerializeField]
        private float heatGeneration;

        [SerializeField]
        private List<Vector2Int> heatingZone;

        private HeatManager _heatManager;

        private bool isBaked;

        private void BakeZone()
        {
            var zone = heatingZone;
            heatingZone = new List<Vector2Int>();
            heatingZone.AddRange(zone.Union(GridObject.Fields));
        }

        public override void OnBuildingConstructed()
        {
            base.OnBuildingConstructed();

            _heatManager = SystemsManager.Instance.Get<HeatManager>();
            _heatManager.RegisterGenerator(this);
        }

        public override void OnBuildingDestroyed()
        {
            base.OnBuildingDestroyed();

            if (_heatManager)
                _heatManager.RemoveGenerator(this);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            foreach (var field in heatingZone)
            {
                Vector3 pos = transform.position + new Vector3(field.x, 0, field.y) * WorldGrid.CELL_SIZE + Vector3.one / 2f;

                Gizmos.DrawCube(pos, Vector3.one * 0.5f);
            }
        }
    }
}