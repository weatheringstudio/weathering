
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
    //public class ConceptDescription : Attribute {
    //    public Type DescriptionKey { get; private set; }
    //    public ConceptDescription(Type type) {
    //        DescriptionKey = type;
    //    }
    //}

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
    [Concept]
    public class Worker { }


    // 文化
    [Depend(typeof(Discardable))]
    [Concept]
    public class Culture { }


    // 食物
    [Depend(typeof(DiscardableSolid))]
    [Concept]
    public class Food { }


    // 蔬菜
    [Depend(typeof(Food))]
    [Concept]
    public class Vegetable { }

    // 水果
    [Depend(typeof(Food))]
    [Concept]
    public class Fruit { }




    // 肉类
    [Depend(typeof(Food))]
    [Concept]
    public class AnimalFlesh { }



    // 禽肉
    [Depend(typeof(AnimalFlesh))]
    [Concept]
    public class Poultry { }



    // 花朵
    [Depend(typeof(DiscardableSolid))]
    [Concept]
    public class Flower { }

}

