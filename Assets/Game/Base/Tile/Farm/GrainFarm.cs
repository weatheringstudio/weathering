
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
    [Concept]
	public class GrainFarm : StandardTile
	{

        public override string SpriteKey {
            get {
                if (food.Maxed) {
                    if (food.Max == 0) {
                        return typeof(Farm).Name;
                    } else {
                        return typeof(Farm).Name + "Ripe";
                    }
                } else {
                    return typeof(Farm).Name + "Growing";
                }
            }
        }

        public override void OnConstruct() {
            Values = Weathering.Values.GetOne();
            food = Values.Create<Grain>();
            food.Max = 0;
            food.Inc = productionQuantity;
            food.Del = 100 * Value.Second;
        }

        public override void OnDestruct() {
        }

        private IValue food;
        private IValue sanity;
        private static string construct;

        private const long productionQuantity = 100;
        public override void OnEnable() {
            base.OnEnable();
            sanity = Globals.Ins.Values.Get<Sanity>();
            food = Values.Get<Grain>();

            construct = Localization.Ins.Get<Construct>();
        }

        public override void OnTapPlaySound() {
            Sound.Ins.PlayGrassSound();
        }
        public override void OnTap() {
            var items = new List<IUIItem>();

            const long sanityCost = 1;
            const long foodCost = 10;

            items.Add(UIItem.CreateTimeProgress<Grain>(Values));
            items.Add(UIItem.CreateValueProgress<Grain>(Values));

            items.Add(new UIItem {
                Type = IUIItemType.Button,
                Content = $"{Localization.Ins.Get<Sow>()}{Localization.Ins.Val<Sanity>(-sanityCost)}{Localization.Ins.Val<Grain>(-foodCost)}",
                OnTap = () => {
                    Map.Inventory.Remove<Grain>(foodCost);
                    sanity.Val -= sanityCost;

                    food.Max = productionQuantity;
                },
                CanTap = () => {
                    return food.Max == 0
                        && Map.Inventory.Get<Grain>() >= foodCost
                        && sanity.Val >= sanityCost;
                },
            });

            items.Add(new UIItem {
                Type = IUIItemType.Button,
                Content = $"{Localization.Ins.Get<Harvest>()}{Localization.Ins.Val<Sanity>(-sanityCost)}",
                OnTap = () => {
                    sanity.Val -= sanityCost;
                    long quantity = Map.Inventory.AddAsManyAsPossible<Grain>(food);
                    food.Max -= quantity;
                },
                CanTap = () => food.Max > 0
                    && food.Maxed
                    && Map.Inventory.CanAdd<Grain>() > 0
                    && sanity.Val >= sanityCost,
            });

            items.Add(UIItem.CreateSeparator());

            items.Add(new UIItem {
                Type = IUIItemType.Button,
                Content = Localization.Ins.Get<Destruct>(),
                OnTap = () => {
                    Map.UpdateAt<Grassland>(Pos);
                    UI.Ins.Active = false;
                },
            });

            UI.Ins.ShowItems(Localization.Ins.Get<GrainFarm>(), items);
        }
    }
}

