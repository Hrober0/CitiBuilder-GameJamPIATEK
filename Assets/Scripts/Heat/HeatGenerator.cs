using GridObjects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GridObject))]
public class HeatGenerator : MonoBehaviour
{
    public float HeatGeneration => heatGeneration;
    
    public GridObject GridObject { get; private set; }

    [SerializeField]
    private float heatGeneration;

    private void Start()
    {
        GridObject = GetComponent<GridObject>();
        HeatManager.Instance.RegisterGenerator(this);
    }

    private void OnDestroy()
    {
        HeatManager.Instance.RemoveGenerator(this);
    }
}
