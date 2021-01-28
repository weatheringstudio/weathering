
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
            UI.Ins.ShowItems(Localization.Ins.Get<Grassland>(),

                UIItem.CreateText($"温度{Temporature()} 湿度{Moisture()} (气候暂时对游戏没有作用)"),

                UIItem.CreateShortcutOfConstructionButton(this),
                UIItem.CreateConstructButton<Farm>(this),
                UIItem.CreateConstructButton<Village>(this),
                UIItem.CreateConstructButton<FacilityStorageManual>(this),
                UIItem.CreateButton(Localization.Ins.Get<Terraform>(), () => { }, () => false)
            );
        }

        private uint Moisture() {
            return HashCode % 100;
        }

        private uint Temporature() {
            return HashCode % 36;
        }
    }
}

