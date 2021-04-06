

using System;

namespace Weathering
{
    public class SpaceElevator : AbstractTransportStation
    {

        protected override bool ToUniverse => true;
        public override string SpriteKeyRoad => typeof(SpaceElevator).Name;
        protected override long Capacity => 1;

        protected override long CostQuantity => 100;

        protected override Type CostType => typeof(ElectricitySupply);
    }
}
