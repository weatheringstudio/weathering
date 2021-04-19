

using System;

namespace Weathering
{

    [ConstructionCostBase(typeof(BuildingPrefabrication), 10, 0)]
    [BindTerrainType(typeof(TerrainType_Plain))]
    public class RoadLoaderOfRoadAsCanal : AbstractRoad, IRoadAsCanalRoad_CanLinkWith
    {


        public override string SpriteKeyRoad => "Hennery";

        public override long RoadQuantityRestriction => 100;

        public override Type LinkTypeRestriction => typeof(DiscardableSolid);
    }
}
