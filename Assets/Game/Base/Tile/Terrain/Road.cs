
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

            TransportAlongRoads(items, 20);
            LinkUtility.CreateLinkButtons(items, this);
            LinkUtility.CreateUnlinkButtons(items, this);

            items.Add(UIItem.CreateSeparator());
            LinkUtility.CreateDescription(items, this);
            LinkUtility.CreateLinkInfo(items, Refs);

            // 还差几个功能。自动叠加运输，自动取消运输。

            items.Add(UIItem.CreateSeparator());
            items.Add(LinkUtility.CreateDestructionButton(this, Res));

            UI.Ins.ShowItems("道路", items);
        }

        private const string INPUT = "输入";
        private const string OUTPUT = "输出";
        private const string NORTH = "【北方】";
        private const string SOUTH = "【南方】";
        private const string WEST = "【西方】";
        private const string EAST = "【东方】";
        private void TransportAlongRoads(List<IUIItem> items, int depth) {
            if (Res.Value == 0) return; // 没有东西可以运输
            if (Res.Type == null) throw new Exception(); // 有null可以运输？

            ITile northTile = Map.Get(Pos + Vector2Int.up);
            ITile southTile = Map.Get(Pos + Vector2Int.down);
            ITile westTile = Map.Get(Pos + Vector2Int.left);
            ITile eastTile = Map.Get(Pos + Vector2Int.right);

            int roadCount = 0;
            Road roadNorth = northTile as Road;
            if (roadNorth != null && !Refs.Has(typeof(IUp))) roadCount++;
            Road roadSouth = southTile as Road;
            if (roadSouth != null && !Refs.Has(typeof(IDown))) roadCount++;
            Road roadWest = westTile as Road;
            if (roadWest != null && !Refs.Has(typeof(ILeft))) roadCount++;
            Road roadEast = eastTile as Road;
            if (roadEast != null && !Refs.Has(typeof(IRight))) roadCount++;

            if (items == null) {
                // 塞入需要的建筑。
                TryInsertIntoBuilding(northTile, typeof(IUp), typeof(IDown));
                TryInsertIntoBuilding(southTile, typeof(IDown), typeof(IUp));
                TryInsertIntoBuilding(westTile, typeof(ILeft), typeof(IRight));
                TryInsertIntoBuilding(eastTile, typeof(IRight), typeof(ILeft));
            }

            if (roadCount != 1) return;

            if (roadNorth != null) TransportAlongRoad(items, roadNorth, NORTH, typeof(IUp), typeof(IDown), depth);
            if (roadSouth != null) TransportAlongRoad(items, roadSouth, SOUTH, typeof(IDown), typeof(IUp), depth);
            if (roadWest != null) TransportAlongRoad(items, roadWest, WEST, typeof(ILeft), typeof(IRight), depth);
            if (roadEast != null) TransportAlongRoad(items, roadEast, EAST, typeof(IRight), typeof(ILeft), depth);

        }
        private void TryInsertIntoBuilding(ITile northTile, Type providerLinkType, Type consumerLinkType) {
            ILinkableConsumer consumer = northTile as ILinkableConsumer;
            if (consumer != null) {
                var canConsume = consumer.CanConsume;
                long quantity = Math.Min(Res.Value, canConsume.Item2);
                if (quantity > 0) {
                    if (Tag.HasTag(Res.Type, canConsume.Item1)) {
                        // Debug.LogWarning($"{quantity}");
                        LinkUtility.Link(this, northTile, Res, consumer.Res, providerLinkType, consumerLinkType,
                            canConsume.Item1, quantity);
                    } else {
                        // Debug.LogWarning($"{Res.Type}  ?? {canConsume.Item1}");
                    }
                }
            }
        }

        private void TransportAlongRoad(List<IUIItem> items, Road thatRoad, string directionText, Type providerLinkType, Type consumerLinkType, int depth) {

            if (thatRoad == null) return; // 此方向不是路
            if (thatRoad.Res.Type != null) return; // 路上已经有了东西

            if (Refs.Has(providerLinkType)) throw new Exception(); // 已经建立了链接。但那边路上怎么会没有东西？
            if (thatRoad.Refs.Has(consumerLinkType)) throw new Exception(); // 怎么这边每链接，那边有链接？肯定错了

            void action() {
                LinkUtility.Link(this, thatRoad, Res, thatRoad.Res, providerLinkType, consumerLinkType);
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

