using GridObjects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class HeatGenerator : GridObjectModule
{
   public float HeatGeneration => heatGeneration;
    
    

    [SerializeField]
    private float heatGeneration;


    public override void OnBuildingConstructed()
    {
        base.OnBuildingConstructed();

        HeatManager.Instance.RegisterGenerator(this);
    }

    public override void OnBuildingDestroyed()
    {
        base.OnBuildingDestroyed();

        HeatManager.Instance.RemoveGenerator(this);
    }
}
