

using System;

namespace Weathering
{

    // 纯净水
    [Depend(typeof(DiscardableFluid))]
    public class Water { }


    public class FactoryOfDesalination : AbstractFactoryStatic
    {
        public override string SpriteKey => typeof(FactoryOfElectrolysis).Name;

        protected override (Type, long) In_0_Inventory => (typeof(Electricity), 2);

        protected override (Type, long) In_0 => (typeof(SeaWater), 1);
        protected override (Type, long) Out0 => (typeof(Water), 1);
    }
}
