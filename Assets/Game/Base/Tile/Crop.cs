
using System.Collections.Generic;

namespace Weathering
{
    [Concept("农田", "FFDF3B")]
    public class Crop : StandardTile
    {
        // public override string SpriteKey => typeof(Crop).Name;

        public override string SpriteKey {
            get {
                if (food.Maxed) {
                    if (food.Max == 0) {
                        return typeof(Crop).Name;
                    }
                    else {
                        return typeof(Crop).Name + "Ripe";
                    }
                }
                else {
                    return typeof(Crop).Name + "Growing";
                }
            }
        }

        public override void OnConstruct() {
        }

        public override void OnDestruct() {
        }

        private IValue sanity;
        private IValue food;
        private const long productionQuantity = 100;
        public override void OnEnable() {
            base.OnEnable();
            if (Values == null) {
                Values = Weathering.Values.GetOne();
                food = Values.Create<Food>();
                food.Max = 0;
                food.Inc = productionQuantity;
                food.Del = 100 * Value.Second;
            }
            sanity = Globals.Ins.Values.Get<Sanity>();
        }

        public override void OnTap() {
            var items = new List<IUIItem>();

            const long sanityCost = 1;
            const long foodCost = 10;

            items.Add(UIItem.CreateValueProgress<Food>(Values));
            items.Add(UIItem.CreateTimeProgress<Food>(Values));

            items.Add(new UIItem {
                Type = IUIItemType.Button,
                Content = $"播种{Concept.Ins.Val<Sanity>(-sanityCost)}{Concept.Ins.Val<Food>(-foodCost)}",
                OnTap = () => {
                    Map.Inventory.Remove<Food>(foodCost);
                    sanity.Val -= sanityCost;

                    food.Max = productionQuantity;
                },
                CanTap = () => {
                    return food.Max == 0
                        && Map.Inventory.Get<Food>() >= foodCost
                        && sanity.Val >= sanityCost;
                },
            });

            items.Add(new UIItem {
                Type = IUIItemType.Button,
                Content = $"收获{Concept.Ins.Val<Sanity>(-sanityCost)}",
                OnTap = () => {
                    sanity.Val -= sanityCost;
                    long quantity = Map.Inventory.AddAsManyAsPossible<Food>(food);
                    food.Max -= quantity;
                },
                CanTap = () => food.Max > 0
                    && food.Maxed
                    && Map.Inventory.CanAdd<Food>() > 0
                    && sanity.Val >= sanityCost,
            });

            UI.Ins.ShowItems(Concept.Ins.ColoredNameOf<Crop>(), items);
        }
    }
}

