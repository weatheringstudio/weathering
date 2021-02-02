
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
            var items = new List<IUIItem> {};

            items.Add(UIItem.CreateConstructionButton<Village>(this, typeof(Wood), 10));

            items.Add(UIItem.CreateConstructionButton<Farm>(this, typeof(Food), 50));

            items.Add(UIItem.CreateConstructionButton<Workshop>(this));

            items.Add(UIItem.CreateConstructionButton<FacilityStorageManual>(this));

            UI.Ins.ShowItems($"{Localization.Ins.Get<Grassland>()}  温度{Temporature(HashCode)} 湿度{Moisture(HashCode)}", items);
        }
    }
}

