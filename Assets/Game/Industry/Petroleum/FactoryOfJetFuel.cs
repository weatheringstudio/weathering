

using System;

namespace Weathering
{
    // 航空燃油
    [Depend(typeof(DiscardableFluid))]
    [Concept]
    public class JetFuel { }


    [ConstructionCostBase(typeof(BuildingPrefabrication), 100)]
    public class FactoryOfJetFuel : AbstractFactoryStatic
    {
        public override string SpriteKey => DecoratedSpriteKey(typeof(FactoryOfMetalSmelting).Name);

        protected override (Type, long) In_0_Inventory => (typeof(Worker), 1);
        protected override (Type, long) In_1_Inventory => (typeof(Electricity), 10);
        protected override (Type, long) Out0 => (typeof(JetFuel), 2);
        protected override (Type, long) In_0 => (typeof(LightOil), 1);
        protected override (Type, long) In_1 => (typeof(HeavyOil), 1);
    }
}
