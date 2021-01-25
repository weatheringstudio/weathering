
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
    [Concept]
    public class VegetableGarden : StandardTile
    {
        public override string SpriteKey => typeof(VegetableGarden).Name;

        private IValue vegetable;

        public override void OnEnable() {
            base.OnEnable();
            vegetable = Values.Get<Vegetable>();
        }

        public override void OnConstruct() {
            Values = Weathering.Values.GetOne();
            vegetable = Values.Create<Vegetable>();
            vegetable.Max = 10;
            vegetable.Inc = 1;
            vegetable.Del = 10 * Value.Second;
        }

        public override void OnTapPlaySound() {
            Sound.Ins.PlayGrassSound();
        }
        public override void OnTap() {
            const long sanityCost = 1;
            var items = new List<IUIItem>() {
                UIItem.CreateTimeProgress<Vegetable>(Values),
                UIItem.CreateValueProgress<Vegetable>(Values),
                new UIItem {
                    Type = IUIItemType.Button,
                    Content = $"{Localization.Ins.Get<Gather>()}{Localization.Ins.Get<Vegetable>()}{Localization.Ins.Val<Sanity>(-sanityCost)}",
                    OnTap = () => {
                        Map.Inventory.AddAsManyAsPossible<Vegetable>(vegetable);
                        Globals.Ins.Values.Get<Sanity>().Val -= sanityCost;
                    },
                    CanTap = () => Map.Inventory.CanAdd<Vegetable>() > 0
                        && Globals.Ins.Values.Get<Sanity>().Val >= sanityCost,
                },
            };

            items.Add(UIItem.CreateSeparator());

            items.Add(UIItem.CreateInventoryItem<Vegetable>(Map.Inventory, OnTap));

            items.Add(new UIItem {
                Type = IUIItemType.Button,
                Content = $"{Localization.Ins.Get<Destruct>()}{Localization.Ins.Get<VegetableGarden>()}",
                OnTap = UIDecorator.ConfirmBefore(
                    () => {
                        Map.UpdateAt<Grassland>(Pos);
                        UI.Ins.Active = false;
                    }
                ),
            });

            UI.Ins.ShowItems(Localization.Ins.Get<VegetableGarden>(), items);
        }
    }
}

