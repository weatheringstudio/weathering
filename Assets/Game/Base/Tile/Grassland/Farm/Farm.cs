
using System;
using System.Collections.Generic;

namespace Weathering
{
    [Concept]
    public class Farm : Factory1Out, ILinkProvider
    {
        public override string SpriteKey => !Working ? "FarmGrowing" : "FarmRipe";

        protected override Type Type => typeof(GrainSupply);

        protected override long WorkerCost => 1;

        protected override long BaseValue => 8;
    }
}

