

using System;
using System.Collections.Generic;

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

        public override Type LinkTypeRestriction => typeof(TransportableSolid);

        protected override void AddBuildingDescriptionPage(List<IUIItem> items) {
            items.Add(UIItem.CreateText($"使用锤子工具，可以在海上建造{Localization.Ins.Get<RoadAsBridge>()}"));
        }
    }
}
