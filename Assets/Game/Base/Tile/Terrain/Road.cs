
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{


    public class Road : StandardTile, ILinkConsumer, ILinkProvider, ILinkEvent, ILinkSpeedLimit
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
            RoadRef = Refs.Create<Road>();
            RoadRef.Type = null;
            RoadRef.BaseValue = long.MaxValue;
        }

        public IRef RoadRef { get; private set; }

        public int Limit => 10;

        public override void OnEnable() {
            base.OnEnable();
            RoadRef = Refs.Get<Road>();
        }

        public void OnLink() {
            if (!LinkUtility.HasAnyLink(this)) {
                RoadRef.Type = null;
            }
        }

        public override void OnTap() {

            var items = UI.Ins.GetItems();

            LinkUtility.AddButtons(items, this);
            if (RoadRef.Type == null) {
                items.Add(UIItem.CreateSeparator());
                items.Add(UIItem.CreateDestructButton<TerrainDefault>(this));
            }

            UI.Ins.ShowItems("道路", items);
        }

        public void Consume(List<IRef> refs) {
            refs.Add(RoadRef);
        }

        public void Provide(List<IRef> refs) {
            refs.Add(RoadRef);
        }
    }
}

