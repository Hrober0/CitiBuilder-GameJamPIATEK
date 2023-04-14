using GridObjects;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using WordGrid;

public class HeatManager : MonoBehaviour
{
    public static HeatManager Instance { get; private set; }

    private HashSet<HeatGenerator> heatGenerators;

    [SerializeField]
    private float[] kernel;

    public void GenerateHeat()
    {
        foreach (var generator in heatGenerators)
        {
            foreach (var tile in generator.GridObject.OccupiedTiles)
            {
                tile.Heat = Mathf.Max(tile.Heat + generator.HeatGeneration);
            }
        }
    }

    public void PropagateHeat()
    {
        ApplyKernel((x, y, i) => GetHeatAt(x + i, y));
        ApplyKernel((x, y, i) => GetHeatAt(x, y + i));
    }

    public void RegisterGenerator(HeatGenerator generator)
    {
        heatGenerators.Add(generator);
    }

    public void RemoveGenerator(HeatGenerator generator)
    {
        heatGenerators.Remove(generator);
    }

    private IEnumerator Start()
    {
        Assert.IsNull(Instance, $"Mulitple instances {nameof(HeatManager)}");

        heatGenerators = new HashSet<HeatGenerator>();

        SetupKernel();

        Instance = this;
        GenerateHeat();

        while (true)
        {
            yield return new WaitForSeconds(2.5f);
            PropagateHeat();
            yield return new WaitForSeconds(2.5f);
            GenerateHeat();
        }
    }

    private void SetupKernel()
    {
        var k = kernel;
        int k2 = k.Length - 1;
        kernel = new float[2 * k2 + 1];
        float sum = 0;
        for (int i=0; i < k2 + 1; i++)
        {
            sum += kernel[k2 + i] = k[i];
            sum += kernel[k2 - i] = k[i];
        }

        sum -= k[0];

        Debug.Log(sum);
        
        float sum2 = 0;

        for (int i = 0; i < kernel.Length; i++) 
        {
            kernel[i] /= sum;
            sum2 += kernel[i];
        }

        Debug.Log(sum2);
    }

    private void OnDrawGizmosSelected()
    {
        var grid = WorldGrid.Instance;
        if(grid == null)
        {
            return;
        }
        var c = Gizmos.color;
        foreach (var pos in grid.GridSize.allPositionsWithin)
        {
            var color = new Color(
                Mathf.Sin(grid.GetCell(pos).Heat) / 2f + 0.5f,
                0f,
                Mathf.Cos(grid.GetCell(pos).Heat) / 2f + 0.5f
                );
            Gizmos.color = color;
            Gizmos.DrawCube(new Vector3(pos.x, 0, pos.y), Vector3.one);
        }
        Gizmos.color = c;
    }

    private float GetHeatAt(int x, int y)
    {
        var grid = WorldGrid.Instance;
        if (x < 0 || x >= grid.GridX || y < 0 || y >= grid.GridY) {
            return 0f;
        }
        return grid.GetCell(x, y).Heat;
    }

    private void ApplyKernel(Func<int, int, int, float> heatGetter)
    {
        var grid = WorldGrid.Instance;
        float[,] newHeat = new float[grid.GridX, grid.GridY];

        int k2 = kernel.Length / 2;

        //Suboptimal code below
        for (int x = 0; x < grid.GridX; x++)
            for (int y = 0; y < grid.GridY; y++)
            {
                for (int i = 0; i < kernel.Length; i++)
                {
                    newHeat[x, y] += kernel[i] * heatGetter(x, y, i);
                }
            }

        for (int x = 0; x < grid.GridX; x++)
            for (int y = 0; y < grid.GridY; y++)
            {
                grid.GetCell(x, y).Heat = newHeat[x, y];
                newHeat[x, y] = 0;
            }
    }
}
