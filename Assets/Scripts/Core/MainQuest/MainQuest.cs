
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
        private IRef currentQuest;
        private void Start() {
            questCompleteSound = Sound.Ins.Get("mixkit-video-game-treasure-2066");
            currentQuest = Globals.Ins.Refs.GetOrCreate<CurrentQuest>();
            if (currentQuest.Type == null) {
                currentQuest.Type = typeof(Quest.LandRocket);
                currentQuest.X = 0; // x for index
            }

            int index = 0;
            foreach (var quest in MainQuestConfig.QuestSequence) {
                QuestIndexes.Add(quest, index);
                index++;
            }
        }

        public bool IsQuestNotCompleted<T>() {
            return IsQuestNotCompleted(typeof(T));
        }
        public bool IsQuestNotCompleted(Type type) {
            return GetQuestIndexOf(currentQuest.Type) <= GetQuestIndexOf(type);
        }

        public int GetQuestIndexOf(Type type) {
            if (!QuestIndexes.TryGetValue(type, out int result)) {
                throw new Exception();
            }
            return result;
        }
        private readonly Dictionary<Type, int> QuestIndexes = new Dictionary<Type, int>();

        public void TryCompleteQuest(Type type) {
            if (currentQuest.Type == type) {
                Sound.Ins.Play(questCompleteSound);


                var items = UI.Ins.GetItems();
                items.Add(UIItem.CreateButton("查看下一个任务", () => {
                    OnTap();
                }));
                items.Add(UIItem.CreateSeparator());
                items.Add(UIItem.CreateText("以下为刚才已经完成的任务"));
                items.Add(UIItem.CreateSeparator());
                string title = MainQuestConfig.OnTapQuest[currentQuest.Type](items);
                if (title == null) {
                    title = "【任务目标完成】";
                } else {
                    title = $"【任务目标完成】{title}";
                }
                UI.Ins.ShowItems(title, items);


                currentQuest.X++;
                currentQuest.Type = MainQuestConfig.QuestSequence[(int)currentQuest.X];
            }
        }

        public void OnTap() {
            var items = UI.Ins.GetItems();
            if (!MainQuestConfig.OnTapQuest.TryGetValue(currentQuest.Type, out var func)) {
                throw new Exception($"没有配置任务内容{currentQuest.Type}");
            }
            string title = MainQuestConfig.OnTapQuest[currentQuest.Type](items);
            if (title == null) {
                title = "【任务进行中】";
            }
            else {
                title = $"【任务进行中】{title}";
            }
            UI.Ins.ShowItems(title, items);
        }
    }
}

