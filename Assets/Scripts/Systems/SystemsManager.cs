using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameSystems
{
    public class SystemsManager : MonoBehaviour
    {
        [SerializeField] private GameSystem[] systems;

        void Start()
        {
            foreach (var system in systems)
                system.InitSystem();
        }

        private void OnDestroy()
        {
            foreach (var system in systems)
                system.DeinitSystem();
        }
    }
}