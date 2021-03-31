
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
    [BindTerrainType(typeof(TerrainType_Any))]
    public class RoadForTransportable : AbstractRoad
    {
        public override long LinkQuantityRestriction => 100;

        public override Type LinkTypeRestriction => typeof(Transportable);
    }
}

