
using System;
using System.Collections.Generic;

namespace Weathering
{
    [Concept]
    public class Farm : Factory
    {
        public override string SpriteKey => WorkerCount == 0 ? "FarmGrowing" : "FarmRipe";

        protected override long WorkerCost => 1;

        protected override (Type, long) Out0 => (typeof(GrainSupply), 8);
    }
}

