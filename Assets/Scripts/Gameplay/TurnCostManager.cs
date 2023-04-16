using Grids;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Util;

namespace GameSystems
{
    [Serializable]
    public class TurnCostManager
    {
        public event Action PointEnd;

        public float CurrentPoints => (float)points.Time;

        [SerializeField]
        public float DefaultTurnCost = 0;

        [SerializeField]
        private TimeMachine points;

        public void DepositPoints(float amount)
        {
            points.Accumulate(amount);
        }

        public float NextTurnCost(WorldGrid grid)
        {
            var cost = DefaultTurnCost;

            HashSet<GridObjects.GridObject> calculated = new HashSet<GridObjects.GridObject>();

            foreach (var cellPos in grid.GridSize.allPositionsWithin)
            {
                var cell = grid.GetCell(cellPos);

                if (cell.GridObject != null)
                {
                    if (calculated.Contains(cell.GridObject))
                    {
                        continue;
                    }
                    calculated.Add(cell.GridObject);
                    foreach (var costProvider in cell.GridObject.GetComponents<ICostProvider>())
                    {
                        var c = costProvider.CurrentCost;
                        Debug.Log(c);
                        cost += c;
                    }
                }
            }
            return Mathf.Max(cost, 0);
        }

        public void HandlePointConsumption(WorldGrid grid)
        {
            var cost = NextTurnCost(grid);
            Debug.Log(cost);
            if (!points.TryRetrieve(cost))
            {
                //TODO Handle game end
            }
        }

        public void Init(TurnManager manager, WorldGrid grid)
        {
            //manager.TurnPasses += () => HandlePointConsumption(grid);
        }
    }
}