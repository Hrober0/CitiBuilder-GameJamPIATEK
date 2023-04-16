using GridObjects;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using Grids;
using GameSystems;
using UnityEditor.Overlays;

namespace HeatSimulation
{
    public class HeatManager : GameSystem
    {
        public static HeatManager Instance { get; private set; }

        private HashSet<HeatGenerator> heatGenerators;

        [SerializeField]
        private MeshFilter overlay;

        [SerializeField]
        private float[] kernel;


        protected override void InitSystem()
        {
            Assert.IsNull(Instance, $"Mulitple instances {nameof(HeatManager)}");
            Instance = this;

            heatGenerators = new HashSet<HeatGenerator>();

            _systems.Get<TurnManager>().TurnPasses += UpdateHeat;

            SetupKernel();

            //StartCoroutine(_HeatPropagatr());
        }
        protected override void DeinitSystem() 
        {
            _systems.Get<TurnManager>().TurnPasses += UpdateHeat;
        }

        public void RegisterGenerator(HeatGenerator generator)
        {
            heatGenerators.Add(generator);
        }

        public void RemoveGenerator(HeatGenerator generator)
        {
            heatGenerators.Remove(generator);
        }

        private void UpdateHeat()
        {
            GenerateHeat();
            PropagateHeat();
            GenerateOverlay();
        }

        private void GenerateHeat()
        {
            foreach (var generator in heatGenerators)
            {
                foreach (var tile in generator.GridObject.OccupiedCells)
                {
                    tile.Heat = Mathf.Max(tile.Heat + generator.HeatGeneration, 0);
                }
            }
        }

        private void PropagateHeat()
        {
            ApplyKernel((x, y, i) => GetHeatAt(x + i, y));
            ApplyKernel((x, y, i) => GetHeatAt(x, y + i));
        }

        private void GenerateOverlay()
        {
            var grid = WorldGrid.Instance;

            var sizeX = grid.GridX + 1;
            var sizeY = grid.GridX + 1;

            Mesh mesh = new Mesh();

            var verts = new Vector3[sizeX * sizeY * 4];
            var tris = new int[sizeX * sizeY * 6];
            var uv0 = new Vector4[sizeX * sizeY * 4];
            var uv1 = new Vector4[sizeX * sizeY * 4];

            for (int x = 0; x < sizeX; x++)
                for (int y = 0; y < sizeX; y++)
                {
                    int i = sizeX * y + x;
                    int v = i * 4;
                    int t = i * 6;
                    verts[v + 0] = new Vector3(x - 0.5f, 0f, y - 0.5f);
                    verts[v + 1] = new Vector3(x + 0.5f, 0f, y - 0.5f);
                    verts[v + 2] = new Vector3(x + 0.5f, 0f, y + 0.5f);
                    verts[v + 3] = new Vector3(x - 0.5f, 0f, y + 0.5f);

                    uv0[v + 0] = new Vector2(0, 0);
                    uv0[v + 1] = new Vector2(1, 0);
                    uv0[v + 2] = new Vector2(1, 1);
                    uv0[v + 3] = new Vector2(0, 1);

                    var weights = new Vector4(
                        GetHeatAt(x, y),
                        GetHeatAt(x, y - 1),
                        GetHeatAt(x - 1, y - 1),
                        GetHeatAt(x - 1, y)
                        );

                    uv1[v + 0] = weights;
                    uv1[v + 1] = weights;
                    uv1[v + 2] = weights;
                    uv1[v + 3] = weights;

                    tris[t + 0] = v + 0;
                    tris[t + 1] = v + 2;
                    tris[t + 2] = v + 1;
                    tris[t + 3] = v + 0;
                    tris[t + 4] = v + 3;
                    tris[t + 5] = v + 2;
                }

            mesh.SetVertices(verts);
            mesh.SetUVs(0, uv0);
            mesh.SetUVs(1, uv1);

            mesh.SetTriangles(tris, 0);

            overlay.mesh = mesh;
        }

        private void SetupKernel()
        {
            var k = kernel;
            int k2 = k.Length - 1;
            kernel = new float[2 * k2 + 1];
            float sum = 0;
            for (int i = 0; i < k2 + 1; i++)
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

        private float GetHeatAt(int x, int y)
        {
            var grid = WorldGrid.Instance;
            if (x < 0 || x >= grid.GridX || y < 0 || y >= grid.GridY)
            {
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
                        var v = heatGetter(x, y, i - k2);
                        newHeat[x, y] += kernel[i] * v;
                    }
                }

            for (int x = 0; x < grid.GridX; x++)
                for (int y = 0; y < grid.GridY; y++)
                {
                    grid.GetCell(x, y).Heat = newHeat[x, y];
                }
        }


#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            var grid = WorldGrid.Instance;
            if (grid == null)
            {
                return;
            }
            var c = Gizmos.color;
            foreach (var pos in grid.GridSize.allPositionsWithin)
            {
                var h = grid.GetCell(pos).Heat;
                //h *= h;
                var color = new Color(
                    -Mathf.Cos(h) / 2f + 0.5f,
                    0f,
                    Mathf.Sin(h) / 2f + 0.5f
                    );
                Gizmos.color = color;
                Gizmos.DrawCube(new Vector3(pos.x + 0.5f, 0, pos.y + 0.5f), Vector3.one * 0.9f);
            }
            Gizmos.color = c;
        }
#endif
    }
}