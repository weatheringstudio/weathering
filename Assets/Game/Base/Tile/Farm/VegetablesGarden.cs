
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
    [Concept]
    public class VegetablesGarden : StandardTile
    {
        public override string SpriteKey => typeof(Farm).Name;

        public override void OnConstruct() {
        }

        public override void OnDestruct() {
        }

        public override void OnTap() {
            var items = new List<IUIItem>();



            UI.Ins.ShowItems(Localization.Ins.Get<VegetablesGarden>(), items);
        }
    }
}

