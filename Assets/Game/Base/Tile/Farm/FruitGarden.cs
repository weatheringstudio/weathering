
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
    [Concept]
    public class FruitGarden : StandardTile
    {
        public override string SpriteKey => typeof(FruitGarden).Name;

        private IValue fruit;

        public override void OnEnable() {
            base.OnEnable();
            fruit = Values.Get<Fruit>();
        }

        public override void OnConstruct() {
            Values = Weathering.Values.GetOne();
            fruit = Values.Create<Fruit>();
            fruit.Max = 10;
            fruit.Inc = 1;
            fruit.Del = 10 * Value.Second;
        }


        public override void OnTapPlaySound() {
            Sound.Ins.PlayGrassSound();
        }
        public override void OnTap() {
            const long sanityCost = 1;
            var items = new List<IUIItem>() {
                UIItem.CreateTimeProgress<Fruit>(Values),
                UIItem.CreateValueProgress<Fruit>(Values),
                new UIItem {
                    Type = IUIItemType.Button,
                    Content = $"{Localization.Ins.Get<Gather>()}{Localization.Ins.Get<Fruit>()}{Localization.Ins.Val<Sanity>(-sanityCost)}",
                    OnTap = () => {
                        Map.Inventory.AddFrom<Fruit>(fruit);
                        Globals.Ins.Values.Get<Sanity>().Val -= sanityCost;
                    },
                    CanTap = () => Map.Inventory.CanAdd<Fruit>() > 0
                        && Globals.Ins.Values.Get<Sanity>().Val >= sanityCost,
                },
            };

            items.Add(UIItem.CreateSeparator());

            items.Add(UIItem.CreateInventoryItem<Fruit>(Map.Inventory, OnTap));

            items.Add(new UIItem {
                Type = IUIItemType.Button,
                Content = $"{Localization.Ins.Get<Destruct>()}{Localization.Ins.Get<FruitGarden>()}",
                OnTap = UIDecorator.ConfirmBefore(
                    () => {
                        Map.UpdateAt<Grassland>(Pos);
                        UI.Ins.Active = false;
                    }
                ),
            });

            UI.Ins.ShowItems(Localization.Ins.Get<FruitGarden>(), items);
        }
    }
}

