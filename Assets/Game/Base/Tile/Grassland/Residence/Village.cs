
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
    public class Village : StandardTile
    {
        public override string SpriteKey => typeof(Village).Name;

        public override void OnTap() {
            var items = UI.Ins.GetItems();



            UI.Ins.ShowItems("村庄", items);
        }
    }
}

