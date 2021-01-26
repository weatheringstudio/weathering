
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
    public class Workshop : StandardTile
    {
        public override string SpriteKey => typeof(Workshop).Name;


        private IValue handicraft;

        public override void OnEnable() {
            base.OnEnable();
            handicraft = Values.Get<WorkshopProduct>();
        }

        public override void OnConstruct() {
            Values = Weathering.Values.GetOne();
            handicraft = Values.Create<WorkshopProduct>();
            handicraft.Max = 10;
            handicraft.Inc = 1;
            handicraft.Del = 10 * Value.Second;
        }
        public override void OnTap() {
            const long sanityCost = 1;
            var items = new List<IUIItem>() {
                UIItem.CreateTimeProgress<WorkshopProduct>(Values),
                UIItem.CreateValueProgress<WorkshopProduct>(Values),
                new UIItem {
                    Type = IUIItemType.Button,
                    Content = $"{Localization.Ins.Get<Gather>()}{Localization.Ins.Get<WorkshopProduct>()}{Localization.Ins.Val<Sanity>(-sanityCost)}",
                    OnTap = () => {
                        Map.Inventory.AddFrom<WorkshopProduct>(handicraft);
                        Globals.Ins.Values.Get<Sanity>().Val -= sanityCost;
                    },
                    CanTap = () => Map.Inventory.CanAdd<WorkshopProduct>() > 0
                        && Globals.Ins.Values.Get<Sanity>().Val >= sanityCost,
                },
            };

            items.Add(UIItem.CreateSeparator());

            items.Add(UIItem.CreateInventoryItem<WorkshopProduct>(Map.Inventory, OnTap));

            items.Add(new UIItem {
                Type = IUIItemType.Button,
                Content = $"{Localization.Ins.Get<Destruct>()}{Localization.Ins.Get<Workshop>()}",
                OnTap = UIDecorator.ConfirmBefore(
                    () => {
                        Map.UpdateAt<Grassland>(Pos);
                        UI.Ins.Active = false;
                    }
                ),
            });

            UI.Ins.ShowItems(Localization.Ins.Get<Workshop>(), items);
        }
    }
}

