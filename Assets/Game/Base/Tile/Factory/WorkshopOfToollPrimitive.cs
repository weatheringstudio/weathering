

using System;

namespace Weathering
{
    // 工具
    [ConceptSupply(typeof(ToolPrimitiveSupply))]
    [ConceptDescription(typeof(ToolPrimitiveDescription))]
    [Depend(typeof(DiscardableSolid))]
    [Concept]
    public class ToolPrimitive { }

    [ConceptResource(typeof(ToolPrimitive))]
    [Depend(typeof(TransportableSolid))]
    [Concept]
    public class ToolPrimitiveSupply { }

    [Concept]
    public class ToolPrimitiveDescription { }

    [ConstructionCostBase(typeof(WoodPlank), 100)]
    public class WorkshopOfToolPrimitive : AbstractFactoryStatic
    {
        public override string SpriteKey => DecoratedSpriteKey(typeof(WorkshopOfWoodcutting).Name);

        protected override (Type, long) In_0_Inventory => (typeof(Worker), 1);

        protected override (Type, long) Out0 => (typeof(ToolPrimitiveSupply), 3);

        protected override (Type, long) In_0 => (typeof(WoodPlankSupply), 1);
        protected override (Type, long) In_1 => (typeof(StoneBrickSupply), 1);
    }
}
