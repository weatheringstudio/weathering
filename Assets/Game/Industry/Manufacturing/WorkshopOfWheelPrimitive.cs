

using System;

namespace Weathering
{
    // 轮子
    [Depend(typeof(DiscardableSolid))]
    public class WheelPrimitive { }


    [ConstructionCostBase(typeof(ToolPrimitive), 100)]
    public class WorkshopOfWheelPrimitive : AbstractFactoryStatic
    {
        public override string SpriteKey => typeof(Workshop).Name;

        protected override (Type, long) In_0_Inventory => (typeof(Worker), 1);

        protected override (Type, long) Out0 => (typeof(WheelPrimitive), 1);

        protected override (Type, long) In_0 => (typeof(WoodPlank), 1);
        protected override (Type, long) In_1 => (typeof(StoneBrick), 1);
    }
}
