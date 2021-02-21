
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{

    namespace Quest
    {
        public class LandRocket { }
        public class GatherLocalResources { }
        public class CongratulationsQuestAllCompleted { }
    }


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
                title = "【任务】";
            }
            else {
                title = $"【任务】{title}";
            }
            UI.Ins.ShowItems(title, items);
        }
    }
}

