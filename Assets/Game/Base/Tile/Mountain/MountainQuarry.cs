
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
    [Concept]
    public class MountainQuarry : Factory
    {
        protected override bool PreserveLandscape => true;
        public override string SpriteKey => DecoratedSpriteKey(typeof(MountainQuarry).Name);
        protected override long WorkerCost => 1;
        protected override (Type, long) Out0 => (typeof(StoneSupply), 1);
    }
}

