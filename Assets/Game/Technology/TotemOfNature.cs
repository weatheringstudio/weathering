

using System;
using System.Collections.Generic;

namespace Weathering
{


    public class KnowledgeOfGatheringBerry { }
    public class KnowledgeOfGatheringBerryEfficiently { }

    public class KnowledgeOfHammer { }

    // BerryBush

    public class KnowledgeOfGatheringWood { }
    public class KnowledgeOfGatheringWoodEfficiently { }

    // WareHouse




    [Depend(typeof(Technology))]
    public class KnowledgeOfNature { }

    public class TotemOfNature : AbstractTechnologyCenter
    {
        public override string SpriteKey => "Totem";

        protected override Type TechnologyType => typeof(KnowledgeOfNature);

        protected override List<(Type, long)> TechList => new List<(Type, long)> {

            // nature
            (typeof(KnowledgeOfGatheringBerry), 0), // 采集
            (typeof(KnowledgeOfGatheringBerryEfficiently), 5), // 高效采集
            (typeof(BerryBush), 20), // 浆果丛
            (typeof(KnowledgeOfHammer), 50), // 工具：锤子
            (typeof(TotemOfAncestors), 100), // 祖先雕像

        };

        protected override void DecorateIfCompleted(List<IUIItem> items) {
            
        }

        private readonly Type OfferingType = typeof(Berry);
        protected override void DecorateItems(List<IUIItem> items, Action onTap) {
            IValue techValue = Globals.Ins.Values.Get(TechnologyType);
            long quantity = Math.Min(techValue.Max - techValue.Val, Map.Inventory.CanRemove(OfferingType));

            string offeringName = Localization.Ins.ValUnit(OfferingType);

            if (quantity == 0) {
                if (!techValue.Maxed) {
                    items.Add(UIItem.CreateMultilineText($"{Localization.Ins.Get(GetType())}发出了一个声音：“给点{offeringName}吧”"));
                }
            }
            else {
                items.Add(new UIItem {
                    Type = IUIItemType.Slider,
                    InitialSliderValue = 1,
                    DynamicSliderContent = (float x) => {
                        slider = x;
                        sliderRounded = (long)Math.Round(slider * quantity);
                        return $"选择贡献数量 {sliderRounded}";
                    }
                });
                items.Add(UIItem.CreateStaticButton(quantity == 0 ? $"献上{offeringName}" :
                    $"献上{offeringName} {Localization.Ins.ValPlus(OfferingType, -quantity)} {Localization.Ins.ValPlus(TechnologyType, quantity)}", () => {

                        Map.Inventory.Remove(OfferingType, quantity);
                        Globals.Ins.Values.Get(TechnologyType).Val += quantity;
                        onTap();
                    }, quantity > 0));
            }

        }
        private float slider = 0;
        private long sliderRounded = 0;
    }
}
