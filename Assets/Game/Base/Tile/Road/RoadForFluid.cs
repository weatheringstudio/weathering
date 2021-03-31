
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
    [BindTerrainType(typeof(TerrainType_Plain))]
    public class RoadForFluid : AbstractRoad, IWalkingTimeModifier
    {
        public override float WalkingTimeModifier => 1;

        private const string pipe = "Pipe";
        protected override string SpriteKeyRoadBase => pipe;
        public override long LinkQuantityRestriction => 100;

        public override Type LinkTypeRestriction => typeof(TransportableFluid);
    }
}


