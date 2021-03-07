
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
    public class QuestResource { }
    public class QuestRequirement { }
    public class CurrentQuest { }

    public class MainQuest : MonoBehaviour
    {
        public readonly static Type StartingQuest = typeof(Quest_LandRocket);

        public static MainQuest Ins { get; private set; }
        private void Awake() {
            if (Ins != null) throw new Exception();
            Ins = this;
        }

        private AudioClip questCompleteSound;
        private AudioClip questCanBeCompletedSound;
        private IRef currentQuest;
        public Type CurrentQuest { get => currentQuest.Type; }

        private void Start() {
            questCompleteSound = Sound.Ins.Get("mixkit-magic-potion-music-and-fx-2831");
            questCanBeCompletedSound = Sound.Ins.Get("mixkit-positive-notification-951");
            currentQuest = Globals.Ins.Refs.GetOrCreate<CurrentQuest>();
            if (currentQuest.Type == null) {
                MainQuestConfig.Ins.OnStartQuest.TryGetValue(StartingQuest, out Action action);
                action?.Invoke();
                currentQuest.Type = StartingQuest;
                currentQuest.Value = MainQuestConfig.Ins.GetIndex(StartingQuest); // x for index
                Globals.Ins.Bool(StartingQuest, true);
            }
        }

        public bool IsUnlocked<T>() {
            return Globals.Ins.Bool<T>();
        }

        public void CompleteQuest<T>() {
            CompleteQuest(typeof(T));
        }
        public void CompleteQuest(Type type) {
            if (currentQuest.Type == type) {
                CompleteQuest();
            }
            //else if (!Globals.Ins.Bool(type)) {
            //    Globals.Ins.Bool(type, true);
            //    // Sound.Ins.Play(questCanBeCompletedSound);
            //}
        }

        private void CompleteQuest() {
            Sound.Ins.Play(questCompleteSound); // 任务完成音效
            Type oldQuest = currentQuest.Type; // 刚完成的任务
            Type newQuest = MainQuestConfig.Ins.QuestSequence[(int)currentQuest.Value + 1]; // 新的任务
            string questNameOld = Localization.Ins.Get(oldQuest);
            string questNameNew = Localization.Ins.Get(newQuest);
            Globals.Ins.Bool(newQuest, true); // 任何已完成或者正在进行的任务，标记
            currentQuest.Value++; // 主线任务下标
            currentQuest.Type = newQuest; // 保存正在进行的主线任务

            IRef questResource = Globals.Ins.Refs.GetOrCreate<QuestResource>();
            questResource.Type = null; // 需求任务消耗物品类型：无
            questResource.Value = 0; // 需求任务消耗物品数量：0
            Globals.Ins.Refs.GetOrCreate<QuestRequirement>().Type = null; // 需求任务用有物品：空
            MainQuestConfig.Ins.OnStartQuest.TryGetValue(newQuest, out Action action);
            action?.Invoke(); // 设置新任务需求任务物品

            var items = UI.Ins.GetItems();
            items.Add(UIItem.CreateBanner("questComplete")); // “任务完成”横幅
            items.Add(UIItem.CreateButton($"查看下一个任务 {questNameNew}", () => {
                OnTap();
            }));

            items.Add(UIItem.CreateSeparator());
            items.Add(UIItem.CreateText($"回顾：刚才完成的任务 {questNameOld}"));
            items.Add(UIItem.CreateSeparator());
            MainQuestConfig.Ins.OnTapQuest[oldQuest](items);
            if (questNameOld == null) {
                questNameOld = $"【任务目标完成】";
            } else {
                questNameOld = $"【任务目标完成】{questNameOld}";
            }
            UI.Ins.ShowItems(questNameOld, items);
        }

        public void OnTap() {
            var items = UI.Ins.GetItems();
            if (MainQuestConfig.Ins.OnTapQuest.TryGetValue(currentQuest.Type, out var func)) {
                func(items);
            } else {
                throw new Exception($"没有配置任务内容{currentQuest.Type}");
            }

            IMap map = MapView.Ins.TheOnlyActiveMap;

            IValue ValueOfResource = Globals.Ins.Values.GetOrCreate<QuestResource>();
            IRef TypeOfResource = Globals.Ins.Refs.GetOrCreate<QuestResource>();
            IValue ValueOfRequirement = Globals.Ins.Values.GetOrCreate<QuestRequirement>();
            IRef TypeOfRequirement = Globals.Ins.Refs.GetOrCreate<QuestRequirement>();

            if (TypeOfResource.Type != null) {
                items.Add(UIItem.CreateButton("提交任务", () => {
                    CompleteQuest(CurrentQuest);
                }, () => ValueOfResource.Maxed)); // 资源任务的提交条件：ValueOfResource.Maxed

                items.Add(UIItem.CreateButton($"提交任务物品{Localization.Ins.ValUnit(TypeOfResource.Type)}", () => {
                    long quantity = Math.Min(ValueOfResource.Max - ValueOfResource.Val, map.Inventory.GetWithTag(TypeOfResource.Type));
                    map.Inventory.RemoveWithTag(TypeOfResource.Type, quantity);
                    ValueOfResource.Val += quantity;
                }, () => !ValueOfResource.Maxed));

                items.Add(UIItem.CreateValueProgress(TypeOfResource.Type, ValueOfResource));

                items.Add(UIItem.CreateText("背包里的相关物品"));
                UIItem.AddEntireInventoryContentWithTag(TypeOfResource.Type, map.Inventory, items, OnTap);
            }

            if (TypeOfRequirement.Type != null) {

                long quantity = map.Inventory.GetWithTag(TypeOfRequirement.Type);
                ValueOfRequirement.Val = quantity;

                items.Add(UIItem.CreateButton("提交任务", () => {
                   CompleteQuest(CurrentQuest);
                }, () => ValueOfRequirement.Maxed));

                items.Add(UIItem.CreateText("已有【任务相关物品】如下"));
                long count = UIItem.AddEntireInventoryContentWithTag(TypeOfRequirement.Type, map.Inventory, items, OnTap);
                if (count == 0) {
                    items.Add(UIItem.CreateText("没有任何【任务相关物品】"));
                }
            }

            string title = Localization.Ins.Get(currentQuest.Type);
            if (title == null) {
                title = $"【任务进行中】";
            } else {
                title = $"【任务进行中】{title}";
            }
            UI.Ins.ShowItems(title, items);
        }
    }
}

