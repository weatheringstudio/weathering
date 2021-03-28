

using System;

namespace Weathering
{
    // 铁器
    [ConceptSupply(typeof(IronProductSupply))]
    [ConceptDescription(typeof(IronProductDescription))]
    [Depend(typeof(MetalProduct))]
    [Concept]
    public class IronProduct { }

    [ConceptResource(typeof(IronProduct))]
    [Depend(typeof(MetalProductSupply))]
    [Concept]
    public class IronProductSupply { }

    [Concept]
    public class IronProductDescription { }

    [ConstructionCostBase(typeof(ToolPrimitive), 100)]
    public class WorkshopOfIronCasting : AbstractFactoryStatic
    {
        public override string SpriteKey => DecoratedSpriteKey(typeof(WorkshopOfMetalCasting).Name);

        protected override (Type, long) In_0_Inventory => (typeof(Worker), 1);

        protected override (Type, long) Out0 => (typeof(IronProductSupply), 1);

        protected override (Type, long) In_0 => (typeof(IronIngotSupply), 3);

        protected override (Type, long) In_1 => (typeof(FuelSupply), 1);
    }
}
