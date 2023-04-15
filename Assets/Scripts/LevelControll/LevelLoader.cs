using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InputControll;
using GridObjects;
using GameSystems;

namespace LevelControll
{
    public class LevelLoader : GameSystem
    {
        [Header("Level start")]
        [SerializeField] private Vector2Int _roadPosition;
        [SerializeField] private GridObject _roadToSpawn;


        private ConstructionController _constructionController;


        protected override void InitSystem()
        {
            _constructionController = _systems.Get<ConstructionController>();
            _constructionController.BuildObject(_roadPosition, _roadToSpawn, false);
        }
        protected override void DeinitSystem() { }
    }

}