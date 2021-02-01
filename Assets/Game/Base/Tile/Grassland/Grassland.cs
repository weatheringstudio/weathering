
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

        public override void OnTap() {
            var items = new List<IUIItem> {};

            items.Add(UIItem.CreateConstructionButton<Village>(this, typeof(Wood), 2));

            items.Add(UIItem.CreateConstructionButton<Farm>(this, typeof(Food), 50));

            UI.Ins.ShowItems($"{Localization.Ins.Get<Grassland>()}  温度{Temporature()} 湿度{Moisture()}", items);
        }

        private uint Moisture() {
            return 20 + HashCode % 60;
        }

        private uint Temporature() {
            return HashCode % 36;
        }
    }
}

