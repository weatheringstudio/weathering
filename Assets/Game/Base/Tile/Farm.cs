
using System.Collections.Generic;

namespace Weathering
{
    [Concept("农田", "FFDF3B")]
    public class Farm : StandardTile
    {
        // public override string SpriteKey => typeof(Crop).Name;

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
            food = Values.Create<Food>();
            food.Max = 0;
            food.Inc = productionQuantity;
            food.Del = 100 * Value.Second;

            level = Values.Create<Level>();
        }

        public override void OnDestruct() {
        }

        private IValue food;
        private IValue level;
        private IValue sanity;
        private static string construct;

        private const long productionQuantity = 100;
        public override void OnEnable() {
            base.OnEnable();
            sanity = Globals.Ins.Values.Get<Sanity>();
            food = Values.Get<Food>();
            level = Values.Get<Level>();

            construct = Concept.Ins.ColoredNameOf<Construct>();
        }

        public override void OnTap() {
            var items = new List<IUIItem>();

            const long sanityCost = 1;
            const long foodCost = 10;

            if (level.Val == 0) {
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
            } else {

            }

            items.Add(new UIItem {
                Type = IUIItemType.Button,
                Content = $"升级",
                OnTap = FarmUpgradePage,
            });
            items.Add(new UIItem {
                Type = IUIItemType.Button,
                Content = $"{Concept.Ins.ColoredNameOf<Destruct>()}",
                OnTap = () => {
                    Map.UpdateAt<Grassland>(Pos);
                    UI.Ins.Active = false;
                },
            });

            UI.Ins.ShowItems(Concept.Ins.ColoredNameOf<Farm>(), items);
        }

        private void FarmUpgradePage() {
            var items = new List<IUIItem>();

            items.Add(new UIItem {
                Type = IUIItemType.Button,
                Content = $"{construct}{Concept.Ins.ColoredNameOf<Plantation>()}",
                OnTap = () => { },
                CanTap = () => false,
            });

            items.Add(new UIItem {
                Type = IUIItemType.Button,
                Content = $"{construct}{Concept.Ins.ColoredNameOf<VegetablesGarden>()}",
                OnTap = () => { },
                CanTap = () => false,
            });


            UI.Ins.ShowItems(construct, items);
        }
    }
}

