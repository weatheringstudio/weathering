
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
    public class CharacterLanded { }

    public interface ILandable
    {
        bool Landed { get; }
        void Land(Vector2Int pos);
        void Leave(Vector2Int pos);
    }

    public class PlanetLanderRes { }

    public class PlanetLander : StandardTile, IStepOn, IIgnoreTool, IPassable
    {
        public bool Passable => true;

        public override string SpriteKey => typeof(PlanetLander).Name;
        public override string SpriteKeyHighLight => GlobalLight.Decorated(SpriteKey);
        //public override bool HasDynamicSpriteAnimation => true;
        //public override string SpriteLeft => Refs.Has<IRight>() && Refs.Get<IRight>().Value > 0 ? ConceptResource.Get(TypeOfResource.Type).Name : null;
        //public override string SpriteRight => Refs.Has<ILeft>() && Refs.Get<ILeft>().Value > 0 ? ConceptResource.Get(TypeOfResource.Type).Name : null;
        //public override string SpriteUp => Refs.Has<IDown>() && Refs.Get<IDown>().Value > 0 ? ConceptResource.Get(TypeOfResource.Type).Name : null;
        //public override string SpriteDown => Refs.Has<IUp>() && Refs.Get<IUp>().Value > 0 ? ConceptResource.Get(TypeOfResource.Type).Name : null;


        public bool IgnoreTool => true;

        public void OnStepOn() {
            // Res是以前火箭接入物流时用的
            if (Globals.Unlocked<KnowledgeOfPlanetLander>()) {
                LeavePlanet();
            }
        }

        public override void OnDestructWithMap() {
            LeavePlanet();
        }

        public void LeavePlanet() {
            ILandable landable = Map as ILandable;
            if (landable == null) throw new Exception();

            UI.Ins.Active = false;
            landable.Leave(Pos);
        }

        public static PlanetLander Ins { get; private set; }
        public override void OnConstruct(ITile tile) {

            base.OnConstruct(tile);
        }

        public override void OnDestruct(ITile newTile) {
            base.OnDestruct(newTile);
            Ins = null;
        }

        public override void OnEnable() {
            base.OnEnable();

            if (Ins != null) throw new Exception();
            Ins = this;

        }

        public override void OnTap() {
            var items = UI.Ins.GetItems();

            ILandable landable = Map as ILandable;
            if (landable == null) throw new Exception();

            if (Globals.Unlocked<KnowledgeOfPlanetLander>()) {
                items.Add(UIItem.CreateButton("开启飞船", () => {
                    LeavePlanet();
                }));
                items.Add(UIItem.CreateButton("暂不开启", () => {
                    UI.Ins.Active = false;
                }));
            } else {
                items.Add(UIItem.CreateMultilineText($"{Localization.Ins.Get<PlanetLander>()}已经坏了，需要研究{Localization.Ins.Get<KnowledgeOfPlanetLander>()}"));
            }

            items.Add(UIItem.CreateSeparator());
            items.Add(UIItem.CreateButton("星球时间重置", ResetPlanetPage));

            UI.Ins.ShowItems(Localization.Ins.Get<PlanetLander>(), items);
        }

        private void ResetPlanetPage() {
            var items = UI.Ins.GetItems();

            IMapDefinition mapDefinition = Map as IMapDefinition;
            if (mapDefinition == null) throw new Exception();

            bool canDelete = mapDefinition.CanDelete;
            items.Add(UIItem.CreateStaticButton("<color=#ff6666ff>确认重置星球</color>", () => {
                mapDefinition.Delete();
            }, canDelete));
            if (!canDelete) {
                items.Add(UIItem.CreateText($"无法重置星球时间，必须先关闭所有运行中的{Localization.Ins.Get<SpaceElevator>()}"));
            }

            UI.Ins.ShowItems("星球时间重置", items);
        }
    }
}

