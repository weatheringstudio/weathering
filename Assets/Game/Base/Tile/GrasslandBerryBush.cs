
using System;
using System.Collections.Generic;

namespace Weathering
{

    [Concept]
    public class GrasslandBerryBush : StandardTile
    {
        public override string SpriteKey {
            get {
                if (!food.Maxed) {
                    return typeof(GrasslandBerryBush).Name + "Growing";
                }
                return typeof(GrasslandBerryBush).Name;
            }
        }

        private IValue food;

        private static string destruct;
        private static string berryBush;
        public override void OnEnable() {
            base.OnEnable();
            food = Values.Get<Food>();

            destruct = Localization.Ins.Get<Destruct>();
            berryBush = Localization.Ins.Get<GrasslandBerryBush>();
        }

        public override void OnConstruct() {
            Values = Weathering.Values.GetOne();
            food = Values.Create<Food>();
            food.Max = 10;
            food.Inc = 1;
            food.Del = 10 * Value.Second;
        }

        public override void OnDestruct() {
        }

        public override void OnTapPlaySound() {
            Sound.Ins.PlayGrassSound();
        }
        public override void OnTap() {
            const long sanityCost = 1;
            var items = new List<IUIItem>() {
                UIItem.CreateTimeProgress<Food>(Values),
                UIItem.CreateValueProgress<Food>(Values),
                new UIItem {
                    Type = IUIItemType.Button,
                    Content = $"{Localization.Ins.Get<Gather>()}{Localization.Ins.Val<Sanity>(-sanityCost)}",
                    OnTap = () => {
                        Map.Inventory.AddAsManyAsPossible<Food>(food);
                        Globals.Ins.Values.Get<Sanity>().Val -= sanityCost;
                    },
                    CanTap = () => Map.Inventory.CanAdd<Food>() > 0
                        && Globals.Ins.Values.Get<Sanity>().Val >= sanityCost,
                },
            };

            items.Add(UIItem.CreateSeparator());

            items.Add(UIItem.CreateInventoryItem<Food>(Map.Inventory, OnTap));

            // items.Add(UIItem.CreateValueProgress<Sanity>(Globals.Ins.Values));

            items.Add(new UIItem {
                Type = IUIItemType.Button,
                Content = $"{destruct}{berryBush}",
                OnTap = UIDecorator.ConfirmBefore(
                    () => {
                        Map.UpdateAt<Grassland>(Pos);
                        UI.Ins.Active = false;
                    }
                ),
            });

            UI.Ins.ShowItems(berryBush, items);
        }
    }
}

