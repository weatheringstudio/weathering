

using System;

namespace Weathering
{

    // 水泥
    [ConceptSupply(typeof(ConcretePowderSupply))]
    [ConceptDescription(typeof(ConcretePowderDescription))]
    [Depend(typeof(Discardable))]
    [Concept]
    public class ConcretePowder { }

    [ConceptResource(typeof(ConcretePowder))]
    [Depend(typeof(Transportable))]
    [Concept]
    public class ConcretePowderSupply { }

    [Concept]
    public class ConcretePowderDescription { }

    [ConstructionCostBase(typeof(MachinePrimitive), 100)]
    public class FactoryOfConcrete : AbstractFactoryStatic
    {
        public override string SpriteKey => DecoratedSpriteKey(typeof(FactoryOfMetalSmelting).Name);

        protected override (Type, long) In_0_Inventory => (typeof(Worker), 1);
        protected override (Type, long) Out0 => (typeof(ConcretePowderSupply), 1);
        protected override (Type, long) In_0 => (typeof(IronOreSupply), 5);
        protected override (Type, long) In_1 => (typeof(StoneSupply), 5);
    }
}
