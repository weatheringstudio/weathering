
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{

    // 鹿肉
    [ConceptSupply(typeof(DearMeatSupply))]
    [ConceptDescription(typeof(DearMeatDescription))]
    [Depend(typeof(Meat))]
    [Concept]
    public class DearMeat { }
    [ConceptResource(typeof(DearMeat))]
    [Depend(typeof(MeatSupply))]
    [Concept]
    public class DearMeatSupply { }
    [Concept]
    public class DearMeatDescription { }

    // 兔肉
    [ConceptSupply(typeof(RabbitMeatSupply))]
    [ConceptDescription(typeof(RabbitMeatDescription))]
    [Depend(typeof(Meat))]
    [Concept]
    public class RabbitMeat { }
    [ConceptResource(typeof(RabbitMeat))]
    [Depend(typeof(MeatSupply))]
    [Concept]
    public class RabbitMeatSupply { }
    [Concept]
    public class RabbitMeatDescription { }


    [Concept]
    public class HuntingGround : StandardTile, IProvider
    {
        public Tuple<Type, long> CanProvide => new Tuple<Type, long>(meatType, 1);
        public void Provide(Tuple<Type, long> request) {
            throw new NotImplementedException();
        }

        private static List<Type> huntingGroundPossibleRevenues = new List<Type>() {
            typeof(DearMeat), typeof(RabbitMeat),
        };

        public override string SpriteKey {
            get {
                if (level.Max == 2) {
                    return producing;
                } else if (level.Max == 1) {
                    if (meatValue.Maxed) {
                        return TimeUtility.GetFrame(0.2f, 2) == 0 ? producing : notProducing;
                    } else {
                        return producing;
                    }
                }
                return producing;
            }
        }


        private string producing = typeof(HuntingGround).Name + "Producing";
        private string notProducing = typeof(HuntingGround).Name;

        private Type meatType;
        private Type meatSupplyType;
        private IValue meatValue;
        private IValue level;
        private const long foodInc = 1;
        private const long foodMax = 10;
        public override void OnConstruct() {
            base.OnConstruct();
            meatType = huntingGroundPossibleRevenues[HashUtility.PerlinNoise((float)Pos.x/Map.Width, (float)Pos.y/Map.Height, Map.Width, Map.Height) < 0 ? 0 : 1];
            var meatSupplyAttr = (Attribute.GetCustomAttribute(meatType, typeof(ConceptSupply)) as ConceptSupply);
            if (meatSupplyAttr == null) throw new Exception(meatType.Name);
            meatSupplyType = meatSupplyAttr.TheType;
            if (meatSupplyType == null) throw new Exception();
            Refs = Weathering.Refs.GetOne();
            Refs.Create<HuntingGround>().Type = meatType;

            Values = Weathering.Values.GetOne();
            meatValue = Values.Create(meatType);
            meatValue.Max = foodMax;
            meatValue.Inc = foodInc;
            meatValue.Del = 10 * Value.Second;

            level = Values.Create<Level>();
            level.Max = 1;
        }

        public override void OnEnable() {
            base.OnEnable();
            meatType = Refs.Get<HuntingGround>().Type;
            meatSupplyType = (Attribute.GetCustomAttribute(meatType, typeof(ConceptSupply)) as ConceptSupply).TheType;

            meatValue = Values.Get(meatType);
            level = Values.Get<Level>();
        }

        public override void OnTap() {
            var inventoryQuery = InventoryQuery.Create(OnTap, Map.Inventory, new List<InventoryQueryItem> {
                new InventoryQueryItem {Target = Map.Inventory, Quantity = 1, Type = meatSupplyType}
            });
            var inventoryQueryInversed = inventoryQuery.CreateInversed();

            if (level.Max == 1) {
                UI.Ins.ShowItems(string.Format(Localization.Ins.Get<StateOfProducing>(), Localization.Ins.Get<HuntingGround>()),
                    UIItem.CreateText("正在等待猎物撞上树干"),
                    UIItem.CreateValueProgress(meatType, meatValue),
                    UIItem.CreateTimeProgress(meatType, meatValue),

                    UIItem.CreateSeparator(),
                    UIItem.CreateButton($"{Localization.Ins.Get<Gather>()}{Localization.Ins.ValUnit(meatType)}", GatherFood, () => meatValue.Val > 0),
                    UIItem.CreateButton($"按时捡走猎物{inventoryQuery.GetDescription()}", () => {
                        inventoryQuery.TryDo(() => {
                            meatValue.Max = long.MaxValue;
                            level.Max = 2;
                        });
                    })

                    , UIItem.CreateSeparator()
                    , UIItem.CreateDestructButton<TerrainDefault>(this)
                );
            }
            else if (level.Max == 2) {
                UI.Ins.ShowItems(string.Format(Localization.Ins.Get<StateOfAutomated>(), Localization.Ins.Get<HuntingGround>())
                    , UIItem.CreateText("森林里每天都有猎物撞上树干，提供了稳定的食物供给")
                    , UIItem.CreateInventoryItem(meatSupplyType, Map.Inventory, OnTap)
                    , UIItem.CreateTimeProgress(meatType, meatValue)

                    , UIItem.CreateSeparator()
                    , UIItem.CreateButton($"{Localization.Ins.Get<Gather>()}{Localization.Ins.ValUnit(meatType)}", GatherFood, () => false)
                    , UIItem.CreateButton($"不再按时捡走猎物{inventoryQueryInversed.GetDescription()}", () => {
                        inventoryQueryInversed.TryDo(() => {
                            meatValue.Max = foodMax;
                            meatValue.Val = 0;
                            level.Max = 1;
                        });
                    })

                    , UIItem.CreateSeparator()
                    , UIItem.CreateDestructButton<TerrainDefault>(this, () => false)
                );
            }
            else {
                throw new Exception();
            }
        }

        private const long gatherFoodSanityCost = 1;
        private void GatherFood() {
            if (Globals.Sanity.Val < gatherFoodSanityCost) {
                UIPreset.ResourceInsufficient<Sanity>(OnTap, gatherFoodSanityCost, Globals.Sanity);
                return;
            }
            if (Map.Inventory.CanAdd(meatType) <= 0) {
                UIPreset.InventoryFull(OnTap, Map.Inventory);
                return;
            }

            Globals.Sanity.Val -= gatherFoodSanityCost;
            Map.Inventory.AddFrom(meatType, meatValue);
        }

    }
}

