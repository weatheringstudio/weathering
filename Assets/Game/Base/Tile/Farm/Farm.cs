
using System;
using System.Collections.Generic;

namespace Weathering
{
    [Concept]
    public class Farm : StandardTile
    {
        public override string SpriteKey {
            get {
                switch (level.Max) {
                    case 0:
                        return "FarmGrowing";
                    case 1:
                        return "FarmGrowing";
                    default:
                        return "Farm";
                }
            }
        }

        private IValue level;
        public override void OnConstruct() {
            Values = Weathering.Values.GetOne();
            level = Values.Create<Level>();

            Inventory = Weathering.Inventory.GetOne();
            Inventory.QuantityCapacity = 5;
            Inventory.TypeCapacity = 5;
        }

        public override void OnDestruct() {
        }

        private string construct = "fsadf";
        public override void OnEnable() {
            base.OnEnable();
            level = Values.Get<Level>();
            level.Max = -1;
        }
        private const long levelMax = 1;
        private const long workerCost = 1;
        private const long grainRevenue = 3;

        public override void OnTap() {

            var items = new List<IUIItem>() { };

            if (level.Max == -1) {
                items.Add(UIItem.CreateText("田还没开垦"));
                items.Add(UIItem.CreateButton("开垦", () => {
                    level.Max = 0;
                    OnTap();
                }));
            }
            if (level.Max >= 0) {
                items.Add(UIItem.CreateButton($"派遣居民种田{Localization.Ins.Val<Worker>(-1)}{Localization.Ins.Val<GrainSupply>(grainRevenue)}", () => {

                    // 有食物供给。凭空产生
                    // 能塞下谷物
                    if (Map.Inventory.CanAdd<GrainSupply>() < grainRevenue) {
                        UIPreset.InventoryFull(OnTap, Map.Inventory);
                    }

                    // 有工人供给
                    Dictionary<Type, InventoryItemData> workerSupply = new Dictionary<Type, InventoryItemData>();
                    if (Map.Inventory.CanRemoveWithTag<Worker>(workerSupply, workerCost) < workerCost) {
                        UIPreset.ResourceInsufficientWithTag<Worker>(OnTap, workerCost, Map.Inventory);
                        return;
                    }
                    // 能装下工人
                    if (!Inventory.CanAddWithTag(workerSupply, workerCost)) {
                        UIPreset.InventoryFull(OnTap, Inventory);
                        return;
                    }

                    Inventory.AddFromWithTag<Worker>(Map.Inventory, workerCost);
                    Map.Inventory.Add<GrainSupply>(grainRevenue);

                    level.Max++;
                    OnTap();

                }, () => level.Max < levelMax));
                items.Add(UIItem.CreateButton($"取消居民种田{Localization.Ins.Val<GrainSupply>(-grainRevenue)}", () => {

                    // 能塞下谷物。可以，凭空消失

                    // 有谷物。只能是谷物啊
                    if (Map.Inventory.CanRemove<GrainSupply>() < grainRevenue) {
                        UIPreset.ResourceInsufficient<GrainSupply>(OnTap, grainRevenue, Map.Inventory);
                    }

                    // 有工人/农民，肯定有的
                    Dictionary<Type, InventoryItemData> workerSupply = new Dictionary<Type, InventoryItemData>();
                    if (Inventory.CanRemoveWithTag<Worker>(workerSupply, workerCost) < workerCost) {
                        UIPreset.ResourceInsufficientWithTag<Worker>(OnTap, workerCost, Inventory);
                    }
                    
                    // 能够存放工人/农民
                    if (!Map.Inventory.CanAddWithTag(workerSupply, workerCost)) {
                        UIPreset.InventoryFull(OnTap, Map.Inventory);
                    }

                    Map.Inventory.Remove<GrainSupply>(grainRevenue);
                    Map.Inventory.AddFromWithTag<Worker>(Inventory, workerCost);

                    level.Max--;
                    OnTap();

                }, () => level.Max > 0));
            }


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

