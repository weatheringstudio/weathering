
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
    [Concept]
    public class FlowerGarden : StandardTile
    {
        public override string SpriteKey => typeof(FlowerGarden).Name;

        private IValue flower;

        public override void OnEnable() {
            base.OnEnable();
            flower = Values.Get<Flower>();
        }

        public override void OnConstruct() {
            Values = Weathering.Values.GetOne();
            flower = Values.Create<Flower>();
            flower.Max = 10;
            flower.Inc = 1;
            flower.Del = 10 * Value.Second;
        }

        public override void OnTap() {
            const long sanityCost = 1;
            var items = new List<IUIItem>() {
                UIItem.CreateTimeProgress<Flower>(Values),
                UIItem.CreateValueProgress<Flower>(Values),
                new UIItem {
                    Type = IUIItemType.Button,
                    Content = $"{Localization.Ins.Get<Gather>()}{Localization.Ins.Get<Flower>()}{Localization.Ins.Val<Sanity>(-sanityCost)}",
                    OnTap = () => {
                        Map.Inventory.AddAsManyAsPossible<Flower>(flower);
                        Globals.Ins.Values.Get<Sanity>().Val -= sanityCost;
                    },
                    CanTap = () => Map.Inventory.CanAdd<Flower>() > 0
                        && Globals.Ins.Values.Get<Sanity>().Val >= sanityCost,
                },
            };

            items.Add(UIItem.CreateSeparator());

            UIItem.AddInventoryItem<Flower>(Map.Inventory, items);

            items.Add(new UIItem {
                Type = IUIItemType.Button,
                Content = $"{Localization.Ins.Get<Destruct>()}{Localization.Ins.Get<FlowerGarden>()}",
                OnTap = UIDecorator.ConfirmBefore(
                    () => {
                        Map.UpdateAt<Grassland>(Pos);
                        UI.Ins.Active = false;
                    }
                ),
            });

            UI.Ins.ShowItems(Localization.Ins.Get<FlowerGarden>(), items);
        }
    }
}

