

using System;

namespace Weathering
{
    // 石油, 液体
    [Depend(typeof(DiscardableFluid))]
    [Concept]
    public class CrudeOil { }

    [ConstructionCostBase(typeof(BuildingPrefabrication), 100)]
    public class OilDriller : AbstractFactoryStatic
    {
        public override string SpriteKey => DecoratedSpriteKey(typeof(OilDriller).Name);

        protected override (Type, long) In_0_Inventory => (typeof(Worker), 1);
        protected override (Type, long) In_1_Inventory => (typeof(Electricity), 10);

        protected override (Type, long) Out0 => (typeof(CrudeOil), 1);
    }
}
