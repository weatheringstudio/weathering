

using System;

namespace Weathering
{
    /// <summary>
    /// 电动机
    /// </summary>
    [Depend(typeof(DiscardableSolid))]
    public class ElectroMotor { }

    [ConstructionCostBase(typeof(BuildingPrefabrication), 100)]
    public class FactoryOfElectroMotor : AbstractFactoryStatic
    {
        public override string SpriteKey => DecoratedSpriteKey(typeof(FactoryOfMetalSmelting).Name);

        protected override (Type, long) In_0_Inventory => (typeof(Worker), 1);
        protected override (Type, long) In_1_Inventory => (typeof(Electricity), 2);

        protected override (Type, long) Out0 => (typeof(ElectroMotor), 1);
        protected override (Type, long) In_0 => (typeof(SteelPipe), 4);
        protected override (Type, long) In_1 => (typeof(CopperWire), 32);
    }
}
