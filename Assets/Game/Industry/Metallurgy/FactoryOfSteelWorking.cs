

using System;

namespace Weathering
{
    public class FactoryOfMetalSmelting { }

    // 钢厂 混合
    [ConstructionCostBase(typeof(BuildingPrefabrication), 100)]
    public class FactoryOfSteelWorking : AbstractFactoryStatic
    {
        public override string SpriteKey => DecoratedSpriteKey(typeof(FactoryOfMetalSmelting).Name);

        protected override (Type, long) In_0_Inventory => (typeof(Worker), 1);
        protected override (Type, long) In_1_Inventory => (typeof(ElectricitySupply), 20);
        protected override (Type, long) Out0 => (typeof(SteelIngotSupply), 1);
        protected override (Type, long) In_0 => (typeof(IronIngotSupply), 3);
    }
}
