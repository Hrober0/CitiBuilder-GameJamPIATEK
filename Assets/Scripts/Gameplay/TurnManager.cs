using GridObjects;
using InputControll;
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

    [SerializeField]
    private ConstructionController constructionController;

    [SerializeField]
    private BucketRandom<GridObject> objectsRandomiser;

    public void EndTurn()
    {
        TurnPasses?.Invoke();
        constructionController.SetObject(objectsRandomiser.GetRandom());
    }

    private void Start()
    {
        TurnCost.Init(this, Grids.WorldGrid.Instance);
        constructionController.OnGridObjectSelected += (obj) =>
        {
            if (obj == null)
            {
                EndTurn();
            }
        };
    }
}
