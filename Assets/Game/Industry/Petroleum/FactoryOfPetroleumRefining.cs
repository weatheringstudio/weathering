

using System;

namespace Weathering
{
    // 重油
    [Depend(typeof(DiscardableFluid))]
    [Concept]
    public class HeavyOil { }

    // 轻油
    [Depend(typeof(DiscardableFluid))]
    [Concept]
    public class LightOil { }

    // 石油气
    [Depend(typeof(DiscardableFluid))]
    [Concept]
    public class LiquefiedPetroleumGas { }

    [ConstructionCostBase(typeof(BuildingPrefabrication), 100)]
    public class FactoryOfPetroleumRefining : AbstractFactoryStatic
    {
        public override string SpriteKey => DecoratedSpriteKey("FactoryOfCrudeOilProcessing");

        protected override (Type, long) In_0_Inventory => (typeof(Worker), 1);
        protected override (Type, long) In_1_Inventory => (typeof(Electricity), 10);

        protected override (Type, long) Out0 => (typeof(LiquefiedPetroleumGas), 1);
        protected override (Type, long) Out1 => (typeof(LightOil), 1);
        protected override (Type, long) Out2 => (typeof(HeavyOil), 1);

        protected override (Type, long) In_0 => (typeof(CrudeOil), 3);
    }
}
