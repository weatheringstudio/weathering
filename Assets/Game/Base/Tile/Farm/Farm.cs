
using System.Collections.Generic;

namespace Weathering
{
    [Concept]
    public class Farm : StandardTile
    {

        public override string SpriteKey {
            get {
                return typeof(Farm).Name;
            }
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

        private IValue food;
        private static string construct;

        public override void OnEnable() {
            base.OnEnable();
            food = Values.Get<Food>();

            construct = Localization.Ins.Get<Construct>();
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
                    Content = $"{Localization.Ins.Get<Gather>()}{Localization.Ins.NoVal<Fruit>()}{Localization.Ins.Val<Sanity>(-sanityCost)}",
                    OnTap = () => {
                        Map.Inventory.AddAsManyAsPossible<Fruit>(food);
                        Globals.Ins.Values.Get<Sanity>().Val -= sanityCost;
                    },
                    CanTap = () => Map.Inventory.CanAdd<Fruit>() > 0
                        && Globals.Ins.Values.Get<Sanity>().Val >= sanityCost,
                },
            };

            items.Add(UIItem.CreateSeparator());

            items.Add(new UIItem {
                Type = IUIItemType.Button,
                Content = Localization.Ins.Get<Construct>(),
                OnTap = BuildPage,
            });
            items.Add(new UIItem {
                Type = IUIItemType.Button,
                Content = Localization.Ins.Get<Destruct>(),
                OnTap = () => {
                    Map.UpdateAt<Grassland>(Pos);
                    UI.Ins.Active = false;
                },
            });

            UI.Ins.ShowItems(Localization.Ins.Get<Farm>(), items);
        }

        private void BuildPage() {
            var items = new List<IUIItem>();

            items.Add(new UIItem {
                Type = IUIItemType.Button,
                Content = $"{construct}{Localization.Ins.Get<GrainFarm>()}",
                OnTap = () => {
                    Map.UpdateAt<GrainFarm>(Pos);
                    Map.Get(Pos).OnTap();
                },
            });

            items.Add(new UIItem {
                Type = IUIItemType.Button,
                Content = $"{construct}{Localization.Ins.Get<FlowerGarden>()}",
                OnTap = () => {
                    Map.UpdateAt<FlowerGarden>(Pos);
                    Map.Get(Pos).OnTap();
                },
            });

            items.Add(new UIItem {
                Type = IUIItemType.Button,
                Content = $"{construct}{Localization.Ins.Get<VegetableGarden>()}",
                OnTap = () => {
                    Map.UpdateAt<VegetableGarden>(Pos);
                    Map.Get(Pos).OnTap();
                },
            });

            items.Add(new UIItem {
                Type = IUIItemType.Button,
                Content = $"{construct}{Localization.Ins.Get<FruitGarden>()}",
                OnTap = () => {
                    Map.UpdateAt<FruitGarden>(Pos);
                    Map.Get(Pos).OnTap();
                },
            });

            items.Add(new UIItem {
                Type = IUIItemType.Button,
                Content = $"{construct}{Localization.Ins.Get<Plantation>()}",
                OnTap = () => { },
                CanTap = () => false,
            });

            UI.Ins.ShowItems(construct, items);
        }
    }
}

