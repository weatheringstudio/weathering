

using System;

namespace Weathering
{
    [ConstructionCostBase(typeof(MachinePrimitive), 100)]
    public class PowerGeneratorOfWood : AbstractFactoryStatic
    {
        public override string SpriteKey => DecoratedSpriteKey(typeof(PowerPlant).Name);

        protected override (Type, long) In_0 => (typeof(FuelSupply), 6);

        protected override (Type, long) In_0_Inventory => (typeof(Worker), 1);
        protected override (Type, long) Out0_Inventory => (typeof(ElectricitySupply), 5);
    }
}
