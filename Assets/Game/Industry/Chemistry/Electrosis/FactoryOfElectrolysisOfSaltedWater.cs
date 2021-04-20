

using System;

namespace Weathering
{

    // 氯气
    [Depend(typeof(DiscardableFluid))]
    public class Chlorine { }



    // 氢氧化钠
    [Depend(typeof(DiscardableFluid))]
    public class SodiumHydroxide { }


    public class FactoryOfElectrolysisOfSaltedWater : AbstractFactoryStatic
    {
        public override string SpriteKey => typeof(FactoryOfElectrolysis).Name;

        protected override (Type, long) In_0_Inventory => (typeof(Electricity), 30);

        protected override (Type, long) In_0 => (typeof(SeaWater), 2);
        protected override (Type, long) Out0 => (typeof(Hydrogen), 1);
        protected override (Type, long) Out1 => (typeof(Chlorine), 1);
        protected override (Type, long) Out2 => (typeof(SodiumHydroxide), 1);
    }
}
