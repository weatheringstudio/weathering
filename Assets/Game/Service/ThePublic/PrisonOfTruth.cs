

using System;

namespace Weathering
{
    // 真理
    [ConceptSupply(typeof(TruthSupply))]
    [ConceptDescription(typeof(TruthDescription))]
    [Depend(typeof(Discardable))]
    [Concept]
    public class Truth { }
    [ConceptResource(typeof(Truth))]
    [ConceptDescription(typeof(TruthDescription))]
    [Depend(typeof(NonDiscardable))]
    [Concept]
    public class TruthSupply { }
    [Concept]
    public class TruthDescription { }

    public class ServiceOfTruth { }

    public class PrisonOfTruth : AbstractFactoryStatic
    {
        public override string SpriteKey => DecoratedSpriteKey(typeof(ServiceOfTruth).Name);
        protected override (Type, long) In_0_Inventory => (typeof(Worker), 1);
        protected override (Type, long) Out0_Inventory => (typeof(TruthSupply), 100);
    }
}
