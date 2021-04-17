
using System;

namespace Weathering
{
    public class RoadForSolid : AbstractRoad
    {
        public override float WalkingTimeModifier { get => 0.8f; }
        public override long RoadQuantityRestriction => 10;

        public override Type LinkTypeRestriction => typeof(DiscardableSolid);
    }
}

