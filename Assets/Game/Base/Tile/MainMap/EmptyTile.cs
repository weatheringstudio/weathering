
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
    public class EmptyTile : StandardTile
    {
        public override string SpriteKey => typeof(EmptyTile).Name;

        public override void OnConstruct() {
        }

        public override void OnTap() {
        }
    }
}

