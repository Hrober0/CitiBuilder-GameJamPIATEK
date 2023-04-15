using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace GameSystems
{
    public abstract class GameSystem : MonoBehaviour
    {
        protected SystemsManager _systems;
        public bool IsInit => _systems != null;

        public void Init(SystemsManager systems)
        {
            Assert.IsNull(_systems, $"{name} system was already init");

            _systems = systems;
            InitSystem();
        }
        public void Deinit()
        {
            Assert.IsNotNull(_systems, $"{name} system was already deinit");
            DeinitSystem();
            _systems = null;
        }

        protected abstract void InitSystem();
        protected abstract void DeinitSystem();
    }
}