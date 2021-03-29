

using System;

namespace Weathering
{
    [ConstructionCostBase(typeof(MachinePrimitive), 100)]
    public class FactoryOfIronSmelting : AbstractFactoryStatic
    {
        public override string SpriteKey => DecoratedSpriteKey(typeof(FactoryOfMetalSmelting).Name);

        protected override (Type, long) In_0_Inventory => (typeof(Worker), 1);
        protected override (Type, long) In_1_Inventory => (typeof(ElectricitySupply), 1);

        protected override (Type, long) Out0 => (typeof(IronIngotSupply), 3);

        protected override (Type, long) In_0 => (typeof(IronOreSupply), 5);

    }
}
