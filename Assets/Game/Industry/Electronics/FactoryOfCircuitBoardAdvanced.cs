

using System;

namespace Weathering
{
    // 高级电路板
    [ConceptSupply(typeof(CircuitBoardAdvancedSupply))]
    [ConceptDescription(typeof(CircuitBoardAdvancedDescription))]
    [Depend(typeof(DiscardableSolid))]
    [Concept]
    public class CircuitBoardAdvanced { }

    [ConceptResource(typeof(CircuitBoardAdvanced))]
    [Depend(typeof(TransportableSolid))]
    [Concept]
    public class CircuitBoardAdvancedSupply { }
    [Concept]
    public class CircuitBoardAdvancedDescription { }

    [ConstructionCostBase(typeof(LightMaterial), 100)]
    public class FactoryOfCircuitBoardAdvanced : AbstractFactoryStatic
    {
        public override string SpriteKey => DecoratedSpriteKey(typeof(FactoryOfMetalSmelting).Name);

        protected override (Type, long) In_0_Inventory => (typeof(Worker), 1);
        protected override (Type, long) In_1_Inventory => (typeof(ElectricitySupply), 30);

        protected override (Type, long) Out0 => (typeof(CircuitBoardAdvancedSupply), 1);

        protected override (Type, long) In_0 => (typeof(CircuitBoardIntegratedSupply), 2);
        protected override (Type, long) In_1 => (typeof(LightMaterialSupply), 1);
    }
}
