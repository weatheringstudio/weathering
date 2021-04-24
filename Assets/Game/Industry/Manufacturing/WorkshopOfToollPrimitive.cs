

using System;

namespace Weathering
{
    // 工具
    [Depend(typeof(DiscardableSolid))]
    public class ToolPrimitive { }


    [ConstructionCostBase(typeof(WoodPlank), 100)]
    public class WorkshopOfToolPrimitive : AbstractFactoryStatic
    {
        public override string SpriteKey => typeof(WorkshopOfWoodcutting).Name;

        protected override (Type, long) In_0_Inventory => (typeof(Worker), 1);

        protected override (Type, long) Out0 => (typeof(ToolPrimitive), 1);

        protected override (Type, long) In_0 => (typeof(WoodPlank), 1);
        protected override (Type, long) In_1 => (typeof(StoneBrick), 1);
    }
}
