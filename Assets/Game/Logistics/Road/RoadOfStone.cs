
using System;

namespace Weathering
{
    [ConstructionCostBase(typeof(StoneBrick), 10, 0)]
    public class RoadOfStone : AbstractRoad
    {
        public override float WalkingTimeModifier { get => 0.7f; }

        private const string roadBase = "RoadOfStone";
        protected override string SpriteKeyRoadBase => roadBase;
        public override long RoadQuantityRestriction => 20;
        public override Type LinkTypeRestriction => typeof(DiscardableSolid);
    }
}

