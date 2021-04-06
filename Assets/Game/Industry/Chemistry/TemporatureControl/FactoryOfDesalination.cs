

using System;

namespace Weathering
{

    // 纯净水
    [ConceptSupply(typeof(WaterSupply))]
    [ConceptDescription(typeof(WaterDescription))]
    [Depend(typeof(DiscardableFluid))]
    [Concept]
    public class Water { }

    [ConceptResource(typeof(Water))]
    [Depend(typeof(TransportableFluid))]
    [Concept]
    public class WaterSupply { }

    [Concept]
    public class WaterDescription { }

    public class FactoryOfDesalination : AbstractFactoryStatic
    {
        public override string SpriteKey => DecoratedSpriteKey(typeof(FactoryOfElectrosis).Name);

        protected override (Type, long) In_0_Inventory => (typeof(ElectricitySupply), 2);

        protected override (Type, long) In_0 => (typeof(SeaWaterSupply), 1);
        protected override (Type, long) Out0 => (typeof(WaterSupply), 1);
    }
}
