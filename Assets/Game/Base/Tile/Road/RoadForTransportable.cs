
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
    [BindTerrainType(TerrainType.Any)]
    public class RoadForTransportable : AbstractRoad
    {
        public override long LinkQuantityRestriction => 100;

        public override Type Restriction => throw new NotImplementedException();

        public override Type LinkTypeRestriction => typeof(Transportable);
    }
}

