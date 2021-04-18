
using System;

namespace Weathering
{
    [ConstructionCostBase(typeof(ConcretePowder), 10, 0)]
    public class RoadOfConcrete : AbstractRoad
    {
        public override float WalkingTimeModifier { get => 0.6f; }

        private const string roadBase = "RoadOfConcrete";
        protected override string SpriteKeyRoadBase => roadBase;
        public override long RoadQuantityRestriction => 50;
        public override Type LinkTypeRestriction => typeof(DiscardableSolid);
    }
}

