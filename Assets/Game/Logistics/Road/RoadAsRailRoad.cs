

using System;

namespace Weathering
{
    public interface IRoadAsRailRoad_CanLinkWith { }

    [ConstructionCostBase(typeof(BuildingPrefabrication), 10, 0)]
    [BindTerrainType(typeof(TerrainType_Plain))]
    public class RoadAsRailRoad : AbstractRoad, ILinkTileTypeRestriction, IRoadAsRailRoad_CanLinkWith
    {

        private const string road = "RoadAsRailRoad";
        protected override string SpriteKeyRoadBase => road;
        public override long RoadQuantityRestriction => 200;

        public override Type LinkTypeRestriction => typeof(DiscardableSolid);

        public Type LinkTileTypeRestriction => typeof(IRoadAsRailRoad_CanLinkWith);
    }
}
