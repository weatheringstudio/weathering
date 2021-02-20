
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
    public class TutorialMap : StandardMap
    {
        public override int Width => 32;

        public override int Height => 32;

        public override Type GenerateTileType(Vector2Int pos) {
            throw new NotImplementedException();
        }
    }
}

