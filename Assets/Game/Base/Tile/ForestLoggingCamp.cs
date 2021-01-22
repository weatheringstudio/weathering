
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

        private static string destruct;
        private static string forestLoggingCamp;
        public override void OnEnable() {
            base.OnEnable();

            wood = Values.Get<Wood>();

            destruct = Localization.Ins.Get<Destruct>();
            forestLoggingCamp = Localization.Ins.Get<ForestLoggingCamp>();
        }

        public override void OnConstruct() {
            Values = Weathering.Values.GetOne();
            wood = Values.Create<Wood>();
            wood.Max = 10;
            wood.Inc = 1;
            wood.Del = 10 * Value.Second;
        }

        public override void OnTap() {
            const long sanityCost = 1;
            var items = new List<IUIItem>() {
                UIItem.CreateTimeProgress<Wood>(Values),
                UIItem.CreateValueProgress<Wood>(Values),
                new UIItem {
                    Type = IUIItemType.Button,
                    Content = $"拿走木材{Localization.Ins.Val<Sanity>(-sanityCost)}",
                    OnTap = () => {
                        Map.Inventory.AddAsManyAsPossible<Wood>(wood);
                        Globals.Ins.Values.Get<Sanity>().Val -= sanityCost;
                    },
                    CanTap = () => Map.Inventory.CanAdd<Wood>() > 0
                        && Globals.Ins.Values.Get<Sanity>().Val >= sanityCost,
                },
            };

            items.Add(UIItem.CreateSeparator());

            UIItem.AddInventoryItem<Wood>(Map.Inventory, items);

            items.Add(new UIItem {
                Type = IUIItemType.Button,
                Content = $"{destruct}{forestLoggingCamp}",
                OnTap = UIDecorator.ConfirmBefore(
                    () => {
                        Map.UpdateAt<Forest>(Pos);
                        UI.Ins.Active = false;
                    }
                ),
            });

            UI.Ins.ShowItems(forestLoggingCamp, items);
        }
    }
}

