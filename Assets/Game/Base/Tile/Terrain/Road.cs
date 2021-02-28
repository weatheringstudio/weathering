
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{


    public class Road : StandardTile, ILinkable
    {
        public override bool HasDynamicSpriteAnimation => true;
        public override string SpriteLeft => Refs.Has<IRight>() && Refs.Get<IRight>().Value > 0 ? typeof(Food).Name : null;
        public override string SpriteRight => Refs.Has<ILeft>() && Refs.Get<ILeft>().Value > 0 ? typeof(Food).Name : null;
        public override string SpriteUp => Refs.Has<IDown>() && Refs.Get<IDown>().Value > 0 ? typeof(Food).Name : null;
        public override string SpriteDown => Refs.Has<IUp>() && Refs.Get<IUp>().Value > 0 ? typeof(Food).Name : null;

        public override string SpriteKeyBase => TerrainDefault.CalculateTerrain(Map as StandardMap, Pos).Name;
        public override string SpriteKeyRoad {
            get {
                int index = TileUtility.Calculate4x4RuleTileIndex(this, (tile, direction) => (tile is Road) || Refs.Has(direction));
                return $"Road_{index}";
            }
        }

        public override void OnConstruct() {
            base.OnConstruct();
            Refs = Weathering.Refs.GetOne();
            Refs.Create<Road>();
        }

        public void OnLink(Type direction) { }

        public IRef Res { get; private set; }
        public override void OnEnable() {
            base.OnEnable();
            Res = Refs.Get<Road>();
        }

        public override void OnTap() {

            var items = UI.Ins.GetItems();

            LinkUtility.CreateDescription(items, Res);
            TransportAlongRoads(items, 20);
            LinkUtility.CreateLinkButtons(items, this, Res);
            LinkUtility.CreateUnlinkButtons(items, this, Res);

            items.Add(UIItem.CreateSeparator());
            items.Add(LinkUtility.CreateDestructionButton(this, Res));

            UI.Ins.ShowItems("道路", items);
        }

        private const string NORTH = "【北方】";
        private const string SOUTH = "【南方】";
        private const string WEST = "【西方】";
        private const string EAST = "【东方】";
        private void TransportAlongRoads(List<IUIItem> items, int depth) {
            if (Res.Value == 0) return; // 没有东西可以运输
            if (Res.Type == null) throw new Exception(); // 有null可以运输？

            int roadCount = 0;
            Road roadNorth = Map.Get(Pos + Vector2Int.up) as Road;
            if (roadNorth != null && !Refs.Has(typeof(IUp))) roadCount++;
            Road roadSouth = Map.Get(Pos + Vector2Int.down) as Road;
            if (roadSouth != null && !Refs.Has(typeof(IDown))) roadCount++;
            Road roadWest = Map.Get(Pos + Vector2Int.left) as Road;
            if (roadWest != null && !Refs.Has(typeof(ILeft))) roadCount++;
            Road roadEast = Map.Get(Pos + Vector2Int.right) as Road;
            if (roadEast != null && !Refs.Has(typeof(IRight))) roadCount++;

            if (roadCount != 1) return;

            if (roadNorth != null) TransportAlongRoad(items, roadNorth, NORTH, typeof(IUp), typeof(IDown), depth);
            if (roadSouth != null) TransportAlongRoad(items, roadSouth, SOUTH, typeof(IDown), typeof(IUp), depth);
            if (roadWest != null) TransportAlongRoad(items, roadWest, WEST, typeof(ILeft), typeof(IRight), depth);
            if (roadEast != null) TransportAlongRoad(items, roadEast, EAST, typeof(IRight), typeof(ILeft), depth);

            // 塞入需要的建筑。
        }

        private void TransportAlongRoad(List<IUIItem> items, Road thatRoad, string directionText, Type providerLinkType, Type consumerLinkType, int depth) {

            if (thatRoad == null) return; // 此方向不是路
            if (thatRoad.Res.Type != null) return; // 路上已经有了东西

            if (Refs.Has(providerLinkType)) throw new Exception(); // 已经建立了链接。但那边路上怎么会没有东西？
            if (thatRoad.Refs.Has(consumerLinkType)) throw new Exception(); // 怎么这边每链接，那边有链接？肯定错了

            void action() {
                IRef providerLink = Refs.Create(providerLinkType);
                providerLink.Type = Res.Type;
                providerLink.Value = -Res.Value;
                IRef consumerLink = thatRoad.Refs.Create(consumerLinkType);
                consumerLink.Type = Res.Type;
                consumerLink.Value = Res.Value;

                if (thatRoad.Res.Type == null) thatRoad.Res.Type = Res.Type;
                thatRoad.Res.Value += Res.Value;
                Res.Value = 0;

                NeedUpdateSpriteKeys = true;
                thatRoad.NeedUpdateSpriteKeys = true;

                OnLink(providerLinkType);
                thatRoad.OnLink(consumerLinkType);

                thatRoad.TransportAlongRoads(null, depth - 1);

                if (items != null) OnTap();
            }
            if (items != null) {
                items.Add(UIItem.CreateButton($"向{directionText}方向自动运输", action));
            } else {
                action();
            }
        }
    }
}

