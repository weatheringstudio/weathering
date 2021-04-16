

using System;
using System.Collections.Generic;

namespace Weathering
{


    public class KnowledgeOfGatheringBerry { }
    public class KnowledgeOfGatheringBerryEfficiently { }

    public class KnowledgeOfGatheringWood { }
    public class KnowledgeOfGatheringWoodEfficiently { }

    // BerryBush

    // WareHouse



    [Depend(typeof(Technology))]
    public class KnowledgeOfNature { }

    public class Totem : AbstractTechnologyCenter
    {
        public override string SpriteKey => "Totem";

        protected override Type TechnologyType => typeof(KnowledgeOfNature);

        protected override List<(Type, long)> TechList => new List<(Type, long)> {
            (typeof(KnowledgeOfGatheringBerry), 0),
            (typeof(KnowledgeOfGatheringBerryEfficiently), 10),
            (typeof(BerryBush), 10),
            (typeof(WareHouseOfGrass), 10),
        };

        protected override void DecorateItems(List<IUIItem> items, Action onTap) {
            IValue techValue = Globals.Ins.Values.Get(TechnologyType);
            long quantity = Math.Min(techValue.Max - techValue.Val, Map.Inventory.CanRemove<Berry>());


            items.Add(UIItem.CreateStaticButton(quantity == 0 ? "献上浆果" : 
                $"献上浆果 {Localization.Ins.ValPlus<Berry>(-quantity)} {Localization.Ins.ValPlus<KnowledgeOfNature>(quantity)}", () => {

                Map.Inventory.Remove<Berry>(quantity);
                Globals.Ins.Values.Get(TechnologyType).Val += quantity;
                onTap();
            }, quantity > 0));
        }
    }
}
