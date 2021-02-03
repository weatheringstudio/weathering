
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
    public class ConceptDescription : Attribute {
        public Type DescriptionKey { get; private set; }
        public ConceptDescription(Type type) {
            DescriptionKey = type;
        }
    }

    [Depend]
    [Concept]
    public class InventoryItemResource { }

    /// <summary>
    /// 可以被丢弃
    /// </summary>
    [Depend(typeof(InventoryItemResource))]
    [Concept]
    public class Discardable { }

    [Depend(typeof(InventoryItemResource))]
    [Concept]
    public class NonDiscardable { }



    [Depend(typeof(Discardable))]
    [Concept]
    public class Culture { }

    // 工人
    [Depend(typeof(NonDiscardable))]
    [Concept]
    public class Worker { }


    // 食物
    [ConceptDescription(typeof(FoodDescription))]
    [Depend(typeof(Discardable))]
    [Concept]
    public class Food { }
    [Depend(typeof(NonDiscardable))]
    [Concept]
    public class FoodSupply { }
    [Concept]
    public class FoodDescription { }



    [Depend(typeof(Food))]
    [Concept]
    public class Vegetable { }

    [Depend(typeof(Food))]
    [Concept]
    public class Fruit { }

    [Depend(typeof(FoodSupply))]
    [Concept]
    public class FruitSupply { }



    [ConceptDescription(typeof(BerryDescription))]
    [Depend(typeof(Fruit))]
    [Concept]
    public class Berry { }

    [Depend(typeof(FruitSupply))]
    [Concept]
    public class BerrySupply { }

    [Concept]
    public class BerryDescription { }


    [Depend(typeof(Food))]
    [Concept]
    public class Grain { }

    [Depend(typeof(FoodSupply))]
    [Concept]
    public class GrainSupply { }


    [ConceptDescription(typeof(MeatDescription))]
    [Depend(typeof(Food))]
    [Concept]
    public class Meat { }

    [Depend(typeof(FoodSupply))]
    [Concept]
    public class MeatSupply { }
    [Concept]
    public class MeatDescription { }


    [ConceptDescription(typeof(AquaticProductDescription))]
    [Depend(typeof(Meat))]
    [Concept]
    public class AquaticProduct { }

    [Depend(typeof(MeatSupply))]
    [Concept]
    public class AquaticProductSupply { }
    [Concept]
    public class AquaticProductDescription { }



    [Depend(typeof(Discardable))]
    [Concept]
    public class Flower { }


    // 石材
    [ConceptDescription(typeof(StoneDescription))]
    [Depend(typeof(Discardable))]
    [Concept]
    public class Stone { }
    [Depend(typeof(NonDiscardable))]
    [Concept]
    public class StoneSupply { }
    [Concept]
    public class StoneDescription { }


    // 木材
    [ConceptDescription(typeof(WoodDescription))]
    [Depend(typeof(Discardable))]
    [Concept]
    public class Wood { }
    [Depend(typeof(NonDiscardable))]
    [Concept]
    public class WoodSupply { }

    [Concept]
    public class WoodDescription { }


    // 金属矿
    [ConceptDescription(typeof(MetalOreDescription))]
    [Depend(typeof(Discardable))]
    [Concept]
    public class MetalOre { }
    [Depend(typeof(NonDiscardable))]
    [Concept]
    public class MetalOreSupply { }

    [Concept]
    public class MetalOreDescription { }



    [Depend(typeof(Discardable))]
    [Concept]
    public class WorkshopProduct { }


    [Depend(typeof(Discardable))]
    [Concept]
    public class Weapon { }


    [Depend(typeof(Weapon))]
    [Concept]
    public class Knife { }
}

