
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


    //[Depend(typeof(Discardable))]
    //[Concept]
    //public class Labor { }


    [Depend(typeof(NonDiscardable))]
    [Concept]
    public class Worker { }


    
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


    [Depend(typeof(Discardable))]
    [Concept]
    public class Flower { }

    [Depend(typeof(Discardable))]
    [Concept]
    public class Stone { }


    [Depend(typeof(Discardable))]
    [Concept]
    public class Wood { }
    [Depend(typeof(NonDiscardable))]
    [Concept]
    public class WoodSupply { }


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

