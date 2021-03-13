
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
    public class ConceptResource : Attribute
    {
        public static Type Get(Type type) {
            ConceptResource concept = GetCustomAttribute(type, typeof(ConceptResource)) as ConceptResource;
            if (concept == null) throw new Exception($"{type} 没有定义 ConceptResource");
            return concept.TheType;
        }
        public Type TheType { get; private set; }
        public ConceptResource(Type type) {
            TheType = type;
        }
    }
    public class ConceptSupply : Attribute
    {
        public static Type Get(Type type) {
            ConceptSupply concept = GetCustomAttribute(type, typeof(ConceptSupply)) as ConceptSupply;
            if (concept == null) throw new Exception($"{type} 没有定义 ConceptSupply");
            return concept.TheType;
        }
        public Type TheType { get; private set; }
        public ConceptSupply(Type type) {
            TheType = type;
        }
    }

    public class ConceptTheAbstract : Attribute {}

    [Depend]
    [Concept]
    [ConceptTheAbstract]
    public class InventoryItemResource { }

    /// <summary>
    /// 可以被丢弃
    /// </summary>
    [Depend(typeof(InventoryItemResource))]
    [Concept]
    [ConceptTheAbstract]
    public class Discardable { }

    /// <summary>
    /// 不能被丢弃
    /// </summary>
    [Depend(typeof(InventoryItemResource))]
    [Concept]
    [ConceptTheAbstract]
    public class NonDiscardable { }



    // 文化
    [Depend(typeof(Discardable))]
    [Concept]
    public class Culture { }

    // 工人
    [Depend(typeof(NonDiscardable))]
    [Concept]
    public class Worker { }


    // 食物
    [ConceptDescription(typeof(FoodDescription))]
    [ConceptSupply(typeof(FoodSupply))]
    [Depend(typeof(Discardable))]
    [Concept]
    public class Food { }
    [ConceptResource(typeof(Food))]
    [Depend(typeof(NonDiscardable))]
    [Concept]
    public class FoodSupply { }
    [Concept]
    public class FoodDescription { }


    // 蔬菜
    [Depend(typeof(Food))]
    [Concept]
    public class Vegetable { }

    // 水果
    [Depend(typeof(Food))]
    [Concept]
    public class Fruit { }
    [Depend(typeof(FoodSupply))]
    [Concept]
    public class FruitSupply { }

    // 浆果
    [ConceptDescription(typeof(BerryDescription))]
    [Depend(typeof(Fruit))]
    [Concept]
    public class Berry { }
    [Depend(typeof(FruitSupply))]
    [Concept]
    public class BerrySupply { }
    [Concept]
    public class BerryDescription { }

    // 谷物
    [ConceptSupply(typeof(GrainSupply))]
    [Depend(typeof(Food))]
    [Concept]
    public class Grain { }
    [ConceptResource(typeof(Grain))]
    [Depend(typeof(FoodSupply))]
    [Concept]
    public class GrainSupply { }


    // 肉类
    [ConceptDescription(typeof(AnimalFleshDescription))]
    [Depend(typeof(Food))]
    [Concept]
    public class AnimalFlesh { }

    [Depend(typeof(FoodSupply))]
    [Concept]
    public class AnimalFleshSupply { }
    [Concept]
    public class AnimalFleshDescription { }



    // 禽肉
    [ConceptDescription(typeof(PoultryDescription))]
    [Depend(typeof(AnimalFlesh))]
    [Concept]
    public class Poultry { }

    [Depend(typeof(AnimalFleshSupply))]
    [Concept]
    public class PoultrySupply { }
    [Concept]
    public class PoultryDescription { }



    [Depend(typeof(Discardable))]
    [Concept]
    public class Flower { }


    // 石材
    [ConceptSupply(typeof(StoneSupply))]
    [ConceptDescription(typeof(StoneDescription))]
    [Depend(typeof(Discardable))]
    [Concept]
    public class Stone { }
    [ConceptResource(typeof(Stone))]
    [Depend(typeof(NonDiscardable))]
    [Concept]
    public class StoneSupply { }
    [Concept]
    public class StoneDescription { }


    // 木材
    [ConceptSupply(typeof(WoodSupply))]
    [ConceptDescription(typeof(WoodDescription))]
    [Depend(typeof(Discardable))]
    [Concept]
    public class Wood { }
    [ConceptResource(typeof(Wood))]
    [Depend(typeof(NonDiscardable))]
    [Concept]
    public class WoodSupply { }
    [Concept]
    public class WoodDescription { }


    // 金属矿
    [ConceptSupply(typeof(MetalOreSupply))]
    [ConceptDescription(typeof(MetalOreDescription))]
    [Depend(typeof(Discardable))]
    [Concept]
    public class MetalOre { }
    [ConceptResource(typeof(MetalOre))]
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

