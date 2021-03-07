
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
    [Concept]
    public class MountainQuarry : Factory1Out
    {
        public override string SpriteKey => "MountainQuarry";

        protected override Type Type => typeof(StoneSupply);
        protected override long BaseValue => 1;
        protected override long WorkerCost => 1;
    }
}

