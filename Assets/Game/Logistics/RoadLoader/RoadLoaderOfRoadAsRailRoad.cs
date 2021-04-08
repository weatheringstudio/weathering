

using System;

namespace Weathering
{

    [ConstructionCostBase(typeof(BuildingPrefabrication), 10, 0)]
    [BindTerrainType(typeof(TerrainType_Plain))]
    public class RoadLoaderOfRoadAsRailRoad : AbstractRoad, IRoadAsRailRoad_CanLinkWith
    {

        //private const string road = "Hennery";
        //protected override string SpriteKeyRoadBase => road;

        public override string SpriteKeyRoad => "Hennery";

        public override long RoadQuantityRestriction => 200;

        public override Type LinkTypeRestriction => typeof(TransportableSolid);
    }
}
