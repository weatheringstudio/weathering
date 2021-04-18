

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{

    [Depend]
    public class Technology { }


    public abstract class AbstractTechnologyCenter : StandardTile, ILinkEvent, ILinkConsumer
    {
        public override string SpriteKey => techRef.Value == 0 ? GetType().Name : $"{GetType().Name}_Working";

        public override string SpriteLeft => GetSprite(Vector2Int.left, typeof(ILeft));
        public override string SpriteRight => GetSprite(Vector2Int.right, typeof(IRight));
        public override string SpriteUp => GetSprite(Vector2Int.up, typeof(IUp));
        public override string SpriteDown => GetSprite(Vector2Int.down, typeof(IDown));
        private string GetSprite(Vector2Int dir, Type direction) {
            IRefs refs = Map.Get(Pos - dir).Refs;
            if (refs == null) return null;
            if (refs.TryGet(direction, out IRef result)) return result.Value < 0 ? result.Type.Name : null;
            return null;
        }


        private IValue techValue;
        private IRef techRef;
        public void Consume(List<IRef> refs) {
            refs.Add(techRef);
        }

        protected virtual long TechnologyIncMax { get => long.MaxValue; }

        protected virtual long TechnologyPointCapacity { get; } = 0;

        public override void OnConstruct(ITile oldTile) {
            techValue = Globals.Ins.Values.GetOrCreate(TechnologyPointType);

            if (TechnologyPointCapacity > 0) {
                techValue.Max += TechnologyPointCapacity;
            }

            Refs = Weathering.Refs.GetOne();

            techRef = Refs.Create(TechnologyPointType);
            techRef.BaseValue = TechnologyIncMax;
        }

        public override void OnDestruct(ITile newTile) {
            if (TechnologyPointCapacity > 0) {
                techValue.Max -= TechnologyPointCapacity;
            }
        }

        private static AudioClip soundEffectOnUnlockTech;
        public override void OnEnable() {
            base.OnEnable();

            techValue = Globals.Ins.Values.Get(TechnologyPointType);

            techRef = Refs.Get(TechnologyPointType);
            techRef.Type = TechnologyPointType;

            if (soundEffectOnUnlockTech == null) {
                soundEffectOnUnlockTech = Sound.Ins.Get("mixkit-magic-potion-music-and-fx-2831");
            }
        }

        protected virtual void DecorateItems(List<IUIItem> items, Action onTap) { }
        protected virtual void DecorateIfCompleted(List<IUIItem> items) { }


        public void OnLink(Type direction, long quantity) {
            techValue.Inc += quantity;
        }

        private static List<IUIItem> itemsUnlockedBuffer = new List<IUIItem>();
        private List<IUIItem> items;
        public override void OnTap() {
            items = UI.Ins.GetItems();


            DecorateItems(items, OnTap);

            Type techType = TechnologyPointType;

            if (TechnologyPointCapacity > 0) {
                items.Add(UIItem.CreateMultilineText($"每个{Localization.Ins.Get(GetType())}可以提高{Localization.Ins.ValUnit(techType)}上限{TechnologyPointCapacity}"));
            }

            items.Add(UIItem.CreateValueProgress(techType, techValue));
            if (techValue.Inc > 0) {
                items.Add(UIItem.CreateTimeProgress(techType, techValue));
            }

            items.Add(UIItem.CreateSeparator());

            if (itemsUnlockedBuffer.Count != 0) throw new Exception();
            int techShowed = 0;
            foreach (var item in TechList) {
                Type techPointType = item.Item1;
                long techPointCount = item.Item2;

                // test. TODO: remove this after testing
                if (techPointType == typeof(SchoolEquipment)) techPointCount = 10;

                string techName = Localization.Ins.Get(techPointType);

                bool hasTech = Globals.Ins.Bool(techPointType);
                if (!hasTech) {
                    items.Add(UIItem.CreateDynamicButton(techPointCount == 0 ? $"研究 {techName}" : 
                        $"研究 {techName} {(DontConsumeTechnologyPoint ? $"需要{Localization.Ins.Val(techType, techPointCount)}" : Localization.Ins.Val(techType, -techPointCount))}", () => {

                            if (!DontConsumeTechnologyPoint) {
                                techValue.Val -= techPointCount;
                            }

                            Globals.Ins.Bool(techPointType, true);
                            Sound.Ins.Play(soundEffectOnUnlockTech);

                            // OnTap();
                            if (TechnologyResearched_Event.Event.TryGetValue(techPointType, out var action)) {
                                var items_ = UI.Ins.GetItems();
                                items_.Add(UIItem.CreateReturnButton(OnTap));
                                action(items_);
                                UI.Ins.ShowItems($"成功研究 {Localization.Ins.Get(techPointType)}", items_);
                            }
                            else {
                                OnTap();
                            }

                        }, () => Globals.Ins.Values.Get(techType).Val >= techPointCount));
                    techShowed++;
                }
                else {
                    if (TechnologyResearched_Event.Event.TryGetValue(techPointType, out var action)) {
                        itemsUnlockedBuffer.Add(UIItem.CreateButton($"已研究 {techName}", () => {
                            var items_ = UI.Ins.GetItems();
                            items_.Add(UIItem.CreateReturnButton(OnTap));
                            action(items_);
                            UI.Ins.ShowItems($"成功研究 {Localization.Ins.Get(techPointType)}", items_);
                        }));
                    } else {
                        itemsUnlockedBuffer.Add(UIItem.CreateText($"已研究 {techName}"));
                    }
                }
                if (techShowed >= ShowedTechToBeResearched) {
                    break;
                }
            }

            if (techShowed == 0) {
                items.Add(UIItem.CreateText($"{Localization.Ins.Get(GetType())}的所有技术, 已经全部被成功研究! "));
                DecorateIfCompleted(items);
            }

            items.Add(UIItem.CreateSeparator());

            foreach (var item in itemsUnlockedBuffer) {
                items.Add(item);
            }
            itemsUnlockedBuffer.Clear();

            items.Add(UIItem.CreateSeparator());
            items.Add(UIItem.CreateStaticDestructButton(this));


            UI.Ins.ShowItems(Localization.Ins.Get(GetType()), items);
            items = null;
        }

        public override bool CanDestruct() => true;



        private const int ShowedTechToBeResearched = 3;

        /// <summary>
        /// 可解锁的科技列表
        /// </summary>
        protected abstract List<ValueTuple<Type, long>> TechList { get; }
        /// <summary>
        /// 科技所消耗资源
        /// </summary>
        protected abstract Type TechnologyPointType { get; }
        /// <summary>
        /// 消耗科技点
        /// </summary>
        protected virtual bool DontConsumeTechnologyPoint { get => false; }

    }
}

