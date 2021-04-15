

using System;

namespace Weathering
{

    // 铀235
    [Depend(typeof(DiscardableSolid))]
    public class Uranrium235 { }


    // 铀238
    [Depend(typeof(DiscardableSolid))]
    public class Uranrium238 { }




    [ConstructionCostBase(typeof(BuildingPrefabrication), 100)]
    public class PowerGeneratorOfNulearFissionEnergy : AbstractFactoryStatic
    {
        public override string SpriteKey => DecoratedSpriteKey(typeof(PowerPlant).Name);

        protected override (Type, long) In_0 => (typeof(Uranrium235), 1);
        protected override (Type, long) Out0_Inventory => (typeof(Electricity), 300);
    }
}
