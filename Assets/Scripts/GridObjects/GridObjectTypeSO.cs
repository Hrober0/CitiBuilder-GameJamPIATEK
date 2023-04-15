using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GridObjects
{
    [CreateAssetMenu(fileName ="NewGridObjectType", menuName ="Scriptables/GridObjectType")]
    public class GridObjectTypeSO : ScriptableObject
    {
        [SerializeField] private string _name = "default";
        public string DisplayedName => _name;
    }
}