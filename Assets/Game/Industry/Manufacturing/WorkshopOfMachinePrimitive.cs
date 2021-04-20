

using System;

namespace Weathering
{
    [Depend(typeof(DiscardableSolid))]
    public class MachinePrimitive { }


    [ConstructionCostBase(typeof(ToolPrimitive), 100)]
    public class WorkshopOfMachinePrimitive : AbstractFactoryStatic
    {
        public override string SpriteKey => typeof(Workshop).Name;

        protected override (Type, long) In_0_Inventory => (typeof(Worker), 3);

        protected override (Type, long) Out0 => (typeof(MachinePrimitive), 1);

        protected override (Type, long) In_0 => (typeof(IronProduct), 2);
        protected override (Type, long) In_1 => (typeof(CopperProduct), 1);
    }
}
