using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InputControll;
using GridObjects;

namespace LevelControll
{
    public class LevelLoader : MonoBehaviour
    {
        [SerializeField] private ConstructionController _constructionController;

        [Header("Level start")]
        [SerializeField] private Vector2Int _roadPosition;
        [SerializeField] private GridObject _roadToSpawn;


        private void Start()
        {
            _constructionController.BuildObject(_roadPosition, _roadToSpawn);

            //_constructionController.SetObject()
        }
    }

}