

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{

    [Depend]
    public class Technology { }


    public abstract class AbstractTechnologyCenter : StandardTile
    {

        private static AudioClip soundEffectOnUnlockTech;
        public override void OnEnable() {
            base.OnEnable();

            if (soundEffectOnUnlockTech == null) {
                soundEffectOnUnlockTech = Sound.Ins.Get("mixkit-magic-potion-music-and-fx-2831");
            }
        }

        protected virtual void DecorateItems(List<IUIItem> items, Action onTap) { }

        private static List<IUIItem> itemsUnlockedBuffer = new List<IUIItem>();
        private List<IUIItem> items;
        public override void OnTap() {
            items = UI.Ins.GetItems();

            DecorateItems(items, OnTap);

            Type techType = TechnologyType;
            IValue techValue = Globals.Ins.Values.Get(techType);
            items.Add(UIItem.CreateValueProgress(techType, techValue));
            if (techValue.Inc > 0) {
                items.Add(UIItem.CreateTimeProgress(techType, techValue));
            }

            items.Add(UIItem.CreateSeparator());

            items.Add(UIItem.CreateStaticDestructButton(this));

            items.Add(UIItem.CreateSeparator());

            if (itemsUnlockedBuffer.Count != 0) throw new Exception();
            int techShowed = 0;
            foreach (var item in TechList) {
                Type tech = item.Item1;
                long techPointCount = item.Item2;

                bool hasTech = Globals.Ins.Bool(tech);
                if (!hasTech) {
                    items.Add(UIItem.CreateDynamicButton(techPointCount == 0 ? $"研究 {Localization.Ins.Get(tech)}" : 
                        $"研究 {Localization.Ins.Get(tech)} {Localization.Ins.Val(techType, -techPointCount)}", () => {

                        techValue.Val -= techPointCount;
                        Globals.Ins.Bool(tech, true);
                        OnTap();
                        Sound.Ins.Play(soundEffectOnUnlockTech);
                    }, () => Globals.Ins.Values.Get(techType).Val >= techPointCount));
                    techShowed++;
                }
                else {
                    itemsUnlockedBuffer.Add(UIItem.CreateText($"已研究 {Localization.Ins.Get(tech)}"));
                }
                if (techShowed >= ShowedTechToBeResearched) {
                    break;
                }
            }

            if (techShowed == 0) {
                items.Add(UIItem.CreateText("此科技建筑内，所有科技已经全部被成功研究！"));
            }

            items.Add(UIItem.CreateSeparator());

            foreach (var item in itemsUnlockedBuffer) {
                items.Add(item);
            }
            itemsUnlockedBuffer.Clear();


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

