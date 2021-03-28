

using System;

namespace Weathering
{
    // 轮子
    [ConceptSupply(typeof(MachinePrimitiveSupply))]
    [ConceptDescription(typeof(MachinePrimitiveDescription))]
    [Depend(typeof(Discardable))]
    [Concept]
    public class MachinePrimitive { }

    [ConceptResource(typeof(MachinePrimitive))]
    [Depend(typeof(Transportable))]
    [Concept]
    public class MachinePrimitiveSupply { }

    [Concept]
    public class MachinePrimitiveDescription { }

    [ConstructionCostBase(typeof(ToolPrimitive), 100)]
    public class WorkshopOfMachinePrimitive : AbstractFactoryStatic
    {
        public override string SpriteKey => DecoratedSpriteKey(typeof(WorkshopOfWoodcutting).Name);

        protected override (Type, long) In_0_Inventory => (typeof(Worker), 1);

        protected override (Type, long) Out0 => (typeof(MachinePrimitiveSupply), 1);

        protected override (Type, long) In_0 => (typeof(IronIngotSupply), 1);
        protected override (Type, long) In_1 => (typeof(CopperIngotSupply), 1);
    }
}
