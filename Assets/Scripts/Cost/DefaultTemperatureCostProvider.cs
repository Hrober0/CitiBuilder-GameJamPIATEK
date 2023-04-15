using GridObjects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultTemperatureCostProvider : MonoBehaviour, ICostProvider
{
    public float CurrentCost
    {
        get
        {
            //https://www.desmos.com/calculator/mogrqfowxc?lang=pl
            var heat = obj.Heat - offset;
            heat = Mathf.Sign(heat) * Mathf.Pow(Mathf.Abs(heat), power);
            return Mathf.Max(heat * coef, power, min);
        }
    }

    [SerializeField]
    public float min = 0f;

    [SerializeField]
    public float coef = 1f;
    
    [SerializeField]
    public float offset = 0f;

    [SerializeField]
    public float power = 1f;

    [SerializeField]
    private ObjectHeatProvider obj;
}