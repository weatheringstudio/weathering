
using System;
using System.Collections.Generic;

namespace Weathering
{

    [Concept("浆果丛", "ff9999")]
    public class BerryBush : StandardTile {
        public override string SpriteKey {
            get {
                if (!Values.Get<Food>().Maxed) {
                    return typeof(BerryBush).Name + "Growing";
                }
                return typeof(BerryBush).Name;
            }
        }

        private IValue food;
        public override void OnEnable() {
            base.OnEnable();
            if (Values == null) {
                Values = Weathering.Values.Create();
                food = Values.Create<Food>();
                food.Max = 10;
                food.Inc = 1;
                food.Del = 10 * Value.Second;
            }
            food = Values.Get<Food>();
        }

        public override void OnConstruct() {
        }

        public override void OnDestruct() {
        }

        public override void OnTap() {
            var items = new List<IUIItem>() {
                UIItem.CreateValueProgress<Food>(Values),
                UIItem.CreateTimeProgress<Food>(Values),
                new UIItem {
                    Type = IUIItemType.Button,
                    Content = $"拿走食材{Concept.Ins.Val<Sanity>(-1)}",
                    OnTap = () => {
                        Map.Inventory.AddAsManyAsPossible<Food>(food);
                        Globals.Ins.Values.Get<Sanity>().Val -= 1;
                    },
                    CanTap = () => Map.Inventory.CanAdd<Food>() >= 1
                            && food.Val >= 1
                            && Globals.Ins.Values.Get<Sanity>().Val >= 1,
                },
            };

            UIItem.AddSeparator(items);

            UIItem.AddInventory<Food>(Map.Inventory, items);
            //items.Add(UIItem.CreateValueProgress<Sanity>(Globals.Ins.Values));
            //UIItem.AddInventoryInfo(Map.Inventory, items);

            UI.Ins.ShowItems(Concept.Ins.ColoredNameOf<BerryBush>(), items);
        }
    }
}

