

using System;

namespace Weathering
{
    // 轮子
    [ConceptSupply(typeof(WheelPrimitiveSupply))]
    [ConceptDescription(typeof(WheelPrimitiveDescription))]
    [Depend(typeof(Discardable))]
    [Concept]
    public class WheelPrimitive { }

    [ConceptResource(typeof(WheelPrimitive))]
    [Depend(typeof(Transportable))]
    [Concept]
    public class WheelPrimitiveSupply { }

    [Concept]
    public class WheelPrimitiveDescription { }

    [ConstructionCostBase(typeof(WoodPlank), 100)]
    public class WorkshopOfWheelPrimitive : AbstractFactoryStatic
    {
        public override string SpriteKey => DecoratedSpriteKey(typeof(WorkshopOfWoodcutting).Name);

        protected override (Type, long) In_0_Inventory => (typeof(Worker), 1);

        protected override (Type, long) Out0 => (typeof(WheelPrimitiveSupply), 1);

        protected override (Type, long) In_0 => (typeof(WoodPlankSupply), 2);
        protected override (Type, long) In_1 => (typeof(StoneBrickSupply), 2);
    }
}
