

using System;

namespace Weathering
{
    // 燃料
    [ConceptSupply(typeof(ElectricitySupply))]
    [ConceptDescription(typeof(ElectricityDescription))]
    [Depend(typeof(Discardable))]
    [Concept]
    public class Electricity { }
    [ConceptResource(typeof(Electricity))]
    [ConceptDescription(typeof(ElectricityDescription))]
    [Depend(typeof(NonDiscardable))]
    [Concept]
    public class ElectricitySupply { }
    [Concept]
    public class ElectricityDescription { }

    [ConstructionCostBase(typeof(BuildingPrefabrication), 300)]
    public class PowerPlant : AbstractFactoryStatic
    {
        public override string SpriteKey => DecoratedSpriteKey(typeof(PowerPlant).Name);

        protected override (Type, long) In_0 => (typeof(FuelSupply), 3);

        protected override (Type, long) In_0_Inventory => (typeof(Worker), 1);
        protected override (Type, long) Out0_Inventory => (typeof(ElectricitySupply), 100);
    }
}
