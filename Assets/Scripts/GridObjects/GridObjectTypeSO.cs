using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GridObjects
{
    [CreateAssetMenu(fileName ="NewGridObject", menuName ="Scriptables/GridObject")]
    public class GridObjectTypeSO : ScriptableObject
    {
        [SerializeField] private string _name = "default";
        public string DisplayedName => _name;
    }
}