
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{

    [Concept]
    public class GrasslandRoad : StandardTile, IRoadlike
    {
        public override string SpriteKey {
            get {
                int index = TileUtility.Calculate4x4RuleTileIndex(tile => tile.GetType() == typeof(GrasslandRoad), Map, Pos);
                return $"StoneRoad_{index}";
            }
        }

        public override void OnConstruct() {
            base.OnConstruct();
            Refs = Weathering.Refs.GetOne();
        }

        public override void OnTap() {
            var items = new List<IUIItem>();

            items.Add(UIItem.CreateMultilineText("【在目前游戏版本中，道路暂时没有作用。在以后的版本中，建筑需要贴近道路才能自动化，进行物流】"));

            items.Add(Road.CreateButtonOfDestructingRoad<Grassland>(this, OnTap));

            UI.Ins.ShowItems(Localization.Ins.Get<GrasslandRoad>(), items);
        }
    }
}

