
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
    public class SeaHolyShip : StandardTile, IRoadlike, ISealike
    {

        private int index = 0;
        public override string SpriteKey {
            get {
                index = TileUtility.Calculate6x8RuleTileIndex(tile => typeof(ISealike).IsAssignableFrom(tile.GetType()), Map, Pos);
                return "Sea_" + index.ToString();
            }
        }
        public override string SpriteOverlayKey => typeof(SeaHolyShip).Name;

        public override void OnConstruct() {
            base.OnConstruct();
            Refs = Weathering.Refs.GetOne();
        }

        public override void OnTap() {
            var items = new List<IUIItem>();

            items.Add(Road.CreateButtonOfDestructingRoad<Sea>(this, OnTap));

            UI.Ins.ShowItems(Localization.Ins.Get<SeaHolyShip>(), items);
        }
    }
}

