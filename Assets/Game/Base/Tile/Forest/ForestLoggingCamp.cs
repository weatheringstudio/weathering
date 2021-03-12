
using System;
using System.Collections.Generic;

namespace Weathering
{
    [Concept]
    class ForestLoggingCamp : Factory, ILinkProvider
    {
        public override string SpriteKey => typeof(ForestLoggingCamp).Name;

        protected override long WorkerCost => 1;

        protected override (Type, long) Out0 => (typeof(WoodSupply), 1);
    }
}

