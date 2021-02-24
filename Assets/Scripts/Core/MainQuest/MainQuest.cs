﻿
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
    public class CurrentQuest { }

    public class MainQuest : MonoBehaviour
    {
        public static MainQuest Ins { get; private set; }
        private void Awake() {
            if (Ins != null) throw new Exception();
            Ins = this;
        }

        private AudioClip questCompleteSound;
        private AudioClip questCanBeCompletedSound;
        private IRef currentQuest;
        private void Start() {
            questCompleteSound = Sound.Ins.Get("mixkit-magic-potion-music-and-fx-2831");
            questCanBeCompletedSound = Sound.Ins.Get("mixkit-positive-notification-951");
            currentQuest = Globals.Ins.Refs.GetOrCreate<CurrentQuest>();
            if (currentQuest.Type == null) {
                currentQuest.Type = typeof(Quest_LandRocket);
                currentQuest.X = 0; // x for index
            }
        }

        public bool IsQuestCompleted<T>() {
            return Globals.Ins.Bool<T>();
        }

        public void CompleteQuest<T>() {
            CompleteQuest(typeof(T));
        }
        public void CompleteQuest(Type type) {
            if (currentQuest.Type == type) {
                CompleteQuest();
            }
            else if (!Globals.Ins.Bool(type)) {
                Globals.Ins.Bool(type, true);
                //if (MainQuestConfig.Ins.CheckQuestCanBeCompleted.TryGetValue(currentQuest.Type, out Func<bool> func)) {
                //    if (func()) {
                //        // do nothing
                //    }
                //}
                Sound.Ins.Play(questCanBeCompletedSound);
            }
        }

        private void CompleteQuest() {
            Sound.Ins.Play(questCompleteSound);
            Type oldQuest = currentQuest.Type;
            Type newQuest = MainQuestConfig.Ins.QuestSequence[(int)currentQuest.X + 1];
            string questNameOld = Localization.Ins.Get(oldQuest);
            string questNameNew = Localization.Ins.Get(newQuest);
            currentQuest.X++;
            currentQuest.Type = newQuest;

            var items = UI.Ins.GetItems();
            items.Add(UIItem.CreateButton($"查看下一个任务 {questNameNew}", () => {
                OnTap();
            }));

            items.Add(UIItem.CreateSeparator());
            items.Add(UIItem.CreateText($"以下为刚才已经完成的任务 {questNameOld}"));
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
            }
            else {
                throw new Exception($"没有配置任务内容{currentQuest.Type}");
            }
            string title = Localization.Ins.Get(currentQuest.Type);
            if (title == null) {
                title = $"【任务进行中】";
            }
            else {
                title = $"【任务进行中】{title}";
            }
            UI.Ins.ShowItems(title, items);
        }
    }
}
