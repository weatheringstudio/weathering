

using System;

namespace Weathering
{

    // 铜锭
    [ConceptSupply(typeof(CopperIngotSupply))]
    [ConceptDescription(typeof(CopperIngotDescription))]
    [Depend(typeof(MetalIngot))]
    [Concept]
    public class CopperIngot { }

    [ConceptResource(typeof(CopperIngot))]
    [Depend(typeof(MetalIngotSupply))]
    [Concept]
    public class CopperIngotSupply { }

    [Concept]
    public class CopperIngotDescription { }

    [ConstructionCostBase(typeof(ToolPrimitive), 100)]
    public class WorkshopOfCopperSmelting : AbstractFactoryStatic
    {
        public override string SpriteKey => DecoratedSpriteKey(typeof(WorkshopOfMetalSmelting).Name);

        protected override (Type, long) In_0_Inventory => (typeof(Worker), 1);

        protected override (Type, long) Out0 => (typeof(CopperIngotSupply), 1);

        protected override (Type, long) In_0 => (typeof(CopperOreSupply), 2);

        protected override (Type, long) In_1 => (typeof(FuelSupply), 1);
    }
}
