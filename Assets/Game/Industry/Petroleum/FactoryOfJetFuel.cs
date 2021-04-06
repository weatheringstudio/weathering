

using System;

namespace Weathering
{
    // 航空燃油
    [ConceptSupply(typeof(JetFuelSupply))]
    [ConceptDescription(typeof(JetFuelDescription))]
    [Depend(typeof(DiscardableFluid))]
    [Concept]
    public class JetFuel { }

    [ConceptResource(typeof(JetFuel))]
    [Depend(typeof(TransportableFluid))]
    [Concept]
    public class JetFuelSupply { }

    [Concept]
    public class JetFuelDescription { }

    [ConstructionCostBase(typeof(BuildingPrefabrication), 100)]
    public class FactoryOfJetFuel : AbstractFactoryStatic
    {
        public override string SpriteKey => DecoratedSpriteKey(typeof(FactoryOfMetalSmelting).Name);

        protected override (Type, long) In_0_Inventory => (typeof(Worker), 1);
        protected override (Type, long) In_1_Inventory => (typeof(ElectricitySupply), 10);
        protected override (Type, long) Out0 => (typeof(JetFuelSupply), 2);
        protected override (Type, long) In_0 => (typeof(LightOilSupply), 1);
        protected override (Type, long) In_1 => (typeof(HeavyOilSupply), 1);
    }
}
