

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
    [ConceptSupply(typeof(FuelPack_Oxygen_HydrogenSupply))]
    [ConceptDescription(typeof(FuelPack_Oxygen_HydrogenDescription))]
    [Depend(typeof(DiscardableFluid))]
    [Concept]
    public class FuelPack_Oxygen_Hydrogen { }

    [ConceptResource(typeof(FuelPack_Oxygen_Hydrogen))]
    [Depend(typeof(TransportableFluid))]
    [Concept]
    public class FuelPack_Oxygen_HydrogenSupply { }

    [Concept]
    public class FuelPack_Oxygen_HydrogenDescription {
        public const long ELECTRICITY_CONSUMPTION = 10;
    }

    public class FactoryOfFuelPack_Oxygen_Hydrogen : AbstractFactoryStatic
    {
        public override string SpriteKey => DecoratedSpriteKey(typeof(FactoryOfFuelPack).Name);
        protected override (Type, long) In_0_Inventory => (typeof(ElectricitySupply), FuelPack_Oxygen_HydrogenDescription.ELECTRICITY_CONSUMPTION);

        protected override (Type, long) In_0 => (typeof(HydrogenSupply), 2);
        protected override (Type, long) In_1 => (typeof(OxygenSupply), 2);
        protected override (Type, long) Out0 => (typeof(FuelPack_Oxygen_HydrogenSupply), 1);
    }


    // 氢氧燃料包
    [ConceptSupply(typeof(FuelPack_Oxygen_JetFuelSupply))]
    [ConceptDescription(typeof(FuelPack_Oxygen_JetFuelDescription))]
    [Depend(typeof(DiscardableFluid))]
    [Concept]
    public class FuelPack_Oxygen_JetFuel { }

    [ConceptResource(typeof(FuelPack_Oxygen_JetFuel))]
    [Depend(typeof(TransportableFluid))]
    [Concept]
    public class FuelPack_Oxygen_JetFuelSupply { }

    [Concept]
    public class FuelPack_Oxygen_JetFuelDescription { }

    public class FactoryOfFuelPack_Oxygen_JetFuel : AbstractFactoryStatic
    {
        public override string SpriteKey => DecoratedSpriteKey(typeof(FactoryOfFuelPack).Name);
        protected override (Type, long) In_0_Inventory => (typeof(ElectricitySupply), FuelPack_Oxygen_HydrogenDescription.ELECTRICITY_CONSUMPTION);

        protected override (Type, long) In_0 => (typeof(JetFuelSupply), 1);
        protected override (Type, long) In_1 => (typeof(OxygenSupply), 2);
        protected override (Type, long) Out0 => (typeof(FuelPack_Oxygen_JetFuelSupply), 1);
    }

}
