
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

        public void OnLink(Type direction) {
            if (Res.Value == 0 && !LinkUtility.HasAnyLink(this)) Res.Type = null;
        }

        public IRef Res { get; private set; }
        public override void OnEnable() {
            base.OnEnable();
            Res = Refs.Get<Road>();
        }

        public override void OnTap() {

            var items = UI.Ins.GetItems();

            TransportAlongRoads(items, true, 20);
            LinkUtility.CreateLinkButtons(items, this);
            LinkUtility.CreateUnlinkButtons(items, this);

            items.Add(UIItem.CreateSeparator());
            LinkUtility.CreateDescription(items, this);
            LinkUtility.CreateLinkInfo(items, Refs);


            items.Add(UIItem.CreateSeparator());
            items.Add(LinkUtility.CreateDestructionButton(this, Res));

            UI.Ins.ShowItems("道路", items);
        }

        private const string NORTH = "【北方】";
        private const string SOUTH = "【南方】";
        private const string WEST = "【西方】";
        private const string EAST = "【东方】";
        private void TransportAlongRoads(List<IUIItem> items, bool invokedByTapping, int depth) {
            if (Res.Value == 0) return; // 没有东西可以运输
            if (Res.Type == null) throw new Exception(); // 有null可以运输？

            ITile northTile = Map.Get(Pos + Vector2Int.up);
            ITile southTile = Map.Get(Pos + Vector2Int.down);
            ITile westTile = Map.Get(Pos + Vector2Int.left);
            ITile eastTile = Map.Get(Pos + Vector2Int.right);

            int roadCount = 0;

            Road roadNorth = northTile as Road;
            bool validRoadNorth = roadNorth != null && !Refs.Has(typeof(IUp));
            if (validRoadNorth) roadCount++;

            Road roadSouth = southTile as Road;
            bool validRoadSouth = roadSouth != null && !Refs.Has(typeof(IDown));
            if (validRoadSouth) roadCount++;

            Road roadWest = westTile as Road;
            bool validRoadWest = roadWest != null && !Refs.Has(typeof(ILeft));
            if (validRoadWest) roadCount++;

            Road roadEast = eastTile as Road;
            bool validRoadEast = roadEast != null && !Refs.Has(typeof(IRight));
            if (validRoadEast) roadCount++;

            if (!invokedByTapping) {
                TrySatisfyConsumer(northTile, typeof(IUp), typeof(IDown));
                TrySatisfyConsumer(southTile, typeof(IDown), typeof(IUp));
                TrySatisfyConsumer(westTile, typeof(ILeft), typeof(IRight));
                TrySatisfyConsumer(eastTile, typeof(IRight), typeof(ILeft));

                TrySatisfyProvider(northTile, typeof(IUp), typeof(IDown));
                TrySatisfyProvider(southTile, typeof(IDown), typeof(IUp));
                TrySatisfyProvider(westTile, typeof(ILeft), typeof(IRight));
                TrySatisfyProvider(eastTile, typeof(IRight), typeof(ILeft));
            }
            if (roadCount != 1) return;

            if (validRoadNorth) TransportAlongRoad(items, invokedByTapping, roadNorth, NORTH, typeof(IUp), typeof(IDown), depth);
            if (validRoadSouth) TransportAlongRoad(items, invokedByTapping, roadSouth, SOUTH, typeof(IDown), typeof(IUp), depth);
            if (validRoadWest) TransportAlongRoad(items, invokedByTapping, roadWest, WEST, typeof(ILeft), typeof(IRight), depth);
            if (validRoadEast) TransportAlongRoad(items, invokedByTapping, roadEast, EAST, typeof(IRight), typeof(ILeft), depth);
        }
        private void TrySatisfyConsumer(ITile tile, Type linkTypeOfProvider, Type linkTypeOfConsumer) {
            ILinkableConsumer consumer = tile as ILinkableConsumer;
            if (consumer != null) {
                var canConsume = consumer.CanConsume;
                long quantity = Math.Min(Res.Value, canConsume.Item2);
                if (quantity > 0) {
                    if (Tag.HasTag(Res.Type, canConsume.Item1)) {
                        LinkUtility.Link(this, tile, Res, consumer.Res, linkTypeOfProvider, linkTypeOfConsumer,
                            canConsume.Item1, quantity);
                    }
                }
            }
        }
        private void TrySatisfyProvider(ITile tile, Type linkTypeOfConsumer, Type linkTypeOfProvider) {
            ILinkableProvider provider = tile as ILinkableProvider;
            if (provider != null) {
                var canProvide = provider.CanProvide;
                long quantity = Math.Min(long.MaxValue - Res.Value, canProvide.Item2);
                if (quantity > 0) {
                    if (Tag.HasTag(Res.Type, canProvide.Item1)) {
                        LinkUtility.Link(tile, this, provider.Res, Res, linkTypeOfProvider, linkTypeOfConsumer,
                            canProvide.Item1, quantity);
                    }
                }
            }
        }

        private void TransportAlongRoad(List<IUIItem> items, bool invokedByTapping, Road thatRoad, string directionText, Type linkTypeOfProvider, Type linkTypeOfConsumer, int depth) {

            if (thatRoad == null) return; // 此方向不是路
            if (thatRoad.Res.Type != null) return; // 路上已经有了东西

            if (Refs.Has(linkTypeOfProvider)) throw new Exception($"{linkTypeOfProvider.Name} {linkTypeOfConsumer.Name}"); // 已经建立了链接。但那边路上怎么会没有东西？
            if (thatRoad.Refs.Has(linkTypeOfConsumer)) throw new Exception($"{linkTypeOfProvider.Name} {linkTypeOfConsumer.Name}"); // 怎么这边没链接，那边有链接？肯定错了

            void action() {
                if (invokedByTapping) {
                    TransportAlongRoads(null, false, depth - 1);
                } else {
                    LinkUtility.Link(this, thatRoad, Res, thatRoad.Res, linkTypeOfProvider, linkTypeOfConsumer);
                    thatRoad.TransportAlongRoads(null, false, depth - 1);
                }
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

