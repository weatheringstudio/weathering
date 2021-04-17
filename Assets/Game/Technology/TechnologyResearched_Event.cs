

using System;
using System.Collections.Generic;

namespace Weathering
{
    public static class TechnologyResearched_Event
    {
        public readonly static Dictionary<Type, Action<List<IUIItem>>> Event = new Dictionary<Type, Action<List<IUIItem>>> {
            {   
                typeof(KnowledgeOfGatheringBerry), (List<IUIItem> items) => {
                    items.Add(UIItem.CreateFAQText("如何采集浆果?", "点击森林, 点击探索, 点击采集"));
                    items.Add(UIItem.CreateFAQText("如何吃掉浆果?", $"点击屏幕右上角“地图仓库”, 点击{Localization.Ins.ValUnit<Berry>()}, 点击“使用”"));
                }

            },
            {
                typeof(KnowledgeOfGatheringBerryEfficiently), (List<IUIItem> items) => {
                    items.Add(UIItem.CreateText($"之前一次只能采集{Localization.Ins.Val<Berry>(1)}。"));
                    items.Add(UIItem.CreateText($"现在一次可以采集{Localization.Ins.Val<Berry>(5)}! "));
                }
            },
            {
                typeof(BerryBush), (List<IUIItem> items) => {
                    items.Add(UIItem.CreateText($"再也不用在森林里采集{Localization.Ins.ValUnit<Berry>()}了! "));
                    string name = Localization.Ins.Get<BerryBush>();
                    items.Add(UIItem.CreateFAQText($"如何建造{name}?", $"点击平原, 点击农业, 点击{name}"));
                    items.Add(UIItem.CreateFAQText($"如何采集{name}?", $"只要走上去, 听到一声响"));
                }
            },
            {
                typeof(KnowledgeOfHammer), (List<IUIItem> items) => {
                    items.Add(UIItem.CreateText($"有了锤子工具，轻松<复制>和<拆除>"));
                    string name = Localization.Ins.Get<BerryBush>();
                    string totem = Localization.Ins.Get<TotemOfNature>();
                    items.Add(UIItem.CreateFAQText($"如何复制{name}?", $"点击锤子, 点击{name}, 点击平原"));
                    items.Add(UIItem.CreateFAQText($"如何拆除{name}?", $"点击锤子, 点击{name}"));
                    items.Add(UIItem.CreateFAQText($"如何拆除{totem}?", $"点击锤子, 点击{totem}"));
                }
            },
            {
                typeof(TotemOfAncestors), (List<IUIItem> items) => {
                    string name = Localization.Ins.Get<TotemOfAncestors>();
                    items.Add(UIItem.CreateText($"{name}，可以用于召唤{Localization.Ins.ValUnit<Worker>()}"));
                    items.Add(UIItem.CreateFAQText($"如何建造{name}?", $"点击平原，点击科技，点击{name}"));
                }
            },
        };
    }
}
