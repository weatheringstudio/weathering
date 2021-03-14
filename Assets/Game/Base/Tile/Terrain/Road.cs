
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{

    public class Road : StandardTile, ILinkConsumer, ILinkProvider, ILinkSpeedLimit, ILinkEvent
    {
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

        public override string SpriteKeyBase => TerrainDefault.CalculateTerrainName(Map as StandardMap, Pos);
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

        public void OnLink(Type _, long quantity) {
            if (quantity < 0 && !LinkUtility.HasAnyLink(this)) {
                RoadRef.Type = null;
            }
        }

        private const int MAX_RECURSION_DEPTH = 20;
        public override void OnTap() {

            var items = UI.Ins.GetItems();

            items.Add(UIItem.CreateButton($"延申这条道路", () => ConstructRoadPage(OnTap)));
            if (!LinkUtility.HasAnyLink(this)) {
                items.Add(UIItem.CreateButton($"{Localization.Ins.Get<Destruct>()}这条道路", () => DestructRoadPage(OnTap)));
            }

            items.Add(UIItem.CreateButton("沿路运输", () => {
                TransportAlongRoad(null, MAX_RECURSION_DEPTH);
                UI.Ins.Active = false;
            }));
            items.Add(UIItem.CreateButton("沿路返回", () => {
                TransportAlongRoad_Undo(null, MAX_RECURSION_DEPTH);
                UI.Ins.Active = false;
            }));

            items.Add(UIItem.CreateSeparator());


            LinkUtility.AddButtons(items, this);

            if (RoadRef.Type != null) {
                items.Add(UIItem.CreateTileImage(ConceptResource.Get(RoadRef.Type)));
            }

            if (RoadRef.Type == null) {
                items.Add(UIItem.CreateSeparator());
            }

            items.Add(UIItem.CreateDestructButton<TerrainDefault>(this, () => RoadRef.Type == null));

            UI.Ins.ShowItems("道路", items);
        }

        public void Consume(List<IRef> refs) {
            refs.Add(RoadRef);
        }

        public void Provide(List<IRef> refs) {
            refs.Add(RoadRef);
        }

        // 能否建造
        public static bool CanBeBuiltOn(ITile tile) {
            var terrainDefault = tile as TerrainDefault;
            if (terrainDefault == null) return false;

            if (!TerrainDefault.IsPassable(tile.GetMap() as StandardMap, tile.GetPos())) {
                return false;
            }

            MainQuest quest = MainQuest.Ins;
            if (terrainDefault.AltitudeType == typeof(AltitudePlain)
                && terrainDefault.MoistureType == typeof(MoistureForest)
                ) {
                return true;
            } else if (terrainDefault.AltitudeType == typeof(AltitudePlain)
                  && terrainDefault.MoistureType != typeof(MoistureForest)
                  && quest.IsUnlocked<Quest_HavePopulation_Settlement>()
                  ) {
                return true;
            }
            return false;
        }

        // 沿路拆毁
        private const int DESTRUCT_ROAD_RECURSION_DEPTH = 20;
        private void DestructRoadPage(Action back) {
            var items = UI.Ins.GetItems();

            if (back != null) {
                items.Add(UIItem.CreateReturnButton(back));
            }

            // start block
            items.Add(UIItem.CreateButton("拆除四方道路", () => {
                DestructRoadAlongDirection(Vector2Int.up, DESTRUCT_ROAD_RECURSION_DEPTH);
                DestructRoadAlongDirection(Vector2Int.down, DESTRUCT_ROAD_RECURSION_DEPTH);
                DestructRoadAlongDirection(Vector2Int.left, DESTRUCT_ROAD_RECURSION_DEPTH);
                DestructRoadAlongDirection(Vector2Int.right, DESTRUCT_ROAD_RECURSION_DEPTH);
                UI.Ins.Active = false;
            }));
            items.Add(UIItem.CreateButton("拆除北方道路", () => {
                DestructRoadAlongDirection(Vector2Int.up, DESTRUCT_ROAD_RECURSION_DEPTH);
                UI.Ins.Active = false;
            }));
            items.Add(UIItem.CreateButton("拆除南方道路", () => {
                DestructRoadAlongDirection(Vector2Int.down, DESTRUCT_ROAD_RECURSION_DEPTH);
                UI.Ins.Active = false;
            }));
            items.Add(UIItem.CreateButton("拆除西方道路", () => {
                DestructRoadAlongDirection(Vector2Int.left, DESTRUCT_ROAD_RECURSION_DEPTH);
                UI.Ins.Active = false;
            }));
            items.Add(UIItem.CreateButton("拆除东方道路", () => {
                DestructRoadAlongDirection(Vector2Int.right, DESTRUCT_ROAD_RECURSION_DEPTH);
            }));
            items.Add(UIItem.CreateButton("拆除横向道路", () => {
                DestructRoadAlongDirection(Vector2Int.left, DESTRUCT_ROAD_RECURSION_DEPTH);
                DestructRoadAlongDirection(Vector2Int.right, DESTRUCT_ROAD_RECURSION_DEPTH);
            }));
            items.Add(UIItem.CreateButton("拆除纵向道路", () => {
                DestructRoadAlongDirection(Vector2Int.up, DESTRUCT_ROAD_RECURSION_DEPTH);
                DestructRoadAlongDirection(Vector2Int.down, DESTRUCT_ROAD_RECURSION_DEPTH);
            }));
            // end block

            UI.Ins.ShowItems("道路", items);
        }
        private void DestructRoadAlongDirection(Vector2Int direction, int depth) {
            if (depth < 0) return;
            Vector2Int otherPos = Pos + direction;
            ITile otherTile = Map.Get(otherPos);
            if (otherTile is Road road) {
                if (!LinkUtility.HasAnyLink(road)) {
                    Map.UpdateAt<TerrainDefault>(otherPos);
                    road.DestructRoadAlongDirection(direction, depth - 1);
                }
            }
            Map.UpdateAt<TerrainDefault>(Pos);
            UI.Ins.Active = false;
        }

        // 沿路建造
        private const int CONSTRUCT_ROAD_RECURSION_DEPTH = 20;
        private void ConstructRoadPage(Action back) {
            var items = UI.Ins.GetItems();

            if (back != null) {
                items.Add(UIItem.CreateReturnButton(back));
            }

            //items.Add(UIItem.CreateButton("放弃加长道路", () => {
            //    UI.Ins.Active = false;
            //}));

            // start block
            items.Add(UIItem.CreateButton("向四方加长道路", () => {
                ConstructRoadAlongDirection(Vector2Int.up, CONSTRUCT_ROAD_RECURSION_DEPTH);
                ConstructRoadAlongDirection(Vector2Int.down, CONSTRUCT_ROAD_RECURSION_DEPTH);
                ConstructRoadAlongDirection(Vector2Int.left, CONSTRUCT_ROAD_RECURSION_DEPTH);
                ConstructRoadAlongDirection(Vector2Int.right, CONSTRUCT_ROAD_RECURSION_DEPTH);
            }));
            items.Add(UIItem.CreateButton("向横向加长道路", () => {
                ConstructRoadAlongDirection(Vector2Int.left, CONSTRUCT_ROAD_RECURSION_DEPTH);
                ConstructRoadAlongDirection(Vector2Int.right, CONSTRUCT_ROAD_RECURSION_DEPTH);
            }));
            items.Add(UIItem.CreateButton("向纵向加长道路", () => {
                ConstructRoadAlongDirection(Vector2Int.up, CONSTRUCT_ROAD_RECURSION_DEPTH);
                ConstructRoadAlongDirection(Vector2Int.down, CONSTRUCT_ROAD_RECURSION_DEPTH);
            }));
            items.Add(UIItem.CreateButton("向北方加长道路", () => {
                ConstructRoadAlongDirection(Vector2Int.up, CONSTRUCT_ROAD_RECURSION_DEPTH);
            }));
            items.Add(UIItem.CreateButton("向南方加长道路", () => {
                ConstructRoadAlongDirection(Vector2Int.down, CONSTRUCT_ROAD_RECURSION_DEPTH);
            }));
            items.Add(UIItem.CreateButton("向西方加长道路", () => {
                ConstructRoadAlongDirection(Vector2Int.left, CONSTRUCT_ROAD_RECURSION_DEPTH);
            }));
            items.Add(UIItem.CreateButton("向东方加长道路", () => {
                ConstructRoadAlongDirection(Vector2Int.right, CONSTRUCT_ROAD_RECURSION_DEPTH);
            }));

            // end block

            UI.Ins.ShowItems("道路", items);
        }
        private void ConstructRoadAlongDirection(Vector2Int direction, int depth) {
            if (depth < 0) return;
            Vector2Int otherPos = Pos + direction;
            ITile otherTile = Map.Get(otherPos);
            if (CanBeBuiltOn(otherTile)) {
                Road road = Map.UpdateAt<Road>(otherPos);
                road.ConstructRoadAlongDirection(direction, depth - 1);
            }
            UI.Ins.Active = false;
        }

        // 沿路运输
        private void TransportAlongRoad(List<IUIItem> items, int depth) {
            if (depth < 0) return;
            if (CalcRoadCount() > 2) return;
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

        private int CalcRoadCount() {
            int count = 0;
            if (Map.Get(Pos + Vector2Int.up) is Road) count++;
            if (Map.Get(Pos + Vector2Int.down) is Road) count++;
            if (Map.Get(Pos + Vector2Int.left) is Road) count++;
            if (Map.Get(Pos + Vector2Int.right) is Road) count++;
            return count;
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

