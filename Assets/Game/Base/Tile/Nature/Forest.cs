
using System;
using System.Collections.Generic;

namespace Weathering
{
    [Concept]
    public class Forest : StandardTile
    {
        public override string SpriteKey => typeof(Forest).Name;


        private static bool initialized = false;
        private static string gatherWood;

        public override void OnEnable() {
            base.OnEnable();

            if (!initialized) {
                initialized = true;
                gatherWood = $"{Localization.Ins.Get<Gather>()}{Localization.Ins.Get<Wood>()}";
            }
        }

        public override void OnTap() {
            UI.Ins.ShowItems(Localization.Ins.Get<Forest>(),
                UIItem.CreateButton(gatherWood, PageOfWoodGathering),
                UIItem.CreateConstructButton<HuntingGround>(this),
                new UIItem {
                    Type = IUIItemType.Button,
                    Content = Localization.Ins.Get<Deforestation>(),
                    CanTap = () => false,
                }
            );
        }

        private void PageOfWoodGathering() {

            const long sanityCost = 1;
            const long woodRevenue = 1;
            var items = new List<IUIItem>() {
                UIItem.CreateMultilineText($"森林里面有木材{Localization.Ins.Val<Wood>(woodRevenue)}{Localization.Ins.Val<Sanity>(-sanityCost)}"),
                new UIItem {
                    Type = IUIItemType.Button,
                    Content = gatherWood,
                    OnTap = () => {
                        Map.Inventory.Add<Wood>(woodRevenue);
                        Globals.Ins.Values.GetOrCreate<Sanity>().Val -= sanityCost;
                    },
                    CanTap = () => Map.Inventory.CanAdd<Wood>() > woodRevenue
                    && Globals.Ins.Values.GetOrCreate<Sanity>().Val >= sanityCost,
                },
            };

            items.Add(UIItem.CreateSeparator());

            items.Add(UIItem.CreateInventoryTitle());
            items.Add(UIItem.CreateValueProgress<Sanity>(Globals.Ins.Values));
            items.Add(UIItem.CreateInventoryItem<Wood>(Map.Inventory));
            items.Add(UIItem.CreateInventoryCapacity(Map.Inventory));
            items.Add(UIItem.CreateInventoryTypeCapacity(Map.Inventory));

            UI.Ins.ShowItems(gatherWood, items);
        }
    }
}

