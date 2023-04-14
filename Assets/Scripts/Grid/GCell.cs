using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GridObjects;

namespace WordGrid
{
    public class GCell
    {
        public GridObject GridObject { get; set; }

        public float Heat { get; set; }
    }
}