

using System;

namespace Weathering
{
    // 高级电路板
    [Depend(typeof(DiscardableSolid))]
    public class CircuitBoardAdvanced { }

    [ConstructionCostBase(typeof(LightMaterial), 100)]
    public class FactoryOfCircuitBoardAdvanced : AbstractFactoryStatic
    {
        public override string SpriteKey =>typeof(Factory).Name;

        protected override (Type, long) In_0_Inventory => (typeof(Worker), 1);
        protected override (Type, long) In_1_Inventory => (typeof(Electricity), 30);

        protected override (Type, long) Out0 => (typeof(CircuitBoardAdvanced), 1);

        protected override (Type, long) In_0 => (typeof(CircuitBoardIntegrated), 2);
        protected override (Type, long) In_1 => (typeof(LightMaterial), 1);
    }
}
