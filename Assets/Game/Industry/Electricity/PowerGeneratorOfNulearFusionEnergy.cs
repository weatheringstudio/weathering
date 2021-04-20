

using System;

namespace Weathering
{

    // 氘
    [Depend(typeof(DiscardableSolid))]
    public class Deuterium { }


    // 氚
    [Depend(typeof(DiscardableSolid))]
    public class Tritium { }



    [ConstructionCostBase(typeof(BuildingPrefabrication), 100)]
    public class PowerGeneratorOfNulearFusionEnergy : AbstractFactoryStatic
    {
        public override string SpriteKey => typeof(PowerPlant).Name;

        protected override (Type, long) In_0 => (typeof(Deuterium), 1);
        protected override (Type, long) In_1 => (typeof(Tritium), 1);
        protected override (Type, long) Out0_Inventory => (typeof(Electricity), 500);
    }
}
