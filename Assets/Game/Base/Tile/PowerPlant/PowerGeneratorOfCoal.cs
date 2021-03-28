

using System;

namespace Weathering
{
    [ConstructionCostBase(typeof(IronProduct), 300)]
    public class PowerGeneraterOfCoal : AbstractFactoryStatic
    {
        public override string SpriteKey => DecoratedSpriteKey(typeof(PowerPlant).Name);

        protected override (Type, long) In_0 => (typeof(CoalSupply), 3);

        protected override (Type, long) In_0_Inventory => (typeof(Worker), 1);
        protected override (Type, long) Out0_Inventory => (typeof(ElectricitySupply), 10);
    }
}
