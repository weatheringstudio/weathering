
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
    [BindTerrainType(TerrainType.Any)]
    public class RoadForFluid : AbstractRoad
    {
        private const string pipe = "Pipe";
        protected override string SpriteKeyRoadBase => pipe;
        public override long LinkQuantityRestriction => 100;

        public override Type LinkTypeRestriction => typeof(TransportableFluid);
    }
}


