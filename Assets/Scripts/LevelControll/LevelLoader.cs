using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InputControll;
using GridObjects;
using GameSystems;
using UnityEngine.SceneManagement;

namespace LevelControll
{
    public class LevelLoader : GameSystem
    {
        [Header("Level start")]
        [SerializeField] private Vector2Int _roadPosition;
        [SerializeField] private GridObject _roadToSpawn;
        [SerializeField] private string _uiScene;


        private ConstructionController _constructionController;


        protected override void InitSystem()
        {
            _constructionController = _systems.Get<ConstructionController>();
            _constructionController.BuildObject(_roadPosition, _roadToSpawn, false);

            SceneManager.LoadSceneAsync(_uiScene, LoadSceneMode.Additive);
        }
        protected override void DeinitSystem() { }
    }

}