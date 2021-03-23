

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

    public class PowerPlant : Factory
    {
        public override string SpriteKey => DecoratedSpriteKey(typeof(PowerPlant).Name);

        protected override (Type, long) In_0 => (typeof(FuelSupply), 1);

        protected override (Type, long) In_0_Inventory => (typeof(Worker), 1);
        protected override (Type, long) Out0_Inventory => (typeof(ElectricitySupply), 1);
    }
}
