
using System;
using System.Collections.Generic;

namespace Weathering
{
    [Concept]
    class ForestLoggingCamp : Factory1Out, ILinkProvider
    {
        public override string SpriteKey => typeof(ForestLoggingCamp).Name;

        protected override Type Type => typeof(WoodSupply);

        protected override long WorkerCost => 1;

        protected override long BaseValue => 1;
    }
}

