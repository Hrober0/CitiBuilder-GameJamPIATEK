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
                system.Init(this);
        }

        private void OnDestroy()
        {
            foreach (var system in systems)
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