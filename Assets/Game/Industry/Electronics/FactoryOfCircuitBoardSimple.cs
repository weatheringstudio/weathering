

using System;

namespace Weathering
{
    // 简单电路板
    [ConceptSupply(typeof(CircuitBoardSimpleSupply))]
    [ConceptDescription(typeof(CircuitBoardSimpleDescription))]
    [Depend(typeof(DiscardableSolid))]
    [Concept]
    public class CircuitBoardSimple { }

    [ConceptResource(typeof(CircuitBoardIntegrated))]
    [Depend(typeof(TransportableSolid))]
    [Concept]
    public class CircuitBoardSimpleSupply { }
    [Concept]
    public class CircuitBoardSimpleDescription { }

    [ConstructionCostBase(typeof(BuildingPrefabrication), 100)]
    public class FactoryOfCircuitBoardSimple : AbstractFactoryStatic
    {
        public override string SpriteKey => DecoratedSpriteKey(typeof(FactoryOfMetalSmelting).Name);

        protected override (Type, long) In_0_Inventory => (typeof(Worker), 1);
        protected override (Type, long) In_1_Inventory => (typeof(ElectricitySupply), 10);

        protected override (Type, long) Out0 => (typeof(CircuitBoardSimpleSupply), 1);

        protected override (Type, long) In_0 => (typeof(CopperWire), 2);
        protected override (Type, long) In_1 => (typeof(WoodPlankSupply), 1);
    }
}
