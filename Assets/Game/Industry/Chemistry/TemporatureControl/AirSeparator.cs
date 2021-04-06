

using System;

namespace Weathering
{

    // 氧气
    [ConceptSupply(typeof(OxygenSupply))]
    [ConceptDescription(typeof(OxygenDescription))]
    [Depend(typeof(DiscardableFluid))]
    [Concept]
    public class Oxygen { }

    [ConceptResource(typeof(Oxygen))]
    [Depend(typeof(MetalIngotSupply))]
    [Concept]
    public class OxygenSupply { }

    [Concept]
    public class OxygenDescription { }

    // 氮气
    [ConceptSupply(typeof(NitrogenSupply))]
    [ConceptDescription(typeof(NitrogenDescription))]
    [Depend(typeof(DiscardableFluid))]
    [Concept]
    public class Nitrogen { }

    [ConceptResource(typeof(Nitrogen))]
    [Depend(typeof(TransportableFluid))]
    [Concept]
    public class NitrogenSupply { }

    [Concept]
    public class NitrogenDescription { }

    public class AirSeparator : AbstractFactoryStatic
    {
        public override string SpriteKey => DecoratedSpriteKey(typeof(FactoryOfMetalSmelting).Name);

        protected override (Type, long) In_0_Inventory => (typeof(ElectricitySupply), 30);

        protected override (Type, long) Out0 => (typeof(NitrogenSupply), 3);
        protected override (Type, long) Out1 => (typeof(OxygenSupply), 1);
    }
}
