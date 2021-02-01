﻿
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
            //UI.Ins.ShowItems($"{Localization.Ins.Get<Grassland>()}  温度{Temporature()} 湿度{Moisture()}"
            //    // , UIItem.CreateShortcutOfConstructionButton(this)
            //    , UIItem.CreateConstructButton<Farm>(this)
            //    , UIItem.CreateConstructButton<Village>(this)
            //    , UIItem.CreateConstructButton<FacilityStorageManual>(this)
            //    , UIItem.CreateConstructButton<GrasslandToForest>(this)
            //);
        }

        private uint Moisture() {
            return 20 + HashCode % 60;
        }

        private uint Temporature() {
            return HashCode % 36;
        }
    }
}
