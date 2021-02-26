
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{


    public class Road : StandardTile, ILinkable
    {
        public override bool HasSpriteDirection => true;
        public override string SpriteLeft => Refs.Has<IRight>() && Refs.Get<IRight>().Value > 0 ? typeof(Food).Name : null;
        public override string SpriteRight => Refs.Has<ILeft>() && Refs.Get<ILeft>().Value > 0 ? typeof(Food).Name : null;
        public override string SpriteUp => Refs.Has<IDown>() && Refs.Get<IDown>().Value > 0 ? typeof(Food).Name : null;
        public override string SpriteDown => Refs.Has<IUp>() && Refs.Get<IUp>().Value > 0 ? typeof(Food).Name : null;

        public override string SpriteKey {
            get {
                int index = TileUtility.Calculate4x4RuleTileIndex(tile => (tile is Road) || LinkUtility.HasLink(this, tile.GetPos() - Pos), Map, Pos);
                return $"StoneRoad_{index}";
            }
        }


        public override void OnConstruct() {
            base.OnConstruct();
            Refs = Weathering.Refs.GetOne();
            Refs.Create<Road>();
        }

        public IRef Res => res;
        IRef res;
        public override void OnEnable() {
            base.OnEnable();
            res = Refs.Get<Road>();
        }

        public override void OnTap() {

            var items = UI.Ins.GetItems();

            //items.Add(UIItem.CreateText($" left {Refs.Has<ILeft>()}"));
            //items.Add(UIItem.CreateText($" right {Refs.Has<IRight>()}"));
            //items.Add(UIItem.CreateText($" up {Refs.Has<IUp>()}"));
            //items.Add(UIItem.CreateText($" down {Refs.Has<IDown>()}"));

            LinkUtility.CreateButtons(items, this, res);

            UI.Ins.ShowItems("道路", items);
        }
    }
}

