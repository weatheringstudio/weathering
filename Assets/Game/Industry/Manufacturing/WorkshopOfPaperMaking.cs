

using System;

namespace Weathering
{
    [Depend(typeof(DiscardableSolid))]
    public class Paper { }


    [ConstructionCostBase(typeof(Wood), 100)]
    public class WorkshopOfPaperMaking : AbstractFactoryStatic
    {
        public override string SpriteKey => typeof(WorkshopOfWoodcutting).Name;

        protected override (Type, long) In_0_Inventory => (typeof(Worker), 1);

        protected override (Type, long) Out0 => (typeof(Paper), 1);

        protected override (Type, long) In_0 => (typeof(Wood), 1);
    }

}
