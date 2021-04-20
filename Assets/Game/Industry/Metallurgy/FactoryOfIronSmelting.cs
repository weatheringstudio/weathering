

using System;

namespace Weathering
{
    [ConstructionCostBase(typeof(BuildingPrefabrication), 100)]
    public class FactoryOfIronSmelting : AbstractFactoryStatic
    {
        public override string SpriteKey => typeof(Factory).Name;

        protected override (Type, long) In_0_Inventory => (typeof(Worker), 1);
        protected override (Type, long) In_1_Inventory => (typeof(Electricity), 10);

        protected override (Type, long) Out0 => (typeof(IronIngot), 3);

        protected override (Type, long) In_0 => (typeof(IronOre), 5);

    }
}
