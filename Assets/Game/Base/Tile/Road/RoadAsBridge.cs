

using System;

namespace Weathering
{
    [CanBeBuildOnNotPassableTerrain]
    [ConstructionCostBase(typeof(StoneBrick), 30, 0)]
    [BindTerrainType(typeof(TerrainType_Sea))]
    public class RoadAsBridge : AbstractRoad
    {
        protected override bool PreserveLandscape => true;

        protected override string SpriteKeyRoadBase => "Bridge";

        public override long LinkQuantityRestriction => 10;

        public override Type LinkTypeRestriction => typeof(Transportable);
    }
}
