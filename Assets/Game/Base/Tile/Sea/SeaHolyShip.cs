
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
    public class SeaHolyShip : StandardTile, IRoadlike, ISealike, IDefaultDestruction
    {

        private int index = 0;
        public override string SpriteKey {
            get {
                index = TileUtility.Calculate6x8RuleTileIndex(tile => typeof(ISealike).IsAssignableFrom(tile.GetType()), Map, Pos);
                return "Sea_" + index.ToString();
            }
        }
        public override string SpriteKeyOverlay => typeof(SeaHolyShip).Name;

        public Type DefaultDestruction => typeof(Sea);

        public override void OnConstruct() {
            base.OnConstruct();
            Refs = Weathering.Refs.GetOne();
            Map.Values.Create<SeaHolyShip>();
        }

        public override void OnTap() {
            var items = new List<IUIItem>();

            items.Add(UIItem.CreateText("每个世界只有一艘圣船"));

            // items.Add(RoadUtility.CreateButtonOfDestructingRoad<Sea>(this, OnTap));

            UI.Ins.ShowItems(Localization.Ins.Get<SeaHolyShip>(), items);
        }
    }
}

