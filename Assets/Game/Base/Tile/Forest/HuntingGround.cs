
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
    [Concept]
    public class HuntingGround : StandardTile
    {
        public override string SpriteKey => typeof(HuntingGround).Name;


        private IValue food;

        public override void OnEnable() {
            base.OnEnable();


        }

        public override void OnTap() {
            var items = new List<IUIItem>();


            UI.Ins.ShowItems(Localization.Ins.Get<HuntingGround>(), items);
        }
    }
}

