
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{

    public class Road : StandardTile, ILinkConsumer, ILinkProvider, ILinkEvent, ILinkSpeedLimit {
        public override bool HasDynamicSpriteAnimation => true;
        public override string SpriteLeft => GetSprite(Vector2Int.left, typeof(ILeft));
        public override string SpriteRight => GetSprite(Vector2Int.right, typeof(IRight));
        public override string SpriteUp => GetSprite(Vector2Int.up, typeof(IUp));
        public override string SpriteDown => GetSprite(Vector2Int.down, typeof(IDown));
        private string GetSprite(Vector2Int pos, Type direction) {
            IRefs refs = Map.Get(Pos - pos).Refs;
            if (refs == null) return null;
            if (refs.TryGet(direction, out IRef result)) return result.Value < 0 ? ConceptResource.Get(result.Type).Name : null;
            return null;
        }

        public override string SpriteKeyBase => TerrainDefault.CalculateTerrain(Map as StandardMap, Pos).Name;
        public override string SpriteKeyRoad {
            get {
                int index = TileUtility.Calculate4x4RuleTileIndex(this, (tile, direction) => Refs.Has(direction) || ((tile is Road) && (tile as Road).RoadRef.Type == RoadRef.Type));
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

        public int LinkLimit => 100;

        public override void OnEnable() {
            base.OnEnable();
            RoadRef = Refs.Get<Road>();
        }

        public void OnLink(Type _, long __) {
            if (!LinkUtility.HasAnyLink(this)) {
                RoadRef.Type = null;
            }
        }

        private const int MAX_RECURSION_DEPTH = 20;
        public override void OnTap() {

            var items = UI.Ins.GetItems();

            items.Add(UIItem.CreateButton("沿路运输", () => {
                TransportAlongRoad(items, MAX_RECURSION_DEPTH);
            }));
            items.Add(UIItem.CreateButton("沿路返回", () => {
                TransportAlongRoad_Undo(items, MAX_RECURSION_DEPTH);
            }));

            items.Add(UIItem.CreateSeparator());

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

        private void TransportAlongRoad(List<IUIItem> items, int depth) {
            if (depth < 0) return;
            LinkUtility.AddConsumerButtons(items, this, true);
            LinkUtility.AddProviderButtons(items, this, true);
            Road theRoad = TheOnlyOutputRoad();
            if (theRoad != null) theRoad.TransportAlongRoad(null, depth - 1);
        }
        private void TransportAlongRoad_Undo(List<IUIItem> items, int depth) {
            if (depth < 0) return;
            LinkUtility.AddProviderButtons_Undo(items, this, true);
            LinkUtility.AddConsumerButtons_Undo(items, this, true);
            Road theRoad = TheOnlyInputRoad();
            if (theRoad != null) theRoad.TransportAlongRoad_Undo(null, depth - 1);
        }

        private Road TheOnlyOutputRoad() {
            int count = 0;
            Road theRoad = null;
            ITile left = Map.Get(Pos + Vector2Int.left);
            if (left is Road leftRoad && Refs.Has<ILeft>() && Refs.Get<ILeft>().Value < 0) {
                count++;
                // if (count != 1) return null; // impossible
                theRoad = leftRoad;
            }
            ITile right = Map.Get(Pos + Vector2Int.right);
            if (right is Road rightRoad && Refs.Has<IRight>() && Refs.Get<IRight>().Value < 0) {
                count++;
                // if (count != 1) return null;
                theRoad = rightRoad;
            }
            ITile up = Map.Get(Pos + Vector2Int.up);
            if (up is Road upRoad && Refs.Has<IUp>() && Refs.Get<IUp>().Value < 0) {
                count++;
                // if (count != 1) return null;
                theRoad = upRoad;
            }
            ITile down = Map.Get(Pos + Vector2Int.down);
            if (down is Road downRoad && Refs.Has<IDown>() && Refs.Get<IDown>().Value < 0) {
                count++;
                // if (count != 1) return null;
                theRoad = downRoad;
            }
            return count == 1 ? theRoad : null;
        }
        private Road TheOnlyInputRoad() {
            int count = 0;
            Road theRoad = null;
            ITile left = Map.Get(Pos + Vector2Int.left);
            if (left is Road leftRoad && leftRoad.RoadRef.Type != null) {
                count++;
                if (count != 1) return null; // impossible
                theRoad = leftRoad;
            }
            ITile right = Map.Get(Pos + Vector2Int.right);
            if (right is Road rightRoad && rightRoad.RoadRef.Type != null) {
                count++;
                if (count != 1) return null;
                theRoad = rightRoad;
            }
            ITile up = Map.Get(Pos + Vector2Int.up);
            if (up is Road upRoad && upRoad.RoadRef.Type != null) {
                count++;
                if (count != 1) return null;
                theRoad = upRoad;
            }
            ITile down = Map.Get(Pos + Vector2Int.down);
            if (down is Road downRoad && downRoad.RoadRef.Type != null) {
                count++;
                if (count != 1) return null;
                theRoad = downRoad;
            }
            return theRoad;
        }
    }
}

