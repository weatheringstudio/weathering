
using System;
using System.Collections.Generic;

namespace Weathering
{
    [Concept]
    class ForestLoggingCamp : Factory, ILinkProvider
    {
        protected override bool PreserveLandscape => true;
        public override string SpriteKey => DecoratedSpriteKey(typeof(ForestLoggingCamp).Name);
        protected override long WorkerCost => 1;
        protected override (Type, long) Out0 => (typeof(WoodSupply), 1);
    }
}

