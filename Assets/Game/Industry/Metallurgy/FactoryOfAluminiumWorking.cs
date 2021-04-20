

using System;

namespace Weathering
{

    // 铝锭
    [Depend(typeof(MetalIngot))]
    public class AluminiumIngot { }


    [ConstructionCostBase(typeof(BuildingPrefabrication), 100)]
    public class FactoryOfAluminiumWorking : AbstractFactoryStatic
    {
        public override string SpriteKey => typeof(Factory).Name;


        protected override (Type, long) In_0_Inventory => (typeof(Worker), 1);
        protected override (Type, long) In_1_Inventory => (typeof(Electricity), 30);
        protected override (Type, long) Out0 => (typeof(AluminiumIngot), 1);
        protected override (Type, long) In_0 => (typeof(AluminumOre), 6);
    }
}
