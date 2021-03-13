
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
    [Concept]
    public class MountainMine : Factory
    {
        protected override bool PreserveLandscape => true;
        public override string SpriteKey => DecoratedSpriteKey(typeof(MountainMine).Name);
        protected override long WorkerCost => 1;
        protected override (Type, long) Out0 => (typeof(MetalOreSupply), 1);
    }
}

