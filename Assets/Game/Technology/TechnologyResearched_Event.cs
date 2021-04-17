

using System;
using System.Collections.Generic;

namespace Weathering
{
    public static class TechnologyResearched_Event
    {
        public readonly static Dictionary<Type, Action<List<IUIItem>>> Event = new Dictionary<Type, Action<List<IUIItem>>> {
            {   
                typeof(KnowledgeOfGatheringBerry), (List<IUIItem> items) => {
                    string name = Localization.Ins.ValUnit<Berry>();
                    items.Add(UIItem.CreateFAQText($"如何采集{name}?", "点击森林, 点击探索, 点击采集"));
                    items.Add(UIItem.CreateFAQText($"什么是“森林”?", $"森林如下图所示"));
                    items.Add(UIItem.CreateTileImage("PlanetLandForm_Tree"));
                    items.Add(UIItem.CreateSeparator());

                    items.Add(UIItem.CreateFAQText($"如何吃掉{name}?", $"点击“星球资源仓库”, 点击{Localization.Ins.ValUnit<Berry>()}, 点击“使用”"));
                    items.Add(UIItem.CreateFAQText($"什么是“星球资源仓库”?", $"如下图所示，在主界面右上方"));
                    items.Add(UIItem.CreateTileImage("InventoryOfResourceIcon"));
                    items.Add(UIItem.CreateSeparator());

                    items.Add(UIItem.CreateFAQText($"吃掉{name}有什么用?", "可以恢复体力"));
                    items.Add(UIItem.CreateFAQText($"如何查看当前体力?", "点击角色"));
                    items.Add(UIItem.CreateFAQText($"角色是什么?", "如下图所示"));
                    items.Add(UIItem.CreateTileImage("character"));
                    items.Add(UIItem.CreateSeparator());

                    items.Add(UIItem.CreateFAQText($"为什么吃不下浆果?", "可能已经吃饱了"));
                }

            },
            {
                typeof(KnowledgeOfGatheringBerryEfficiently), (List<IUIItem> items) => {
                    string name = Localization.Ins.ValUnit<Berry>();
                    items.Add(UIItem.CreateText($"之前一次只能采集{Localization.Ins.Val<Berry>(1)}。"));
                    items.Add(UIItem.CreateText($"现在一次可以采集{Localization.Ins.Val<Berry>(5)}! "));
                    items.Add(UIItem.CreateFAQText($"如何查看已经研究的科技介绍?", $"点击建筑，点击 “已研究【XXX】”"));
                }
            },
            {
                typeof(BerryBush), (List<IUIItem> items) => {
                    items.Add(UIItem.CreateText($"再也不用在森林里采集{Localization.Ins.ValUnit<Berry>()}了! "));
                    string name = Localization.Ins.Get<BerryBush>();
                    items.Add(UIItem.CreateFAQText($"如何建造{name}?", $"点击平原, 点击农业, 点击{name}"));
                    items.Add(UIItem.CreateFAQText($"如何采集{name}?", $"只要走上去, 听到一声响，主界面左边显示“获得XXX”"));
                    items.Add(UIItem.CreateFAQText($"什么是“平原”?", $"平原如下图所示"));
                    items.Add(UIItem.CreateTileImage("PlanetContinental_Grass_16"));
                }
            },
            {
                typeof(KnowledgeOfHammer), (List<IUIItem> items) => {
                    GameMenu.Ins.SyncHammer();
                    string name = Localization.Ins.Get<BerryBush>();
                    string totem = Localization.Ins.Get<TotemOfNature>();
                    string hammer = Localization.Ins.Get<KnowledgeOfHammer>();
                    items.Add(UIItem.CreateText($"有了锤子工具, 轻松<复制>和<拆除>{name}"));

                    items.Add(UIItem.CreateFAQText($"如何复制{name}?", $"点击{hammer}, 点击{name}, 点击平原"));
                    items.Add(UIItem.CreateFAQText($"如何拆除{name}?", $"点击{hammer}, 点击{name}"));
                    items.Add(UIItem.CreateFAQText($"如何拆除{totem}?", $"点击{hammer}, 点击{totem}"));
                    items.Add(UIItem.CreateFAQText($"{hammer}在哪里?", $"如下图所示，在主界面右方"));
                    items.Add(UIItem.CreateTileImage("ConstructDestructIcon"));
                }
            },
            {
                typeof(TotemOfAncestors), (List<IUIItem> items) => {
                    string name = Localization.Ins.Get<TotemOfAncestors>();
                    items.Add(UIItem.CreateText($"{name}, 可以用于召唤{Localization.Ins.ValUnit<Worker>()}"));
                    items.Add(UIItem.CreateFAQText($"如何建造{name}?", $"点击平原, 点击科技, 点击{name}"));
                }
            },

            // ancestors
            {
                typeof(KnowledgeOfMagnet), (List<IUIItem> items) => {
                    GameMenu.Ins.SyncMagnet();
                    string name = Localization.Ins.ValUnit<Grain>();
                    string magnet = Localization.Ins.Get<KnowledgeOfMagnet>();
                    items.Add(UIItem.CreateFAQText($"如何获得{name}?", $"点击{magnet}, 点击平原/森林"));
                    items.Add(UIItem.CreateFAQText($"如何使用{magnet}?", $"点击{magnet}, 点击平原/森林"));
                    items.Add(UIItem.CreateFAQText($"{magnet}在哪里?", $"如下图所示，在主界面右方"));
                    items.Add(UIItem.CreateTileImage("LinkUnlinkIcon"));
                }
            },
            {
                typeof(ResidenceOfGrass), (List<IUIItem> items) => {
                    string name = Localization.Ins.ValUnit<ResidenceOfGrass>();
                    string magnet = Localization.Ins.Get<KnowledgeOfMagnet>();
                    string worker = Localization.Ins.Get<Worker>();
                    string berrybush = Localization.Ins.Get<BerryBush>();
                    items.Add(UIItem.CreateFAQText($"如何建造{name}?", $"点击平原, 点击住房"));
                    items.Add(UIItem.CreateFAQText($"{name}有什么用?", $"如果{name}旁边有3个{berrybush}, 就能产生{worker}!"));
                    items.Add(UIItem.CreateFAQText($"为什么是3个?", $"点击{berrybush}或{name}，点击“建筑功能”, 查看比例"));
                    items.Add(UIItem.CreateFAQText($"如何产生{worker}?", $"点击{magnet}, 点击{name}"));
                }
            },
        };
    }
}
