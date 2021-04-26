

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
                    string forest = Localization.Ins.Get<TerrainType_Forest>();

                    // 采集教程
                    items.Add(UIItem.CreateFAQText($"如何采集{name}?", $"点击{forest}, 点击探索, 点击采集"));
                    items.Add(UIItem.CreateFAQText($"什么是{forest}?", $"{forest}如下图所示"));
                    items.Add(UIItem.CreateTileImage("PlanetContinental_Tree"));
                    items.Add(UIItem.CreateSeparator());

                    // 使用道具教程（没什么道具好使用）
                    items.Add(UIItem.CreateFAQText($"如何吃掉{name}?", $"点击“星球资源仓库”, 点击{Localization.Ins.ValUnit<Berry>()}, 点击“使用”"));
                    items.Add(UIItem.CreateFAQText($"“星球资源仓库”在哪里?", $"如下图所示, 在主界面右上方"));
                    items.Add(UIItem.CreateTileImage("InventoryOfResourceIcon", 2));
                    items.Add(UIItem.CreateSeparator());

                    string sanity = Localization.Ins.ValUnit<Sanity>();
                    items.Add(UIItem.CreateFAQText($"吃掉{name}有什么用?", $"可以恢复角色{sanity}"));

                    // 点击角色教程
                    items.Add(UIItem.CreateFAQText($"如何查看角色{sanity}?", "点击角色"));
                    items.Add(UIItem.CreateFAQText($"角色长什么样?", "如下图所示"));
                    items.Add(UIItem.CreateTileImage("character"));

                    items.Add(UIItem.CreateFAQText($"为什么吃不下{name}?", "可能已经吃饱了"));
                }

            },
            {
                typeof(KnowledgeOfGatheringBerryEfficiently), (List<IUIItem> items) => {
                    string name = Localization.Ins.ValUnit<Berry>();
                    items.Add(UIItem.CreateText($"之前一次只能采集{Localization.Ins.Val<Berry>(1)}。"));
                    items.Add(UIItem.CreateText($"现在一次可以采集{Localization.Ins.Val<Berry>(5)}! "));

                    // 查看科技
                    items.Add(UIItem.CreateFAQText($"如何查看已经研究的科技介绍?", $"点击{Localization.Ins.Get<TotemOfNature>()}, 点击 “已研究【xxxx】”"));
                }
            },
            {
                typeof(BerryBush), (List<IUIItem> items) => {
                    string forest = Localization.Ins.Get<TerrainType_Forest>();
                    string plain = Localization.Ins.Get<TerrainType_Plain>();
                    items.Add(UIItem.CreateText($"再也不用在森林里采集{Localization.Ins.ValUnit<Berry>()}了! "));
                    string name = Localization.Ins.Get<BerryBush>();
                    string berry = Localization.Ins.ValUnit<Berry>();

                    // 建造教程
                    items.Add(UIItem.CreateFAQText($"如何建造{name}?", $"点击{plain}, 点击农业, 点击{name}"));
                    // 收集建筑资源教程
                    items.Add(UIItem.CreateFAQText($"如何采集{berry}?", $"只要走到{name}上, 听到一声响, 主界面左上方显示“获得{berry}”"));
                    items.Add(UIItem.CreateFAQText($"什么是“{plain}”?", $"{plain}如下图所示"));
                    items.Add(UIItem.CreateRawImage("Tutorial_BerryBush"));
                }
            },
            {
                typeof(KnowledgeOfHammer), (List<IUIItem> items) => {
                    GameMenu.Ins.SyncHammer();
                    string name = Localization.Ins.Get<BerryBush>();
                    string totem = Localization.Ins.Get<TotemOfNature>();
                    string hammer = Localization.Ins.Get<KnowledgeOfHammer>();
                    items.Add(UIItem.CreateText($"有了锤子工具, 轻松<color=yellow>复制</color>和<color=yellow>拆除</color>{name}。拆除返还全部建造资源, 放心拆除"));

                    items.Add(UIItem.CreateFAQText($"{hammer}在哪里?", $"如下图所示, 在主界面右方"));
                    items.Add(UIItem.CreateTileImage("ConstructDestructIcon"));

                    // 锤子使用教程
                    string plain = Localization.Ins.Get<TerrainType_Plain>();
                    items.Add(UIItem.CreateFAQText($"如何复制{name}?", $"点击{hammer}, 点击{name}, 点击{plain}"));
                    items.Add(UIItem.CreateFAQText($"如何拆除{name}?", $"点击{hammer}, 点击{name}"));
                    items.Add(UIItem.CreateFAQText($"如何拆除{totem}?", $"点击{hammer}, 点击{totem}"));

                }
            },
            {
                typeof(KnowledgeOfMagnet), (List<IUIItem> items) => {
                    GameMenu.Ins.SyncMagnet();
                    string grain = Localization.Ins.ValUnit<Grain>();
                    string wood = Localization.Ins.ValUnit<Wood>();
                    string magnet = Localization.Ins.Get<KnowledgeOfMagnet>();
                    string plain = Localization.Ins.Get<TerrainType_Plain>();
                    string forest = Localization.Ins.Get<TerrainType_Forest>();

                    // 磁铁使用教程
                    items.Add(UIItem.CreateFAQText($"如何获得{grain}?", $"点击{magnet}, 点击{plain}"));
                    items.Add(UIItem.CreateFAQText($"如何获得{wood}?", $"点击{magnet}, 点击{plain}"));
                    items.Add(UIItem.CreateFAQText($"如何使用{magnet}?", $"点击{magnet}, 点击{plain}和{forest}"));
                    items.Add(UIItem.CreateFAQText($"{magnet}在哪里?", $"如下图所示, 在主界面右方"));
                    items.Add(UIItem.CreateTileImage("LinkUnlinkIcon"));
                }
            },
            {
                typeof(TotemOfAncestors), (List<IUIItem> items) => {
                    string name = Localization.Ins.Get<TotemOfAncestors>();
                    string worker = Localization.Ins.ValUnit<Worker>();
                    string magnet = Localization.Ins.Get<KnowledgeOfMagnet>();
                    string grain = Localization.Ins.ValUnit<Grain>();
                    string plain = Localization.Ins.Get<TerrainType_Plain>();

                    // 新科技建筑

                    items.Add(UIItem.CreateText($"{name}, 可以用于召唤{worker}"));
                    items.Add(UIItem.CreateFAQText($"{worker}有什么用?", $"{worker}可以在建筑里帮玩家工作, 自动生产各种资源"));
                    items.Add(UIItem.CreateFAQText($"如何建造{name}?", $"点击{plain}, 点击科技, 点击{name}"));
                    items.Add(UIItem.CreateFAQText($"如何获得{grain}?", $"点击{magnet}, 点击{plain}"));
                }
            },


            // ancestors
            {
                typeof(ResidenceOfGrass), (List<IUIItem> items) => {
                    string name = Localization.Ins.ValUnit<ResidenceOfGrass>();
                    string magnet = Localization.Ins.Get<KnowledgeOfMagnet>();
                    string worker = Localization.Ins.ValUnit<Worker>();
                    string berrybush = Localization.Ins.Get<BerryBush>();
                    string plain = Localization.Ins.Get<TerrainType_Plain>();

                    items.Add(UIItem.CreateFAQText($"如何建造{name}?", $"点击{plain}, 点击住房, 点击{name}"));
                    items.Add(UIItem.CreateFAQText($"{name}有什么用?", $"如果{name}旁边有3个{berrybush}, 就能产生{worker}!"));

                    items.Add(UIItem.CreateFAQText($"如何产生{worker}?", $"点击{magnet}, 点击{name}, 如果足够的{Localization.Ins.ValUnit<Food>()}被吸引到{name}里, 则成功产生{worker}"));
                    items.Add(UIItem.CreateRawImage("Tutorial_ResidenceOfGrass"));

                    items.Add(UIItem.CreateFAQText($"如何判断{name}是否正常运转?", $"如下图所示, 右下的{name}缺乏食物, 左上的{name}正常运转"));
                    items.Add(UIItem.CreateRawImage("Tutorial_ResidenceOfGrass_1"));

                    items.Add(UIItem.CreateFAQText($"为什么是3个{berrybush}?", $"点击{berrybush}或{name}, 点击“建筑功能”, 查看建筑运转所需资源"));
                    items.Add(UIItem.CreateRawImage("Tutorial_ResidenceOfGrass_2"));

                    items.Add(UIItem.CreateFAQText($"如何查看{worker}?", $"点击“星球盈余产出”"));
                    items.Add(UIItem.CreateFAQText($"“星球盈余产出”在哪里?", $"如下图所示, 在主界面右上方"));
                    items.Add(UIItem.CreateTileImage("InventoryOfSupplyIcon"));
                    items.Add(UIItem.CreateSeparator());
                }
            },

            {
                typeof(Farm), (List<IUIItem> items) => {
                    string name = Localization.Ins.ValUnit<Farm>();
                    string grain = Localization.Ins.ValUnit<Grain>();
                    string worker = Localization.Ins.ValUnit<Worker>();
                    string plain = Localization.Ins.Get<TerrainType_Plain>();

                    items.Add(UIItem.CreateFAQText($"如何建造{name}?", $"点击{plain}, 点击农业, 点击{name}"));
                    items.Add(UIItem.CreateFAQText($"如何采集{grain}?", $"只要走到{name}上, 听到一声响, 主界面左上方显示“获得{grain}”"));

                    items.Add(UIItem.CreateFAQText($"如何判断{name}是否正常运转?", $"如下图所示, 左边的{name}无人工作, 右边的{name}正常运转"));
                    items.Add(UIItem.CreateRawImage("Tutorial_Farm"));
                    items.Add(UIItem.CreateSeparator());

                    items.Add(UIItem.CreateFAQText($"为什么{name}不正常运转?", $"因为“星球盈余产出”里没有空闲居民, 居民会帮忙种田"));
                    items.Add(UIItem.CreateFAQText($"已经有了空闲{worker}, 为什么{name}还是绿色的", $"用磁铁工具点击{name}, 可以把空闲{worker}吸引来工作"));
                }
            },

            {
                typeof(WareHouseOfGrass), (List<IUIItem> items) => {
                    string name = Localization.Ins.ValUnit<WareHouseOfGrass>();
                    string berrybush = Localization.Ins.Get<BerryBush>();
                    string farm = Localization.Ins.Get<Farm>();
                    string magnet = Localization.Ins.Get<KnowledgeOfMagnet>();
                    string plain = Localization.Ins.Get<TerrainType_Plain>();

                    items.Add(UIItem.CreateFAQText($"如何建造{name}?", $"点击{plain}, 点击物流, 点击仓库, 点击{name}"));
                    items.Add(UIItem.CreateFAQText($"{name}有什么用?", $"{name}能储存很多资源, 只要建立在{berrybush}或运行的{farm}旁边"));


                    items.Add(UIItem.CreateFAQText($"如何使用{name}?", $"如下图所示, 使用{magnet}, {name}可以储存各类物资"));
                    items.Add(UIItem.CreateRawImage("Tutorial_WareHouseOfGrass"));
                    items.Add(UIItem.CreateSeparator());

                    items.Add(UIItem.CreateFAQText($"如何收集{name}内资源?", $"只要走到{name}上, 听到一声响, 主界面左上方显示“获得【XXX】”"));

                }
            },

            {
                typeof(RoadForSolid), (List<IUIItem> items) => {
                    string name = Localization.Ins.ValUnit<RoadForSolid>();
                    string berrybush = Localization.Ins.Get<BerryBush>();
                    string farm = Localization.Ins.Get<Farm>();
                    string magnet = Localization.Ins.Get<KnowledgeOfMagnet>();
                    string plain = Localization.Ins.Get<TerrainType_Plain>();

                    items.Add(UIItem.CreateFAQText($"如何建造{name}?", $"点击{plain}, 点击物流, 点击{name}"));
                    items.Add(UIItem.CreateFAQText($"{name}有什么用?", $"{name}能短途运输资源"));

                    items.Add(UIItem.CreateFAQText($"如何使用{name}?", $"如下图所示, 使用{magnet}, 可以让资源沿着{name}运输"));
                    items.Add(UIItem.CreateRawImage("Tutorial_RoadForSolid"));
                    items.Add(UIItem.CreateSeparator());

                    string hammer = Localization.Ins.Get<KnowledgeOfHammer>();
                    items.Add(UIItem.CreateMultilineText($"用{magnet}建立了连接的建筑, 无法用{hammer}拆除。想要拆除, 需要先用{magnet}解除连接"));
                }
            },

            {
                typeof(ForestLoggingCamp), (List<IUIItem> items) => {
                    string name = Localization.Ins.ValUnit<ForestLoggingCamp>();
                    string magnet = Localization.Ins.Get<KnowledgeOfMagnet>();
                    string forest = Localization.Ins.Get<TerrainType_Forest>();

                    items.Add(UIItem.CreateFAQText($"如何获得{Localization.Ins.Val<Wood>(10)}?", $"点击{magnet}, 点击{forest}"));
                    items.Add(UIItem.CreateFAQText($"如何建造{name}?", $"点击{forest}, 点击林业, 点击{name}"));

                    items.Add(UIItem.CreateFAQText($"如何取出{name}内资源?", $"如下图所示, 建造{Localization.Ins.Get<WareHouseOfGrass>()}, 使用{magnet}"));
                    items.Add(UIItem.CreateRawImage("Tutorial_ForestLoggingCamp"));
                    items.Add(UIItem.CreateSeparator());

                }
            },

            {
                typeof(WorkshopOfPaperMaking), (List<IUIItem> items) => {
                    string name = Localization.Ins.ValUnit<WorkshopOfPaperMaking>();
                    string magnet = Localization.Ins.Get<KnowledgeOfMagnet>();

                    items.Add(UIItem.CreateFAQText($"如何建造{name}?", $"点击{Localization.Ins.Get<TerrainType_Plain>()}, 点击工业, 点击制造业, 点击{name}"));

                    items.Add(UIItem.CreateFAQText($"如何运行{name}?", $"如下图所示,  建造{Localization.Ins.Get<ForestLoggingCamp>()}, 使用{magnet}"));
                    items.Add(UIItem.CreateRawImage("Tutorial_WorkshopOfPaperMaking"));
                    items.Add(UIItem.CreateSeparator());
                }
            },

            {
                typeof(WorkshopOfBook), (List<IUIItem> items) => {
                    string name = Localization.Ins.ValUnit<WorkshopOfBook>();
                    string plain = Localization.Ins.Get<TerrainType_Plain>();
                    items.Add(UIItem.CreateFAQText($"如何建造{name}?", $"方法1: 点击{plain}, 点击工业, 点击制造业, 点击{name}\n方法2: 点击{plain}, 点击科技, 点击配套设施, 点击{name}"));
                }
            },

            {
                typeof(LibraryOfAll), (List<IUIItem> items) => {
                    string name = Localization.Ins.ValUnit<LibraryOfAll>();
                    items.Add(UIItem.CreateFAQText($"如何建造{name}?", $"点击平原, 点击科技, 点击{name}"));
                    items.Add(UIItem.CreateFAQText($"如何为{name}提供{Localization.Ins.ValUnit<Book>()}?", $"如下图, 右下角是{name}"));
                    items.Add(UIItem.CreateRawImage("Tutorial_LibraryOfAll"));
                    items.Add(UIItem.CreateSeparator());
                }
            },

            // library
            {
                typeof(LibraryOfAgriculture), (List<IUIItem> items) => {
                    string name = Localization.Ins.ValUnit<LibraryOfAgriculture>();
                    items.Add(UIItem.CreateFAQText($"如何建造{name}?", $"点击平原, 点击科技, 点击{name}"));
                    items.Add(UIItem.CreateFAQText($"如何运行{name}?", $"建设{Localization.Ins.Get<Farm>()}, 使用{Localization.Ins.Get<KnowledgeOfMagnet>()}"));
                }
            },
            {
                typeof(LibraryOfGeography), (List<IUIItem> items) => {
                    string name = Localization.Ins.ValUnit<LibraryOfGeography>();
                    items.Add(UIItem.CreateFAQText($"{name}有什么用?", $"消耗{Localization.Ins.ValUnit<Stone>()}, 解锁矿产, 如{Localization.Ins.ValUnit<Clay>()}{Localization.Ins.ValUnit<IronOre>()}"));
                    items.Add(UIItem.CreateFAQText($"如何运行{name}?", $"建设{Localization.Ins.Get<MountainQuarry>()}, 使用{Localization.Ins.Get<KnowledgeOfMagnet>()}"));
                }
            },
            {
                typeof(LibraryOfHandcraft), (List<IUIItem> items) => {
                    string name = Localization.Ins.ValUnit<LibraryOfHandcraft>();
                    items.Add(UIItem.CreateFAQText($"{name}有什么用?", $"解锁手工业, 如{Localization.Ins.ValUnit<WorkshopOfToolPrimitive>()}{Localization.Ins.ValUnit<MachinePrimitive>()}"));
                }
            },
            {
                typeof(LibraryOfLogistics), (List<IUIItem> items) => {
                    string name = Localization.Ins.ValUnit<LibraryOfLogistics>();
                    items.Add(UIItem.CreateFAQText($"{name}有什么用?", $"解锁新物流方式, 如{Localization.Ins.Get<RoadAsBridge>()}{Localization.Ins.Get<RoadAsTunnel>()}{Localization.Ins.Get<TransportStationSimpliest>()}"));
                    items.Add(UIItem.CreateText($"{name}包含许多关键科技"));
                    items.Add(UIItem.CreateText($"解锁{Localization.Ins.Get<RoadAsBridge>()}, 可以连接新岛屿"));
                    items.Add(UIItem.CreateText($"解锁{Localization.Ins.Get<RoadAsTunnel>()}, 可以深入山地采矿"));
                    items.Add(UIItem.CreateText($"解锁{Localization.Ins.Get<TransportStationSimpliest>()}, 可以方便地运输资源"));
                }
            },
            {
                typeof(LibraryOfEconomy), (List<IUIItem> items) => {
                    string name = Localization.Ins.ValUnit<LibraryOfEconomy>();
                    items.Add(UIItem.CreateFAQText($"{name}有什么用?", $"解锁市场交易"));
                    items.Add(UIItem.CreateText($"{name}解锁的市场交易一般都是亏本的, 但比较灵活"));
                }
            },
            {
                typeof(LibraryOfConstruction), (List<IUIItem> items) => {
                    string name = Localization.Ins.ValUnit<LibraryOfConstruction>();
                    items.Add(UIItem.CreateFAQText($"{name}有什么用?", $"解锁各种住房, 如{Localization.Ins.ValUnit<ResidenceOfWood>()}{Localization.Ins.ValUnit<WareHouseOfWood>()}"));
                }
            },
            {
                typeof(LibraryOfMetalWorking), (List<IUIItem> items) => {
                    string name = Localization.Ins.ValUnit<LibraryOfMetalWorking>();
                    items.Add(UIItem.CreateFAQText($"{name}有什么用?", $"解锁冶金工业, 如{Localization.Ins.ValUnit<WorkshopOfCopperSmelting>()}"));
                }
            },



            {
                typeof(MountainQuarry), (List<IUIItem> items) => {
                    string name = Localization.Ins.ValUnit<MountainQuarry>();
                    items.Add(UIItem.CreateFAQText($"如何建造{name}?", $"如下图，点击{Localization.Ins.Get<TerrainType_Mountain>()}"));
                    items.Add(UIItem.CreateRawImage("Tutorial_MountainQuarry"));
                }
            },
            {
                typeof(RoadAsTunnel), (List<IUIItem> items) => {
                    string name = Localization.Ins.ValUnit<RoadAsTunnel>();
                    items.Add(UIItem.CreateFAQText($"{name}有什么用?", $"如下图, {name}可以建在{Localization.Ins.Get<TerrainType_Mountain>()}，运输物品"));
                    items.Add(UIItem.CreateRawImage("Tutorial_RoadAsTunnel"));
                }
            },
            {
                typeof(MineOfCopper), (List<IUIItem> items) => {
                    string name = Localization.Ins.ValUnit<MineOfCopper>();
                    items.Add(UIItem.CreateFAQText($"如何建造{name}?", $"如下图，点击有矿石的{Localization.Ins.Get<TerrainType_Mountain>()}"));
                    items.Add(UIItem.CreateRawImage("Tutorial_MineOfCopper"));
                }
            },
            {
                typeof(RoadAsBridge), (List<IUIItem> items) => {
                    string name = Localization.Ins.ValUnit<RoadAsBridge>();
                    items.Add(UIItem.CreateFAQText($"{name}有什么用?", $"如下图, {name}可以建在{Localization.Ins.Get<TerrainType_Sea>()}，运输物品"));
                    items.Add(UIItem.CreateRawImage("Tutorial_RoadAsBridge"));
                }
            },
            {
                typeof(WorkshopOfCopperSmelting), (List<IUIItem> items) => {
                    string name = Localization.Ins.ValUnit<WorkshopOfCopperSmelting>();
                    string fuel = Localization.Ins.ValUnit<Fuel>();
                    items.Add(UIItem.CreateFAQText($"如何获得{fuel}?", $"{Localization.Ins.ValUnit<Wood>()}是燃料的一种。点击建造，点击建筑控制，点击{fuel}，可以查看所有属于燃料的资源"));

                    items.Add(UIItem.CreateFAQText($"如何运行{name}?", $"如下图"));
                    items.Add(UIItem.CreateRawImage("Tutorial_WorkshopOfCopperSmelting"));
                }
            },

            // school
            {
                typeof(SchoolOfAll), (List<IUIItem> items) => {
                    string name = Localization.Ins.ValUnit<SchoolOfAll>();
                    items.Add(UIItem.CreateText($"{name}通过消耗{Localization.Ins.ValUnit<SchoolEquipment>()}, 能够解锁许多学园区域"));
                }
            },
        };
    }
}
