

using System;

namespace Weathering
{
    // 简单电路板
    [Depend(typeof(DiscardableSolid))]
    public class CircuitBoardSimple { }

    [ConstructionCostBase(typeof(BuildingPrefabrication), 100)]
    public class FactoryOfCircuitBoardSimple : AbstractFactoryStatic
    {
        public override string SpriteKey => DecoratedSpriteKey(typeof(FactoryOfMetalSmelting).Name);

        protected override (Type, long) In_0_Inventory => (typeof(Worker), 1);
        protected override (Type, long) In_1_Inventory => (typeof(Electricity), 10);

        protected override (Type, long) Out0 => (typeof(CircuitBoardSimple), 1);

        protected override (Type, long) In_0 => (typeof(CopperWire), 2);
        protected override (Type, long) In_1 => (typeof(WoodPlank), 1);
    }
}
