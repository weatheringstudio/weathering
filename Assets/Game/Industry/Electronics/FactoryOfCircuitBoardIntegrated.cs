

using System;

namespace Weathering
{
    // 集成电路板
    [ConceptSupply(typeof(CircuitBoardIntegratedSupply))]
    [ConceptDescription(typeof(CircuitBoardIntegratedDescription))]
    [Depend(typeof(DiscardableSolid))]
    [Concept]
    public class CircuitBoardIntegrated { }

    [ConceptResource(typeof(CircuitBoardIntegrated))]
    [Depend(typeof(TransportableSolid))]
    [Concept]
    public class CircuitBoardIntegratedSupply { }
    [Concept]
    public class CircuitBoardIntegratedDescription { }

    [ConstructionCostBase(typeof(BuildingPrefabrication), 100)]
    public class FactoryOfCircuitBoardIntegrated : AbstractFactoryStatic
    {
        public override string SpriteKey => DecoratedSpriteKey(typeof(FactoryOfMetalSmelting).Name);

        protected override (Type, long) In_0_Inventory => (typeof(Worker), 1);
        protected override (Type, long) In_1_Inventory => (typeof(ElectricitySupply), 50);

        protected override (Type, long) Out0 => (typeof(CircuitBoardIntegratedSupply), 1);

        protected override (Type, long) In_0 => (typeof(LightMaterialSupply), 1);
        protected override (Type, long) In_1 => (typeof(AluminiumIngotSupply), 1);
    }
}
