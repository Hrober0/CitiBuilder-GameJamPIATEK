using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

namespace GameSystems
{
    public class SystemsManager : MonoBehaviour
    {
        [SerializeField] private GameSystem[] systems;

        public static SystemsManager Instance { get; private set; }

        private void Start()
        {
            Assert.IsNull(Instance, $"Mulitple instances {nameof(SystemsManager)}");
            Instance = this;

            foreach (var system in systems)
                system.Init(this);
        }

        private void OnDestroy()
        {
            foreach (var system in systems.Reverse())
                system.Deinit();
        }

        public T Get<T>() where T : GameSystem
        {
            foreach (var sys in systems)
                if (sys is T tSys)
                {
                    if (tSys.IsInit)
                        return tSys;
                    else
                        Debug.LogError($"{tSys} system is not inited");
                }
                    
            return null;
        }
    }
}