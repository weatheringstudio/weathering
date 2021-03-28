

using System;

namespace Weathering
{
    // 铜器
    [ConceptSupply(typeof(CopperProductSupply))]
    [ConceptDescription(typeof(CopperProductDescription))]
    [Depend(typeof(MetalProduct))]
    [Concept]
    public class CopperProduct { }

    [ConceptResource(typeof(CopperProduct))]
    [Depend(typeof(MetalProductSupply))]
    [Concept]
    public class CopperProductSupply { }

    [Concept]
    public class CopperProductDescription { }

    [ConstructionCostBase(typeof(ToolPrimitive), 100)]
    public class WorkshopOfCopperCasting : AbstractFactoryStatic
    {
        public override string SpriteKey => DecoratedSpriteKey(typeof(WorkshopOfMetalCasting).Name);

        protected override (Type, long) In_0_Inventory => (typeof(Worker), 1);

        protected override (Type, long) Out0 => (typeof(CopperProductSupply), 1);

        protected override (Type, long) In_0 => (typeof(CopperIngotSupply), 3);

        protected override (Type, long) In_1 => (typeof(FuelSupply), 1);
    }
}
