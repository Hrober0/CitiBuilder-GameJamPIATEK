using GridObjects;
using System;
using System.Linq;
using UnityEngine;

public class ObjectHeatProvider : MonoBehaviour
{
    public float Heat
    {
        get
        {
            var fields = obj.OccupiedCells.ToArray();
            var heats = new float[fields.Length];

            for (int i = 0; i < fields.Length; i++)
            {
                heats[i] = fields[i].Heat;
            }

            //Jam code
            try
            {
                return heatFunction switch
                {
                    HeatValueFunction.Min => Mathf.Min(heats),
                    HeatValueFunction.Max => Mathf.Max(heats),
                    HeatValueFunction.Mean => heats.Average(),
                    _ => 0f
                };
            }
            catch (InvalidOperationException e)
            {
                Debug.Log(e.Message);
                return 0;
            }
        }
    }

    [SerializeField]
    private GridObject obj;
    
    [SerializeField]
    private HeatValueFunction heatFunction;

    public enum HeatValueFunction
    {
        Min,
        Max,
        Mean
    }
}
