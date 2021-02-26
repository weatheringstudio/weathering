
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{


    public class Road : StandardTile, IProvider
    {
        public override bool HasSpriteDirection => true;
        public override string SpriteLeft => Refs.Has<IConsumerLeft>() ? typeof(Food).Name : null;
        public override string SpriteRight => Refs.Has<IConsumerRight>() ? typeof(Food).Name : null;
        public override string SpriteUp => Refs.Has<IConsumerUp>() ? typeof(Food).Name : null;
        public override string SpriteDown => Refs.Has<IConsumerDown>() ? typeof(Food).Name : null;

        public override string SpriteKey {
            get {
                int index = TileUtility.Calculate4x4RuleTileIndex(tile => (tile as Road != null) || LinkUtility.HasLink(this, Pos - tile.GetPos()), Map, Pos);
                return $"StoneRoad_{index}";
            }
        }

        public (Type, long) CanProvide => (res.Type, res.Value);

        public bool IsLikeRoad(Vector2Int _) { return true; }

        public override void OnConstruct() {
            base.OnConstruct();
            Refs = Weathering.Refs.GetOne();
            Refs.Create<Road>(); // Type运输物品类型 Value运输物品数量
        }

        IRef res;
        public override void OnEnable() {
            base.OnEnable();
            res = Refs.Get<Road>();
        }

        public override void OnTap() {

            var items = UI.Ins.GetItems();

            items.Add(UIItem.CreateText($"provider left {Refs.Has<IProviderLeft>()}"));
            items.Add(UIItem.CreateText($"provider right {Refs.Has<IProviderRight>()}"));
            items.Add(UIItem.CreateText($"consumer left {Refs.Has<IConsumerLeft>()}"));
            items.Add(UIItem.CreateText($"consumer right {Refs.Has<IConsumerRight>()}"));

            LinkUtility.CreateDescription(items, res);
            LinkUtility.CreateButtons(items, this, res);

            UI.Ins.ShowItems("道路", items);
        }

        public void Provide((Type, long) items) {
            if (res.Type != items.Item1) throw new Exception();
            if (res.Value < items.Item2) throw new Exception();
            res.Value -= items.Item2;
        }
    }
}

