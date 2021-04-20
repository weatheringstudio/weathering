

using System;

namespace Weathering
{
    // 木板
    [Depend(typeof(DiscardableSolid))]
    public class WoodPlank { }


    public class Workshop { }


    [ConstructionCostBase(typeof(Wood), 100)]
    public class WorkshopOfWoodcutting : AbstractFactoryStatic
    {
        public override string SpriteKey => typeof(Workshop).Name;

        protected override (Type, long) In_0_Inventory => (typeof(Worker), 1);

        protected override (Type, long) Out0 => (typeof(WoodPlank), 1);

        protected override (Type, long) In_0 => (typeof(Wood), 2);
    }
}
