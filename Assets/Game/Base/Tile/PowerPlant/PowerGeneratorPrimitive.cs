

using System;

namespace Weathering
{
    [ConstructionCostBase(typeof(MachinePrimitive), 100)]
    public class PowerGeneraterPrimitive : AbstractFactoryStatic
    {
        public override string SpriteKey => DecoratedSpriteKey(typeof(PowerPlant).Name);

        protected override (Type, long) In_0 => (typeof(FuelSupply), 2);

        protected override (Type, long) In_0_Inventory => (typeof(Worker), 1);
        protected override (Type, long) Out0_Inventory => (typeof(ElectricitySupply), 10);
    }
}
