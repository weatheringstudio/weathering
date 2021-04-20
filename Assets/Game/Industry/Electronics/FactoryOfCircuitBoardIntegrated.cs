

using System;

namespace Weathering
{
    // 集成电路板
    [Depend(typeof(DiscardableSolid))]
    public class CircuitBoardIntegrated { }

    [ConstructionCostBase(typeof(BuildingPrefabrication), 100)]
    public class FactoryOfCircuitBoardIntegrated : AbstractFactoryStatic
    {
        public override string SpriteKey => typeof(Factory).Name;

        protected override (Type, long) In_0_Inventory => (typeof(Worker), 1);
        protected override (Type, long) In_1_Inventory => (typeof(Electricity), 20);

        protected override (Type, long) Out0 => (typeof(CircuitBoardIntegrated), 1);

        protected override (Type, long) In_0 => (typeof(CircuitBoardSimple), 2);
        protected override (Type, long) In_1 => (typeof(Plastic), 1);
    }
}
