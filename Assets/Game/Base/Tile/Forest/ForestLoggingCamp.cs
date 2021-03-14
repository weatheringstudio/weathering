
using System;
using System.Collections.Generic;

namespace Weathering
{
    // 木材
    [ConceptSupply(typeof(WoodSupply))]
    [ConceptDescription(typeof(WoodDescription))]
    [Depend(typeof(Discardable))]
    [Concept]
    public class Wood { }
    [ConceptResource(typeof(Wood))]
    [Depend(typeof(NonDiscardable))]
    [Concept]
    public class WoodSupply { }
    [Concept]
    public class WoodDescription { }


    [Concept]
    class ForestLoggingCamp : Factory, ILinkProvider
    {
        protected override bool PreserveLandscape => true;
        public override string SpriteKey => DecoratedSpriteKey(typeof(ForestLoggingCamp).Name);
        protected override long WorkerCost => 1;
        protected override (Type, long) Out0 => (typeof(WoodSupply), 1);
    }
}

