

using System;

namespace Weathering
{
    [ConstructionCostBase(typeof(LightMaterial), 10000)]
    public class SpaceElevatorDest : AbstractTransportStationDest
    {

        protected override bool FromUniverse => true;
        public override string SpriteKeyRoad => typeof(SpaceElevator).Name;
        protected override long Capacity => 1;

    }
}

