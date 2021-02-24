
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
    public class SeaFishery : StandardTile, ISealike, ILookLikeRoad
    {
        public bool IsLikeSea => true;
        public override string SpriteKey {
            get {
                int index = TileUtility.Calculate6x8RuleTileIndex(tile => (tile as ISealike) != null && (tile as ISealike).IsLikeSea, Map, Pos);
                return "Sea_" + index.ToString();
            }
        }
        public override string SpriteKeyOverlay => typeof(SeaFishery).Name;

        public bool LookLikeRoad => level.Max == 2;

        IValue fish;
        IValue level;
        private const long fishInc = 2;
        private const long fishMax = 20;
        public override void OnConstruct() {
            base.OnConstruct();
            Values = Weathering.Values.GetOne();
            fish = Values.Create<FishFlesh>();
            fish.Max = fishMax;
            fish.Inc = fishInc;
            fish.Del = 20 * Value.Second;

            level = Values.Create<Level>();
            level.Max = 1;

            Refs = Weathering.Refs.GetOne();
        }

        public override void OnEnable() {
            base.OnEnable();
            fish = Values.Get<FishFlesh>();
            level = Values.Get<Level>();
        }

        public override void OnTap() {
            var inventoryQuery = InventoryQuery.Create(OnTap, Map.Inventory, new List<InventoryQueryItem> {
                new InventoryQueryItem {Target = Map.Inventory, Quantity = 1, Type = typeof(FishFleshSupply)}
            });
            var inventoryQueryInversed = inventoryQuery.CreateInversed();

            if (level.Max == 1) {
                UI.Ins.ShowItems(string.Format(Localization.Ins.Get<StateOfProducing>(), Localization.Ins.Get<SeaFishery>()),
                    UIItem.CreateText("摸鱼中"),
                    UIItem.CreateValueProgress<FishFlesh>(Values),
                    UIItem.CreateTimeProgress<FishFlesh>(Values),

                    UIItem.CreateSeparator(),
                    UIItem.CreateButton($"{Localization.Ins.Get<Gather>()}{Localization.Ins.ValUnit<FishFlesh>()}", GatherFish, () => fish.Val > 0),
                    UIItem.CreateButton($"带走正在仰泳的鱼{inventoryQuery.GetDescription()}", () => {
                        inventoryQuery.TryDo(() => {
                            fish.Max = long.MaxValue;
                            level.Max = 2;
                        });
                    })

                    , UIItem.CreateSeparator()
                    , UIItem.CreateDestructButton<TerrainDefault>(this)
                );
            } else if (level.Max == 2) {
                UI.Ins.ShowItems(string.Format(Localization.Ins.Get<StateOfAutomated>(), Localization.Ins.Get<SeaFishery>())
                    , UIItem.CreateText("在沙滩上晒太阳的鱼，提供了稳定的食物供给")
                    , UIItem.CreateInventoryItem<FishFleshSupply>(Map.Inventory, OnTap)
                    , UIItem.CreateTimeProgress<FishFlesh>(Values)

                    , UIItem.CreateSeparator()
                    , UIItem.CreateButton($"{Localization.Ins.Get<Gather>()}{Localization.Ins.ValUnit<FishFlesh>()}", GatherFish, () => false)
                    , UIItem.CreateButton($"不再按时捡走鱼{inventoryQueryInversed.GetDescription()}", () => {
                        inventoryQueryInversed.TryDo(() => {
                            fish.Max = fishMax;
                            fish.Val = 0;
                            level.Max = 1;
                        });
                    })

                    , UIItem.CreateSeparator()
                    , UIItem.CreateDestructButton<TerrainDefault>(this, () => false)
                );
            } else {
                throw new Exception();
            }
        }

        private const long gatherFishSanityCost = 1;
        private void GatherFish() {
            if (Globals.Sanity.Val < gatherFishSanityCost) {
                UIPreset.ResourceInsufficient<Sanity>(OnTap, gatherFishSanityCost, Globals.Sanity);
                return;
            }
            if (Map.Inventory.CanAdd<FishFlesh>() <= 0) {
                UIPreset.InventoryFull(OnTap, Map.Inventory);
                return;
            }

            Globals.Sanity.Val -= gatherFishSanityCost;
            Map.Inventory.AddFrom<FishFlesh>(fish);
        }
    }
}

