using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameSystems
{
    public abstract class GameSystem : MonoBehaviour
    {
        public abstract void InitSystem();
        public abstract void DeinitSystem();
    }
}