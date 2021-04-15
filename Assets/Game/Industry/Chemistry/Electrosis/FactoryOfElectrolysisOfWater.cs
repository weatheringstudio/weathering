

using System;

namespace Weathering
{
    // 氢气
    [Depend(typeof(DiscardableFluid))]
    public class Hydrogen { }

    public class FactoryOfElectrolysis { }

    public class FactoryOfElectrolysisOfWater : AbstractFactoryStatic
    {
        public override string SpriteKey => DecoratedSpriteKey(typeof(FactoryOfElectrolysis).Name);

        protected override (Type, long) In_0_Inventory => (typeof(Electricity), 30);

        protected override (Type, long) In_0 => (typeof(Water), 2);
        protected override (Type, long) Out0 => (typeof(Hydrogen), 2);
        protected override (Type, long) Out1 => (typeof(Oxygen), 1);
    }
}
