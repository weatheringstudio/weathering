

using System;
using System.Collections.Generic;

namespace Weathering
{

    public class KnowledgeOfMagnet { }



    [Depend(typeof(Technology))]
    public class KnowledgeOfAncestors { }

    public class TotemOfAncestors : AbstractTechnologyCenter
    {
        public override string SpriteKey => "Totem";

        protected override long Capacity => 100;
        protected override Type TechnologyType => typeof(KnowledgeOfAncestors);

        protected override List<(Type, long)> TechList => new List<(Type, long)> {

            (typeof(KnowledgeOfMagnet), 0), // 磁铁
            (typeof(ResidenceOfGrass), 5), // 房屋
            (typeof(Farm), 5), // 农场
            (typeof(WareHouseOfGrass), 5), // 仓库

            (typeof(RoadForSolid), 100), // 道路

            (typeof(ForestLoggingCamp), 100), // 伐木场
            (typeof(WorkshopOfPaperMaking), 100), // 伐木场
            (typeof(LibraryOfManufacturing), 100), // 图书馆

            (typeof(WorkshopOfWoodcutting), 100), // 木板厂
            (typeof(MountainQuarry), 100), // 采石场
            (typeof(WorkshopOfStonecutting), 100), // 石砖厂
            (typeof(WorkshopOfToolPrimitive), 100), // 石砖厂
        };

        private readonly Type OfferingType = typeof(Grain);
        protected override void DecorateItems(List<IUIItem> items, Action onTap) {
            IValue techValue = Globals.Ins.Values.Get(TechnologyType);
            long quantity = Math.Min(techValue.Max - techValue.Val, Map.Inventory.CanRemove(OfferingType));

            string offeringName = Localization.Ins.ValUnit(OfferingType);

            if (quantity == 0) {
                if (!techValue.Maxed) {
                    items.Add(UIItem.CreateMultilineText($"{Localization.Ins.Get(GetType())}发出了一个声音：“给点{offeringName}吧”"));
                }
            } else {
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
