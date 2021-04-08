

using System;

namespace Weathering
{

    // 氯气
    [ConceptSupply(typeof(ChlorineSupply))]
    [ConceptDescription(typeof(ChlorineDescription))]
    [Depend(typeof(DiscardableFluid))]
    [Concept]
    public class Chlorine { }

    [ConceptResource(typeof(Chlorine))]
    [Depend(typeof(TransportableFluid))]
    [Concept]
    public class ChlorineSupply { }

    [Concept]
    public class ChlorineDescription { }


    // 氢氧化钠
    [ConceptSupply(typeof(SodiumHydroxideSupply))]
    [ConceptDescription(typeof(SodiumHydroxideDescription))]
    [Depend(typeof(DiscardableFluid))]
    [Concept]
    public class SodiumHydroxide { }

    [ConceptResource(typeof(SodiumHydroxide))]
    [Depend(typeof(TransportableFluid))]
    [Concept]
    public class SodiumHydroxideSupply { }

    [Concept]
    public class SodiumHydroxideDescription { }


    public class FactoryOfElectrolysisOfSaltedWater : AbstractFactoryStatic
    {
        public override string SpriteKey => DecoratedSpriteKey(typeof(FactoryOfElectrolysis).Name);

        protected override (Type, long) In_0_Inventory => (typeof(ElectricitySupply), 30);

        protected override (Type, long) In_0 => (typeof(SeaWaterSupply), 2);
        protected override (Type, long) Out0 => (typeof(HydrogenSupply), 1);
        protected override (Type, long) Out1 => (typeof(ChlorineSupply), 1);
        protected override (Type, long) Out2 => (typeof(SodiumHydroxideSupply), 1);
    }
}
