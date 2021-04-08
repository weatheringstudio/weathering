
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
            if (concept == null) UIPreset.Throw($"{type} 没有定义 ConceptResource。是不是使用了xxx而不是xxxSupply");
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
    /// 可以被丢弃
    /// </summary>
    [Depend(typeof(Discardable))]
    [Concept]
    [ConceptTheAbstract]
    public class DiscardableSolid { }

    /// <summary>
    /// 可以被丢弃
    /// </summary>
    [Depend(typeof(Discardable))]
    [Concept]
    [ConceptTheAbstract]
    public class DiscardableFluid { }

    /// <summary>
    /// 不能被丢弃
    /// </summary>
    [Depend(typeof(InventoryItemResource))]
    [Concept]
    [ConceptTheAbstract]
    public class NonDiscardable { }

    /// <summary>
    /// 不能被丢弃
    /// </summary>
    [Depend(typeof(NonDiscardable))]
    [Concept]
    [ConceptTheAbstract]
    public class TransportableSolid { }

    /// <summary>
    /// 不能被丢弃
    /// </summary>
    [Depend(typeof(NonDiscardable))]
    [Concept]
    [ConceptTheAbstract]
    public class TransportableFluid { }


    // 工人
    [Depend(typeof(NonDiscardable))]
    [ConceptResource(typeof(Worker))]
    [ConceptSupply(typeof(Worker))]
    [ConceptDescription(typeof(WorkerDescription))]
    [Concept]
    public class Worker { }
    [Concept]
    public class WorkerDescription { }


    // 文化
    [Depend(typeof(Discardable))]
    [Concept]
    public class Culture { }


    // 食物
    [ConceptDescription(typeof(FoodDescription))]
    [ConceptSupply(typeof(FoodSupply))]
    [Depend(typeof(DiscardableSolid))]
    [Concept]
    public class Food { }
    [ConceptResource(typeof(Food))]
    [Depend(typeof(TransportableSolid))]
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


    // 花朵
    [Depend(typeof(DiscardableSolid))]
    [Concept]
    public class Flower { }

}

