

using System;

namespace Weathering
{

    // 预制体
    [ConceptSupply(typeof(BuildingPrefabricationSupply))]
    [ConceptDescription(typeof(BuildingPrefabricationDescription))]
    [Depend(typeof(DiscardableSolid))]
    [Concept]
    public class BuildingPrefabrication { }

    [ConceptResource(typeof(BuildingPrefabrication))]
    [Depend(typeof(TransportableSolid))]
    [Concept]
    public class BuildingPrefabricationSupply { }

    [Concept]
    public class BuildingPrefabricationDescription { }

    [ConstructionCostBase(typeof(MachinePrimitive), 100)]
    public class FactoryOfBuildingPrefabrication : AbstractFactoryStatic
    {
        public override string SpriteKey => DecoratedSpriteKey(typeof(FactoryOfMetalSmelting).Name);

        protected override (Type, long) In_0_Inventory => (typeof(Worker), 1);
        protected override (Type, long) Out0 => (typeof(BuildingPrefabricationSupply), 1);
        protected override (Type, long) In_0 => (typeof(SteelIngotSupply), 1);
        protected override (Type, long) In_1 => (typeof(ConcretePowderSupply), 2);
    }
}
