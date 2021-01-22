
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
    [Depend(typeof(Farm))]
    [Concept]
    public class Plantation : StandardTile
    {
        public override string SpriteKey => typeof(Farm).Name;

        public override void OnConstruct() {
        }

        public override void OnDestruct() {
        }

        public override void OnTap() {

        }
    }
}

