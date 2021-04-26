
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

    public class ResetPointPop { }
    public class ResetPointPop100 { }
    public class ResetPointTool { }
    public class ResetPointMachine { }
    public class ResetPointLightMaterial { }

    public class ResetPointCircuit { }



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
            items.Add(UIItem.CreateButton("进行<color=#ff6666ff>时间回溯</color>", ResetPlanetPage));

            items.Add(UIItem.CreateSeparator());

            items.Add(UIItem.CreateText("飞船仪表盘还有读数："));

            IWeatherDefinition weather = Map as IWeatherDefinition;

            int hour = (int)(((weather.ProgressOfDay + 0.25f) % 1) * 24) + 1;
            string monthDescription = GeographyUtility.MonthTimeDescription(weather.MonthInYear + 1);
            string dateDescription = GeographyUtility.DateDescription(weather.DayInMonth + 1);
            string hourDescription = GeographyUtility.HourDescription(hour);
            items.Add(UIItem.CreateDynamicText(() => $"{weather.YearCount + 1}年 {monthDescription} {dateDescription} {hourDescription}"));

            items.Add(UIItem.CreateButton("星球参数", PlanetInfoPage));

            foreach (var revenue in RevenuesOfReset) {
                Type type = revenue.Item1;
                IValue value = Globals.Ins.Values.GetOrCreate(type);
                if (value.Max > 0) {
                    items.Add(UIItem.CreateValueProgress(type, value));
                }
            }

            UI.Ins.ShowItems(Localization.Ins.Get<PlanetLander>(), items);
        }

        private void PlanetInfoPage() {
            var items = UI.Ins.GetItems();

            items.Add(UIItem.CreateReturnButton(OnTap));

            IWeatherDefinition weather = Map as IWeatherDefinition;
            
            items.Add(UIItem.CreateDynamicText(() => $"风场强度 {Mathf.Round(weather.WindStrength * 100)}"));
            items.Add(UIItem.CreateDynamicText(() => $"相对湿度 {(int)(weather.Humidity * 100)}"));
            items.Add(UIItem.CreateDynamicText(() => $"迷雾浓度 {(int)(weather.FogDensity * 100)}"));
            items.Add(UIItem.CreateDynamicText(() => $"雨雪浓度 {(int)(Mathf.Max(weather.RainDensity, weather.SnowDensity) * 100)}"));

            items.Add(UIItem.CreateSeparator());

            int hour = (int)(((weather.ProgressOfDay + 0.25f) % 1) * 24) + 1;
            string dayTimeDescription = GeographyUtility.DayTimeDescription(weather.ProgressOfDay);
            items.Add(UIItem.CreateDynamicText(() => $"{weather.YearCount + 1}年 {weather.MonthInYear + 1}月 {weather.DayInMonth + 1}日 {dayTimeDescription} {hour} 点"));

            items.Add(UIItem.CreateText($"昼夜周期 {weather.SecondsForADay}秒"));
            items.Add(UIItem.CreateText($"四季周期 {weather.DaysForAYear}天"));
            items.Add(UIItem.CreateText($"月相周期 {weather.DaysForAMonth}天"));
            items.Add(UIItem.CreateText($"四季月相 {MapOfPlanet.MonthForAYear}月"));


            UI.Ins.ShowItems("星球参数", items);
        }

        private static List<(Type, Func<PlanetLander, long>)> RevenuesOfReset = new List<(Type, Func<PlanetLander, long>)> {
            (typeof(ResetPointPop), (PlanetLander pl) => pl.Map.Refs.GetOrCreate<Worker>().Value),
            (typeof(ResetPointPop100), (PlanetLander pl) => pl.Map.Refs.GetOrCreate<Worker>().Value/100),
            (typeof(ResetPointTool), (PlanetLander pl) => pl.Map.Refs.GetOrCreate<ToolPrimitive>().Value),
            (typeof(ResetPointMachine), (PlanetLander pl) => pl.Map.Refs.GetOrCreate<MachinePrimitive>().Value),
            (typeof(ResetPointLightMaterial), (PlanetLander pl) => pl.Map.Refs.GetOrCreate<LightMaterial>().Value),
            (typeof(ResetPointCircuit), (PlanetLander pl) => pl.Map.Refs.GetOrCreate<CircuitBoardAdvanced>().Value),
        };

        private void ResetPlanetPage() {
            var items = UI.Ins.GetItems();

            IMapDefinition mapDefinition = Map as IMapDefinition;
            if (mapDefinition == null) throw new Exception();


            long popCount = Map.Refs.GetOrCreate<Worker>().Value;



            int added = 0;
            foreach (var revenue in RevenuesOfReset) {
                Type type = revenue.Item1;
                long quantity = revenue.Item2(this);

                if (quantity > 0) {
                    IValue value = Globals.Ins.Values.GetOrCreate(type);
                    items.Add(UIItem.CreateText(Localization.Ins.Val(type, quantity)));
                    added++;
                }
            }
            if (added > 0) {
                items.Add(UIItem.CreateMultilineText($"本次回溯可获得上述天赋："));
            }

            bool canDelete = mapDefinition.CanDelete;
            items.Add(UIItem.CreateStaticButton("按下飞船上的<color=#ff6666ff>星球时间回溯</color>按钮， (星球将回到最初状态)", () => {
                mapDefinition.Delete();

                foreach (var revenue in RevenuesOfReset) {
                    Type type = revenue.Item1;
                    long quantity = revenue.Item2(this);

                    if (quantity > 0) {
                        IValue value = Globals.Ins.Values.GetOrCreate(type);
                        value.Max += quantity;
                        value.Val += quantity;
                    }
                }

            }, canDelete));
            if (!canDelete) {
                items.Add(UIItem.CreateText($"无法重置星球时间，必须先关闭所有运行中的{Localization.Ins.Get<SpaceElevator>()}"));
            }

            UI.Ins.ShowItems("星球时间重置", items);
        }
    }
}

