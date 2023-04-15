using Mono.Cecil;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Util;

public class TurnManager : MonoBehaviour
{
    public event Action TurnPasses;

    [SerializeField]
    public TurnCostManager TurnCost;

    public void EndTurn()
    {
        TurnPasses?.Invoke();
    }

    private void Start()
    {
        TurnCost.Init(this, Grids.WorldGrid.Instance);
    }
}
