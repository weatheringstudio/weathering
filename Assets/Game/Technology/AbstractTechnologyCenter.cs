﻿

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{

    [Depend]
    public class Technology { }


    public abstract class AbstractTechnologyCenter : StandardTile
    {

        private IValue techValue;

        protected virtual long Capacity { get; } = 0;

        public override void OnConstruct(ITile oldTile) {
            techValue = Globals.Ins.Values.GetOrCreate(TechnologyType);

            if (Capacity > 0) {
                techValue.Max += Capacity;
            }
        }

        public override void OnDestruct(ITile newTile) {
            if (Capacity > 0) {
                techValue.Max -= Capacity;
            }
        }

        private static AudioClip soundEffectOnUnlockTech;
        public override void OnEnable() {
            base.OnEnable();

            techValue = Globals.Ins.Values.Get(TechnologyType);

            if (soundEffectOnUnlockTech == null) {
                soundEffectOnUnlockTech = Sound.Ins.Get("mixkit-magic-potion-music-and-fx-2831");
            }
        }

        protected virtual void DecorateItems(List<IUIItem> items, Action onTap) { }
        protected virtual void DecorateIfCompleted(List<IUIItem> items) { }


        private static List<IUIItem> itemsUnlockedBuffer = new List<IUIItem>();
        private List<IUIItem> items;
        public override void OnTap() {
            items = UI.Ins.GetItems();


            DecorateItems(items, OnTap);

            Type techType = TechnologyType;
            items.Add(UIItem.CreateValueProgress(techType, techValue));
            if (techValue.Inc > 0) {
                items.Add(UIItem.CreateTimeProgress(techType, techValue));
            }

            items.Add(UIItem.CreateSeparator());

            if (itemsUnlockedBuffer.Count != 0) throw new Exception();
            int techShowed = 0;
            foreach (var item in TechList) {
                Type tech = item.Item1;
                long techPointCount = item.Item2;

                bool hasTech = Globals.Ins.Bool(tech);
                string techName = Localization.Ins.Get(tech);
                if (!hasTech) {
                    items.Add(UIItem.CreateDynamicButton(techPointCount == 0 ? $"研究 {techName}" : 
                        $"研究 {techName} {Localization.Ins.Val(techType, -techPointCount)}", () => {

                            techValue.Val -= techPointCount;
                            Globals.Ins.Bool(tech, true);
                            Sound.Ins.Play(soundEffectOnUnlockTech);

                            // OnTap();
                            if (TechnologyResearched_Event.Event.TryGetValue(tech, out var action)) {
                                var items_ = UI.Ins.GetItems();
                                items_.Add(UIItem.CreateReturnButton(OnTap));
                                action(items_);
                                UI.Ins.ShowItems($"成功研究 {Localization.Ins.Get(tech)}", items_);
                            }
                            else {
                                OnTap();
                            }

                        }, () => Globals.Ins.Values.Get(techType).Val >= techPointCount));
                    techShowed++;
                }
                else {
                    if (TechnologyResearched_Event.Event.TryGetValue(tech, out var action)) {
                        itemsUnlockedBuffer.Add(UIItem.CreateButton($"已研究 {techName}", () => {
                            var items_ = UI.Ins.GetItems();
                            items_.Add(UIItem.CreateReturnButton(OnTap));
                            action(items_);
                            UI.Ins.ShowItems($"成功研究 {Localization.Ins.Get(tech)}", items_);
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
                items.Add(UIItem.CreateText($"{Localization.Ins.Get(GetType())}的所有技术，已经全部被成功研究! "));
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
        protected abstract Type TechnologyType { get; }
    }
}

