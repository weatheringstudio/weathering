

using System;

namespace Weathering
{

    // 氘
    [ConceptSupply(typeof(DeuteriumSupply))]
    [ConceptDescription(typeof(DeuteriumDescription))]
    [Depend(typeof(DiscardableSolid))]
    [Concept]
    public class Deuterium { }

    [ConceptResource(typeof(Deuterium))]
    [Depend(typeof(TransportableSolid))]
    [Concept]
    public class DeuteriumSupply { }

    [Concept]
    public class DeuteriumDescription { }

    // 氚
    [ConceptSupply(typeof(TritiumSupply))]
    [ConceptDescription(typeof(TritiumDescription))]
    [Depend(typeof(DiscardableSolid))]
    [Concept]
    public class Tritium { }

    [ConceptResource(typeof(Tritium))]
    [Depend(typeof(TransportableSolid))]
    [Concept]
    public class TritiumSupply { }

    [Concept]
    public class TritiumDescription { }



    [ConstructionCostBase(typeof(BuildingPrefabrication), 100)]
    public class PowerGeneratorOfNulearFusionEnergy : AbstractFactoryStatic
    {
        public override string SpriteKey => DecoratedSpriteKey(typeof(PowerPlant).Name);

        protected override (Type, long) In_0 => (typeof(DeuteriumSupply), 1);
        protected override (Type, long) In_1 => (typeof(TritiumSupply), 1);
        protected override (Type, long) Out0_Inventory => (typeof(ElectricitySupply), 500);
    }
}
