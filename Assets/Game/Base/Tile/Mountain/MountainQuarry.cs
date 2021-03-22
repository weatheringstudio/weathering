
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
    // 石材
    [ConceptSupply(typeof(StoneSupply))]
    [ConceptDescription(typeof(StoneDescription))]
    [Depend(typeof(Discardable))]
    [Concept]
    public class Stone { }
    [ConceptResource(typeof(Stone))]
    [Depend(typeof(NonDiscardableSupply))]
    [Concept]
    public class StoneSupply { }
    [Concept]
    public class StoneDescription { }


    [Concept]
    public class MountainQuarry : Factory
    {
        protected override bool PreserveLandscape => true;
        public override string SpriteKey => DecoratedSpriteKey(typeof(MountainQuarry).Name);
        protected override long WorkerCost => 1;
        protected override (Type, long) Out0 => (typeof(StoneSupply), 1);
    }
}

