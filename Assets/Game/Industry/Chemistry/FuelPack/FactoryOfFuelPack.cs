

using System;

namespace Weathering
{


    //   "Weathering.FuelPack_DinitrogenTetroxide_Ethanol": "<color=#8f8f8fFF>【燃料包-N2O4-乙醇{0}】</color>",
    //"Weathering.FuelPack_DinitrogenTetroxide_EthanolSupply": "<color=#8f8f8fFF>【燃料包-N2O4-乙醇(供应){0}】</color>",
    //"Weathering.FuelPack_DinitrogenTetroxide_Hydrogen": "<color=#8f8f8fFF>【燃料包-N2O4-液氢{0}】</color>",
    //"Weathering.FuelPack_DinitrogenTetroxide_HydrogenSupply": "<color=#8f8f8fFF>【燃料包-N2O4-液氢(供应){0}】</color>",
    //"Weathering.FuelPack_DinitrogenTetroxide_JetFuel": "<color=#8f8f8fFF>【燃料包-N2O4-燃油{0}】</color>",
    //"Weathering.FuelPack_DinitrogenTetroxide_JetFuelSupply": "<color=#8f8f8fFF>【燃料包-N2O4-燃油(供应){0}】</color>",
    //"Weathering.FuelPack_HydrogenPeroxide_Ethanol": "<color=#8f8f8fFF>【燃料包-H2O2-乙醇{0}】</color>",
    //"Weathering.FuelPack_HydrogenPeroxide_EthanolSupply": "<color=#8f8f8fFF>【燃料包-H2O2-乙醇(供应){0}】</color>",
    //"Weathering.FuelPack_HydrogenPeroxide_Hydrogen": "<color=#8f8f8fFF>【燃料包-H2O2-液氢{0}】</color>",
    //"Weathering.FuelPack_HydrogenPeroxide_HydrogenSupply": "<color=#8f8f8fFF>【燃料包-H2O2-液氢(供应){0}】</color>",
    //"Weathering.FuelPack_HydrogenPeroxide_JetFuel": "<color=#8f8f8fFF>【燃料包-H2O2-燃油{0}】</color>",
    //"Weathering.FuelPack_HydrogenPeroxide_JetFuelSupply": "<color=#8f8f8fFF>【燃料包-H2O2-燃油(供应){0}】</color>",
    //"Weathering.FuelPack_Oxygen_Ethanol": "<color=#8f8f8fFF>【燃料包-液氧-乙醇{0}】</color>",
    //"Weathering.FuelPack_Oxygen_EthanolSupply": "<color=#8f8f8fFF>【燃料包-液氧-乙醇(供应){0}】</color>",
    //"Weathering.FuelPack_Oxygen_Hydrogen": "<color=#8f8f8fFF>【燃料包-液氧-液氢{0}】</color>",
    //"Weathering.FuelPack_Oxygen_HydrogenSupply": "<color=#8f8f8fFF>【燃料包-液氧-液氢(供应){0}】</color>",
    //"Weathering.FuelPack_Oxygen_JetFuel": "<color=#8f8f8fFF>【燃料包-液氧-燃油{0}】</color>",
    //"Weathering.FuelPack_Oxygen_JetFuelSupply": "<color=#8f8f8fFF>【燃料包-液氧-燃油(供应){0}】</color>",

    public abstract class FactoryOfFuelPack : AbstractFactoryStatic 
    {

    }

    // 氢氧燃料包
    [Depend(typeof(DiscardableFluid))]
    public class FuelPack_Oxygen_Hydrogen {
        public const long ELECTRICITY_CONSUMPTION = 10;
    }


    public class FactoryOfFuelPack_Oxygen_Hydrogen : AbstractFactoryStatic
    {
        public override string SpriteKey => DecoratedSpriteKey(typeof(FactoryOfFuelPack).Name);
        protected override (Type, long) In_0_Inventory => (typeof(Electricity), FuelPack_Oxygen_Hydrogen.ELECTRICITY_CONSUMPTION);

        protected override (Type, long) In_0 => (typeof(Hydrogen), 2);
        protected override (Type, long) In_1 => (typeof(Oxygen), 2);
        protected override (Type, long) Out0 => (typeof(FuelPack_Oxygen_Hydrogen), 1);
    }


    // 氢氧燃料包
    [Depend(typeof(DiscardableFluid))]
    public class FuelPack_Oxygen_JetFuel { }


    public class FactoryOfFuelPack_Oxygen_JetFuel : AbstractFactoryStatic
    {
        public override string SpriteKey => DecoratedSpriteKey(typeof(FactoryOfFuelPack).Name);
        protected override (Type, long) In_0_Inventory => (typeof(Electricity), FuelPack_Oxygen_Hydrogen.ELECTRICITY_CONSUMPTION);

        protected override (Type, long) In_0 => (typeof(JetFuel), 1);
        protected override (Type, long) In_1 => (typeof(Oxygen), 2);
        protected override (Type, long) Out0 => (typeof(FuelPack_Oxygen_JetFuel), 1);
    }

}
