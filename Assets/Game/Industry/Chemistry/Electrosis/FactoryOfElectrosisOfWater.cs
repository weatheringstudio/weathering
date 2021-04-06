

using System;

namespace Weathering
{
    // 氢气
    [ConceptSupply(typeof(HydrogenSupply))]
    [ConceptDescription(typeof(HydrogenDescription))]
    [Depend(typeof(DiscardableFluid))]
    [Concept]
    public class Hydrogen { }

    [ConceptResource(typeof(Hydrogen))]
    [Depend(typeof(TransportableFluid))]
    [Concept]
    public class HydrogenSupply { }

    [Concept]
    public class HydrogenDescription { }


    public class FactoryOfElectrosis { }

    public class FactoryOfElectrosisOfWater : AbstractFactoryStatic
    {
        public override string SpriteKey => DecoratedSpriteKey(typeof(FactoryOfElectrosis).Name);

        protected override (Type, long) In_0_Inventory => (typeof(ElectricitySupply), 30);

        protected override (Type, long) In_0 => (typeof(WaterSupply), 2);
        protected override (Type, long) Out0 => (typeof(HydrogenSupply), 2);
        protected override (Type, long) Out1 => (typeof(OxygenSupply), 1);
    }
}
