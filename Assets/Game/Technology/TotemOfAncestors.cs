

using System;
using System.Collections.Generic;

namespace Weathering
{

    public class KnowledgeOfMagnet { }



    [Depend(typeof(Technology))]
    public class KnowledgeOfAncestors { public const long Max = 10000; }

    [ConstructionCostBase(typeof(Grain), 10, 0)]
    public class TotemOfAncestors : AbstractTechnologyCenter
    {
        protected override bool DontConsumeTechnologyPoint => true;

        protected override long TechnologyPointIncRequired => 0;
        protected override Type TechnologyPointType => typeof(KnowledgeOfAncestors);

        protected override List<(Type, long)> TechList => new List<(Type, long)> {

            (typeof(TotemOfAncestors), 0), // 祖先雕像
            (typeof(ResidenceOfGrass), 10), // 房屋
            (typeof(Farm), 10), // 农场

            (typeof(WareHouseOfGrass), 100), // 仓库
            (typeof(RoadForSolid), 200), // 道路

            (typeof(ForestLoggingCamp), 1000), // 伐木场
            (typeof(WorkshopOfPaperMaking), 2000), // 造纸坊
            (typeof(WorkshopOfBook), 5000), // 
            (typeof(LibraryOfAll), 10000), // 图书馆
        };

        private readonly Type OfferingType = typeof(Grain);
        protected override void DecorateItems(List<IUIItem> items, Action onTap) {

            IValue techValue = Globals.Ins.Values.Get(TechnologyPointType);
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
                items.Add(UIItem.CreateDynamicContentButton(() => quantity == 0 ? $"献上{offeringName}" :
                    $"献上{offeringName} {Localization.Ins.ValPlus(OfferingType, -sliderRounded)} {Localization.Ins.ValPlus(TechnologyPointType, sliderRounded)}", () => {

                        Map.Inventory.Remove(OfferingType, sliderRounded);
                        Globals.Ins.Values.Get(TechnologyPointType).Val += sliderRounded;
                        onTap();

                    }, () => sliderRounded > 0));
            }
            items.Add(UIItem.CreateSeparator());
        }
        private float slider = 0;
        private long sliderRounded = 0;
    }
}

