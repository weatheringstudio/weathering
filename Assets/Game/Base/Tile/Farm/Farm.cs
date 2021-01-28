
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
            level.Max = -1;

            Inventory = Weathering.Inventory.GetOne();
            Inventory.QuantityCapacity = 5;
            Inventory.TypeCapacity = 5;
        }

        public override void OnDestruct() {
        }

        public override void OnEnable() {
            base.OnEnable();
            level = Values.Get<Level>();
        }

        public override void OnTap() {

            InventoryQuery sow = InventoryQuery.Create(OnTap, Map.Inventory
                , new InventoryQueryItem { Quantity = 1, Type = typeof(Worker), Source = Map.Inventory, Target = Inventory }
                , new InventoryQueryItem { Quantity = 3, Type = typeof(GrainSupply), Target = Map.Inventory });
            InventoryQuery sowInvsersed = sow.CreateInversed();


            var items = new List<IUIItem>() { };

            if (level.Max == -1) {
                InventoryQuery build = InventoryQuery.Create(OnTap, Map.Inventory
                    , new InventoryQueryItem { Quantity = 10, Type = typeof(Food), Source = Map.Inventory });


                items.Add(UIItem.CreateText("田还没开垦"));
                items.Add(UIItem.CreateButton($"开垦{build.GetDescription()}", () => {
                    build.TryDo(() => {
                        level.Max = 0;
                    });
                }));

                Type grasslandType = typeof(Grassland);
                items.Add(UIItem.CreateConstructButton<GrainFarm>(this, grasslandType));
                items.Add(UIItem.CreateConstructButton<FlowerGarden>(this, grasslandType));
                items.Add(UIItem.CreateConstructButton<VegetableGarden>(this, grasslandType));
                items.Add(UIItem.CreateConstructButton<FruitGarden>(this, grasslandType));
                items.Add(UIItem.CreateConstructButton<Plantation>(this, grasslandType));

            } else if (level.Max == 0) {
                items.Add(UIItem.CreateButton($"派遣居民种田{sow.GetDescription()}", () => {
                    sow.TryDo(() => {
                        level.Max = 1;
                    });
                }));

            } else if (level.Max == 1) {
                items.Add(UIItem.CreateButton($"取消居民种田{sowInvsersed.GetDescription()}", () => {
                    sowInvsersed.TryDo(() => {
                        level.Max = 0;
                    });
                }));
            }

            items.Add(UIItem.CreateDestructButton<Grassland>(this));


            UI.Ins.ShowItems(Localization.Ins.Get<Farm>(), items);
        }
    }
}

