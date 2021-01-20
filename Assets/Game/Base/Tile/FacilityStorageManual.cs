
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
    [Concept("私人仓库")]
    public class FacilityStorageManual : StandardTile
    {
        public override string SpriteKey => "StorageBuilding";

        private string facilityStorageManual;
        public override void OnEnable() {
            base.OnEnable();

            facilityStorageManual = Concept.Ins.ColoredNameOf<FacilityStorageManual>();
        }

        public override void OnConstruct() {
        }

        public override void OnDestruct() {
        }

        public override void OnTap() {
            var items = new List<IUIItem>();

            UIItem.AddText("私人仓库，容量很大");

            items.Add(new UIItem {
                Type = IUIItemType.Button,
                Content = "储存物品",
            });

            UI.Ins.ShowItems(facilityStorageManual, items);
        }
    }
}

