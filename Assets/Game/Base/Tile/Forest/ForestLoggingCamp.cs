
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
    [Concept]
    class ForestLoggingCamp : StandardTile
    {
        public override string SpriteKey {
            get {
                if (!Values.Get<Wood>().Maxed) {
                    return typeof(ForestLoggingCamp).Name + "Growing";
                }
                return typeof(ForestLoggingCamp).Name;
            }
        }

        private IValue wood;
        public override void OnConstruct() {
            Values = Weathering.Values.GetOne();

            wood = Values.Create<Level>();

            Inventory = Weathering.Inventory.GetOne();
        }

        public override void OnEnable() {
            base.OnEnable();

            wood = Values.Get<Wood>();
        }



        public override void OnTap() {

        }
    }
}

