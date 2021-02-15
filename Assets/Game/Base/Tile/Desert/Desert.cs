
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
    public class Desert : StandardTile
    {
        public override string SpriteKey => typeof(Desert).Name;

        public override void OnTap() {
        }
    }
}

