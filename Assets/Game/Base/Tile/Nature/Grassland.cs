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
            UI.Ins.ShowItems(TileName,
                // UIItem.CreateButton(gatherFood, PageOfFoodGathering),
                UIItem.CreateConstructButton<Farm>(this),
                UIItem.CreateButton(Localization.Ins.Get<Terraform>(), () => { }, () => false)
            );
        }
    }
}

