
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{


    [Concept]
    public class Grassland : StandardTile
    {
        public override string SpriteKey => typeof(Grassland).Name;


        public override void OnEnable() {
            base.OnEnable();
        }

        public static uint Moisture(uint hashcode) {
            return 20 + hashcode % 60;
        }

        public static uint Temporature(uint hashcode) {
            return hashcode % 36;
        }

        public override void OnTap() {
            var items = new List<IUIItem> { };

            // items.Add(UIItem.CreateConstructionButton<GrasslandRoad>(this, true));
            items.Add(RoadUtility.CreateButtonOfConstructingRoad<GrasslandRoad>(Map, Pos, true, OnTap));

            // items.Add(UIItem.CreateConstructionButton<Village>(this, typeof(Wood), 10));

            items.Add(UIItem.CreateConstructionButton<Farm>(this, typeof(Food), 50));

            items.Add(UIItem.CreateConstructionButton<Workshop>(this, typeof(Wood), 100));

            items.Add(UIItem.CreateConstructionButton<FacilityStorageManual>(this));

            UI.Ins.ShowItems($"{Localization.Ins.Get<Grassland>()}  温度{Temporature(HashCode)} 湿度{Moisture(HashCode)}", items);
        }

        private UIItem UIItem_CreateConstructionButtonOfRoad() {
            //return new UIItem {
            //    Type = IUIItemType.Button,
            //    Content = $"{Localization.Ins.Get<Construct>()}{Localization.Ins.Get<GrasslandRoad>()}",
            //    OnTap =
            //        () => {
            //            if (GrasslandRoad.CanBeBuiltOn(Map, Pos, out string info, out Vector2Int direction)) {
            //                GrasslandRoad.ConstructOn(Map, Pos);

            //                Map.UpdateAt<GrasslandRoad>(Pos);
            //                GrasslandRoad road = Map.Get(Pos) as GrasslandRoad;
            //                if (road == null) throw new Exception();
            //                road.NeighborRoadDirection = direction;

            //                UI.Ins.Active = false;
            //            }
            //            else {
            //                UIPreset.Notify(OnTap, info, "不能在此建造道路");
            //            }
            //        }
            //    ,
            //};
            return null;
        }
    }
}

