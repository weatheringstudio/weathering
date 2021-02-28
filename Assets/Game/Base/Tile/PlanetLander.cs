
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
    public class CharacterLanded { }

    public interface ILandable
    {
        bool Landable { get; }
        void Land(Vector2Int pos);
        void Leave();
    }

    public class PlanetLander : StandardTile, IStepOn, ILinkable, ILinkableConsumer
    {
        public override string SpriteKey => typeof(PlanetLander).Name;
        public override bool HasDynamicSpriteAnimation => true;
        public override string SpriteLeft => Refs.Has<IRight>() && Refs.Get<IRight>().Value > 0 ? typeof(Food).Name : null;
        public override string SpriteRight => Refs.Has<ILeft>() && Refs.Get<ILeft>().Value > 0 ? typeof(Food).Name : null;
        public override string SpriteUp => Refs.Has<IDown>() && Refs.Get<IDown>().Value > 0 ? typeof(Food).Name : null;
        public override string SpriteDown => Refs.Has<IUp>() && Refs.Get<IUp>().Value > 0 ? typeof(Food).Name : null;

        public void OnLink(Type direction) {
            questProgress.Inc = Res.Value;
        }
        public IRef Res { get; private set; }

        public (Type, long) CanConsume => (ConceptSupply.Get(questProgressRef.Type), long.MaxValue);

        public void OnStepOn() {
            ILandable landable = Map as ILandable;
            if (landable == null) throw new Exception();
            if (Res.Value == 0) {
                UI.Ins.ShowItems("是否乘坐火箭进入行星轨道",
                    UIItem.CreateButton("开启火箭", () => {
                        Map.UpdateAt<TerrainDefault>(Pos);
                        UI.Ins.Active = false;
                        landable.Leave();
                    }),
                    UIItem.CreateButton("暂不开启", () => {
                        UI.Ins.Active = false;
                    })
                );
            }
        }

        public override void OnConstruct() {
            base.OnConstruct();
            Refs = Weathering.Refs.GetOne();
            Res = Refs.Create<PlanetLander>();
        }

        private IValue questProgress;
        private IRef questProgressRef;
        public override void OnEnable() {
            base.OnEnable();
            Res = Refs.Get<PlanetLander>();
            questProgress = Globals.Ins.Values.GetOrCreate<QuestProgress>();
            questProgressRef = Globals.Ins.Refs.GetOrCreate<QuestProgress>();
        }

        public override void OnTap() {
            var items = UI.Ins.GetItems();

            items.Add(UIItem.CreateText($"当前任务：{Localization.Ins.Get(MainQuest.Ins.CurrentQuest)}"));

            if (questProgressRef.Type != null) {
                items.Add(UIItem.CreateValueProgress(questProgressRef.Type, questProgress));
                items.Add(UIItem.CreateTimeProgress(questProgressRef.Type, questProgress));

                items.Add(UIItem.CreateButton("完成当前任务", () => {
                    MainQuest.Ins.CompleteQuest(MainQuest.Ins.CurrentQuest);
                }, () => questProgress.Maxed));

                items.Add(UIItem.CreateSeparator());
                Res.Type = ConceptSupply.Get(questProgressRef.Type);
                LinkUtility.CreateButtons(items, this);
            }

            items.Add(UIItem.CreateSeparator());
            LinkUtility.CreateLinkInfo(items, Refs);

            UI.Ins.ShowItems("火箭", items);
        }
    }
}

