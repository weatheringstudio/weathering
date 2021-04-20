
using System;

namespace Weathering
{
    public class RoadForSolid : AbstractRoad
    {
        public const long CAPACITY = 10;
        public override float WalkingTimeModifier { get => 0.8f; }
        public override long RoadQuantityRestriction => CAPACITY;

        public override Type LinkTypeRestriction => typeof(DiscardableSolid);
    }
}

