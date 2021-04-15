

using System;

namespace Weathering
{

    // 预制体
    [Depend(typeof(DiscardableSolid))]
    public class BuildingPrefabrication { }


    [ConstructionCostBase(typeof(MachinePrimitive), 100)]
    public class FactoryOfBuildingPrefabrication : AbstractFactoryStatic
    {
        public override string SpriteKey => DecoratedSpriteKey(typeof(FactoryOfMetalSmelting).Name);

        protected override (Type, long) In_0_Inventory => (typeof(Worker), 1);
        protected override (Type, long) Out0 => (typeof(BuildingPrefabrication), 1);
        protected override (Type, long) In_0 => (typeof(SteelIngot), 1);
        protected override (Type, long) In_1 => (typeof(ConcretePowder), 2);
    }
}
