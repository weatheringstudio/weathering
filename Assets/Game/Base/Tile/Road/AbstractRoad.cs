

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
    /// <summary>
    /// 目前主要建筑类型：AbstractFactoryStatic, AbstractRoad, TransportStation, TransportStationDest, WareHouse
    /// AbstractRoad特征：输入任意，物流输出同样的东西
    /// </summary>
    public abstract class AbstractRoad : StandardTile, ILinkConsumer, ILinkProvider, ILinkQuantityRestriction, ILinkEvent, ILinkTypeRestriction, IPassable
    {
        public bool Passable => true;

        public override string SpriteLeft => GetSprite(Vector2Int.left, typeof(ILeft));
        public override string SpriteRight => GetSprite(Vector2Int.right, typeof(IRight));
        public override string SpriteUp => GetSprite(Vector2Int.up, typeof(IUp));
        public override string SpriteDown => GetSprite(Vector2Int.down, typeof(IDown));

        public override string SpriteKey => (RoadRef.Value == 0 || RoadRef.Type == null) ? null : ConceptResource.Get(RoadRef.Type).Name;
        public override string SpriteKeyOverlay => (RoadRef.Value == 0 || RoadRef.Type == null) ? null : "RoadStockpile";

        private string GetSprite(Vector2Int pos, Type direction) {
            IRefs refs = Map.Get(Pos - pos).Refs;
            if (refs == null) return null;
            if (refs.TryGet(direction, out IRef result)) return result.Value < 0 ? ConceptResource.Get(result.Type).Name : null;
            return null;
        }

        public void OnLink(Type _, long quantity) {
            if (CanDestruct()) {
                RoadRef.Type = null;
            }
        }

        public override string SpriteKeyBase => null; // TerrainDefault.CalculateTerrainName(Map as StandardMap, Pos);
        public override string SpriteKeyRoad {
            get {
                int index = TileUtility.Calculate4x4RuleTileIndex(this, (tile, direction) => Refs.Has(direction) || ((RoadRef.Type == null) && (tile is AbstractRoad) && (tile as AbstractRoad).RoadRef.Type == null)
                );
                return $"{(SpriteKeyRoadBase == null ? "Road" : SpriteKeyRoadBase)}_{index}";
            }
        }
        protected virtual string SpriteKeyRoadBase { get; } = null;

        public IRef RoadRef { get; private set; }

        public abstract long LinkQuantityRestriction { get; }
        public abstract Type LinkTypeRestriction { get; }

        public void Consume(List<IRef> refs) {
            refs.Add(RoadRef);
        }

        public void Provide(List<IRef> refs) {
            refs.Add(RoadRef);
        }

        public override void OnConstruct() {
            base.OnConstruct();
            Refs = Weathering.Refs.GetOne();
            RoadRef = Refs.Create<AbstractRoad>();
            RoadRef.Type = null;
            RoadRef.BaseValue = long.MaxValue;
        }

        public override void OnEnable() {
            base.OnEnable();
            RoadRef = Refs.Get<AbstractRoad>();
        }

        public override void OnTap() {
            var items = UI.Ins.GetItems();

            if (RoadRef.Type != null) {
                // 传送中的物品图像
                items.Add(UIItem.CreateTileImage(ConceptResource.Get(RoadRef.Type)));
                items.Add(UIItem.CreateSeparator());
            }

            LinkUtility.AddButtons(items, this);

            items.Add(UIItem.CreateDestructButton<TerrainDefault>(this, CanDestruct));

            UI.Ins.ShowItems(Localization.Ins.Get(GetType()), items);
        }

        public override bool CanDestruct() => !LinkUtility.HasAnyLink(this);
    }
}
