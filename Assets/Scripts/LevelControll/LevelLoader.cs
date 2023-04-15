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
        [SerializeField] private ConstructionController _constructionController;

        [Header("Level start")]
        [SerializeField] private Vector2Int _roadPosition;
        [SerializeField] private GridObject _roadToSpawn;


        public override void InitSystem()
        {
            _constructionController.BuildObject(_roadPosition, _roadToSpawn, false);
        }
        public override void DeinitSystem() { }
    }

}