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

            foreach (var cellPos in grid.GridSize.allPositionsWithin)
            {
                var cell = grid.GetCell(cellPos);

                if (cell.GridObject != null)
                {
                    foreach (var costProvider in cell.GridObject.GetComponents<ICostProvider>())
                    {
                        cost += costProvider.CurrentCost;
                    }
                }
            }
            return Mathf.Max(cost, 0);
        }

        public void HandlePointConsumption(WorldGrid grid)
        {
            var cost = NextTurnCost(grid);
            if (!points.TryRetrieve(cost))
            {
                //TODO Handle game end
            }
        }

        public void Init(TurnManager manager, WorldGrid grid)
        {
            manager.TurnPasses += () => HandlePointConsumption(grid);
        }
    }
}