

using System;

namespace Weathering
{
    // 卫星组件
    [ConceptSupply(typeof(SatelliteComponentSupply))]
    [ConceptDescription(typeof(SatelliteComponentDescription))]
    [Depend(typeof(DiscardableSolid))]
    [Concept]
    public class SatelliteComponent { }

    [ConceptResource(typeof(SatelliteComponent))]
    [Depend(typeof(TransportableSolid))]
    [Concept]
    public class SatelliteComponentSupply { }
    [Concept]
    public class SatelliteComponentDescription { }

    [ConstructionCostBase(typeof(BuildingPrefabrication), 100)]
    public class FactoryOfSatelliteComponent : AbstractFactoryStatic
    {
        public override string SpriteKey => DecoratedSpriteKey(typeof(FactoryOfMetalSmelting).Name);

        protected override (Type, long) In_0_Inventory => (typeof(Worker), 1);
        protected override (Type, long) In_1_Inventory => (typeof(ElectricitySupply), 50);

        protected override (Type, long) In_0 => (typeof(LightMaterialSupply), 1);
        protected override (Type, long) In_1 => (typeof(CircuitBoardAdvancedSupply), 1);
    }
}
