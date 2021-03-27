

using System;

namespace Weathering
{

    // 铁锭
    [ConceptSupply(typeof(IronIngotSupply))]
    [ConceptDescription(typeof(MetalIngotOfIronDescription))]
    [Depend(typeof(MetalIngot))]
    [Concept]
    public class IronIngot { }

    [ConceptResource(typeof(IronIngot))]
    [Depend(typeof(MetalIngotSupply))]
    [Concept]
    public class IronIngotSupply { }

    [Concept]
    public class MetalIngotOfIronDescription { }

    [ConstructionCostBase(typeof(StoneBrick), 100)]
    public class WorkshopOfIronSmelting : AbstractFactoryStatic
    {
        public override string SpriteKey => DecoratedSpriteKey(typeof(WorkshopOfMetalSmelting).Name);

        protected override (Type, long) In_0_Inventory => (typeof(Worker), 1);

        protected override (Type, long) Out0 => (typeof(IronIngotSupply), 1);

        protected override (Type, long) In_0 => (typeof(IronOreSupply), 2);

        protected override (Type, long) In_1 => (typeof(FuelSupply), 2);
    }
}
