
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{

    [Concept]
    public class Sanity { }



    [Concept]
    public class OperationUnavailable { }

    [Concept]
    public class PlayerAction { }
    [Concept]
    public class Destruct { }
    [Concept]
    public class Construct { }
    [Concept]
    public class ReturnMenu { }
    [Concept]
    public class Management { }
    [Concept]
    public class Harvest { }
    [Concept]
    public class Sow { }
    [Concept]
    public class Gather { }
    [Concept]
    public class Terraform { }

    [Concept]
    public class Deforestation { }

    [Concept]
    public class ExitGame {}
    [Concept]
    public class SaveGame { }
    [Concept]
    public class ResetGame { }
    [Concept]
    public class GameSettings { }





    [Concept]
    public class ProductionProgress { }

    [Concept]
    public class Level { }

    public class Stage { }


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
    public class NonDiscardable { }


    [Depend(typeof(Discardable))]
    [Concept]
    public class Culture { }


    [Depend(typeof(Discardable))]
    [Concept]
    public class Labor { }


    [Depend(typeof(NonDiscardable))]
    [Concept]
    public class Worker { }




    [Depend(typeof(Discardable))]
    [Concept]
    public class Food { }
    [Depend(typeof(NonDiscardable))]
    [Concept]
    public class FoodSupply { }



    [Depend(typeof(Food))]
    [Concept]
    public class Vegetable { }

    [Depend(typeof(Food))]
    [Concept]
    public class Fruit { }

    [Depend(typeof(FoodSupply))]
    [Concept]
    public class FruitSupply { }

    [Depend(typeof(Fruit))]
    [Concept]
    public class Berry { }

    [Depend(typeof(FruitSupply))]
    [Concept]
    public class BerrySupply { }

    [Depend(typeof(Food))]
    [Concept]
    public class Grain { }

    
    [Depend(typeof(Food))]
    [Concept]
    public class Meat { }

    [Depend(typeof(FoodSupply))]
    [Concept]
    public class MeatSupply { }



    [Depend(typeof(Discardable))]
    [Concept]
    public class Flower { }

    [Depend(typeof(Discardable))]
    [Concept]
    public class Stone { }

    [Depend(typeof(Discardable))]
    [Concept]
    public class Wood { }

    [Depend(typeof(Discardable))]
    [Concept]
    public class WorkshopProduct { }
}

