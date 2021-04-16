
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
    [Concept]
    public interface ILeft { }
    [Concept]
    public interface IRight { }
    [Concept]
    public interface IUp { }
    [Concept]
    public interface IDown { }

    public interface ILinkConsumer : ITile
    {
        void Consume(List<IRef> refs);
    }

    public interface ILinkProvider : ITile
    {
        void Provide(List<IRef> refs);
    }

    public interface ILinkEvent : ITile
    {
        void OnLink(Type direction, long quantity);
    }

    public interface ILinkQuantityRestriction : ITile
    {
        long LinkQuantityRestriction { get; }
    }

    public interface ILinkTypeRestriction : ITile
    {
        Type LinkTypeRestriction { get; }
    }

    public interface ILinkTileTypeRestriction : ITile
    {
        Type LinkTileTypeRestriction { get; }
    }

    public interface IRunnable
    {
        bool Running { get; }
        bool CanRun();
        void Run();
        bool CanStop();
        void Stop();
    }

    public static class LinkUtility
    {


        private readonly static List<Type> directions = new List<Type>() {
            typeof(IUp), typeof(IDown),typeof(ILeft), typeof(IRight),
        };

        /// <summary>
        /// 创建一个IRef对应文本
        /// </summary>
        public static UIItem CreateRefText(IRef pair) {
            if (pair.Type == null) return UIItem.CreateText($"本地内容【无】");
            return UIItem.CreateText($"本地内容{Localization.Ins.Val(pair.Type, pair.Value)}");
            //if (pair.BaseValue == long.MaxValue) return UIItem.CreateText($"本地内容{Localization.Ins.Val(pair.Type, pair.Value)}");
            //return UIItem.CreateText($"本地内容{Localization.Ins.Val(pair.Type, pair.Value)} 内容容量{Localization.Ins.Val(pair.Type, pair.BaseValue)}");
        }

        /// <summary>
        /// 若存在连接，则创造连接对应文本
        /// </summary>
        public static void AddLinkTexts(List<IUIItem> items, ITile tile) {
            bool created = false;
            foreach (var direction in directions) {
                if (tile.Refs.Has(direction)) {
                    IRef r = tile.Refs.Get(direction);
                    if (r.Value > 0) {
                        items.Add(UIItem.CreateText($"{Localization.Ins.Get(direction)}输入{Localization.Ins.ValPlus(r.Type, r.Value)}"));

                    } else {
                        items.Add(UIItem.CreateText($"{Localization.Ins.Get(direction)}输出{Localization.Ins.ValPlus(r.Type, -r.Value)}"));
                    }
                    created = true;
                }
            }
            if (!created) {
                items.Add(UIItem.CreateText("输入输出【无】"));
            }
        }
        /// <summary>
        /// 是否存在任何连接
        /// </summary>
        public static bool HasAnyLink(ITile tile) => tile.Refs == null ? false : (tile.Refs.Has<IUp>() || tile.Refs.Has<IDown>() || tile.Refs.Has<ILeft>() || tile.Refs.Has<IRight>());


        // --------------------------------------------------
        private readonly static HashSet<IRef> buttonsBuffer = new HashSet<IRef>();
        public static void AddButtons(List<IUIItem> items, ITile tile) {
            ILinkConsumer consumer = tile as ILinkConsumer;
            ILinkProvider provider = tile as ILinkProvider;
            if (consumer == null && provider == null) throw new Exception();
            if (buttonsBuffer.Count != 0) throw new Exception();
            if (consumer != null) {
                consumer.Consume(consumerRefsBuffer);
                foreach (var consumerRef in consumerRefsBuffer) {
                    buttonsBuffer.Add(consumerRef);
                }
                consumerRefsBuffer.Clear();
            }
            if (provider != null) {
                provider.Provide(providerRefsBuffer);
                foreach (var providerRef in providerRefsBuffer) {
                    if (consumer == null || !buttonsBuffer.Contains(providerRef)) {
                        buttonsBuffer.Add(providerRef);
                    }
                }
                providerRefsBuffer.Clear();
            }
            foreach (var button in buttonsBuffer) {
                if (button.Type != null && button.Value != 0) items.Add(CreateRefText(button));
            }
            buttonsBuffer.Clear();
            // 运输能力
            if (consumer is ILinkQuantityRestriction linkSpeedLimit) {
                long limit = linkSpeedLimit.LinkQuantityRestriction;
                if (limit < long.MaxValue) items.Add(UIItem.CreateText($"运输能力【{linkSpeedLimit.LinkQuantityRestriction}】"));
            }
            // 输入输出信息
            AddLinkTexts(items, tile);

            if (consumer != null) {
                AddConsumerButtons(items, tile);
                AddConsumerButtons_Undo(items, tile);
            }
            if (provider != null) {
                AddProviderButtons(items, tile);
                AddProviderButtons_Undo(items, tile);
            }

        }

        private readonly static List<IRef> consumerRefsBuffer = new List<IRef>();
        private readonly static List<IRef> providerRefsBuffer = new List<IRef>();

        public static void AutoConsume(ITile tile) {
            AddConsumerButtons(null, tile, true);
        }
        public static void AddConsumerButtons(List<IUIItem> items, ITile tile, bool dontCreateButtons = false) {
            // start
            ILinkConsumer consumer = tile as ILinkConsumer; // assert ILinkConsumer才能作为CreateConsumerButtons参数
            if (consumer == null) return;
            if (consumerRefsBuffer.Count != 0) throw new Exception(); // assert 缓存已经清空
            consumer.Consume(consumerRefsBuffer); // 获取需求
            if (consumerRefsBuffer.Count == 0) return; // 没有需求
            if (consumerRefsBuffer.Count > 4) throw new Exception(); // 不能太多
            IMap map = tile.GetMap();
            Vector2Int pos = tile.GetPos();
            // end

            ITile upTile = map.Get(pos + Vector2Int.up);
            ITile downTile = map.Get(pos + Vector2Int.down);
            ITile leftTile = map.Get(pos + Vector2Int.left);
            ITile rightTile = map.Get(pos + Vector2Int.right);

            // 类型限制
            Type linkTileTypeRestriction = null;
            if (tile is ILinkTileTypeRestriction iLinkTileTypeRestriction) {
                linkTileTypeRestriction = iLinkTileTypeRestriction.LinkTileTypeRestriction;
            }

            TryAddConsumerButton(items, tile, upTile, typeof(IUp), typeof(IDown), dontCreateButtons, linkTileTypeRestriction);
            TryAddConsumerButton(items, tile, downTile, typeof(IDown), typeof(IUp), dontCreateButtons, linkTileTypeRestriction);
            TryAddConsumerButton(items, tile, leftTile, typeof(ILeft), typeof(IRight), dontCreateButtons, linkTileTypeRestriction);
            TryAddConsumerButton(items, tile, rightTile, typeof(IRight), typeof(ILeft), dontCreateButtons, linkTileTypeRestriction);

            //// priority for non road objects
            //bool upTileIsRoad = upTile is Road;
            //bool downTileIsRoad = downTile is Road;
            //bool leftTileIsRoad = leftTile is Road;
            //bool rightTileIsRoad = rightTile is Road;

            //if (!upTileIsRoad) TryAddConsumerButton(items, tile, upTile, typeof(IUp), typeof(IDown), dontCreateButtons);
            //if (!downTileIsRoad) TryAddConsumerButton(items, tile, downTile, typeof(IDown), typeof(IUp), dontCreateButtons);
            //if (!leftTileIsRoad) TryAddConsumerButton(items, tile, leftTile, typeof(ILeft), typeof(IRight), dontCreateButtons);
            //if (!rightTileIsRoad) TryAddConsumerButton(items, tile, rightTile, typeof(IRight), typeof(ILeft), dontCreateButtons);

            //if (upTileIsRoad) TryAddConsumerButton(items, tile, upTile, typeof(IUp), typeof(IDown), dontCreateButtons);
            //if (downTileIsRoad) TryAddConsumerButton(items, tile, downTile, typeof(IDown), typeof(IUp), dontCreateButtons);
            //if (leftTileIsRoad) TryAddConsumerButton(items, tile, leftTile, typeof(ILeft), typeof(IRight), dontCreateButtons);
            //if (rightTileIsRoad) TryAddConsumerButton(items, tile, rightTile, typeof(IRight), typeof(ILeft), dontCreateButtons);

            consumerRefsBuffer.Clear();
        }
        public static void AutoConsume_Undo(ITile tile) {
            AddConsumerButtons_Undo(null, tile, true);
        }
        public static void AddConsumerButtons_Undo(List<IUIItem> items, ITile tile, bool dontCreateButtons = false) {
            // start
            if (consumerRefsBuffer.Count != 0) throw new Exception(); // assert 缓存已经清空
            ILinkConsumer consumer = tile as ILinkConsumer; // assert ILinkConsumer才能作为CreateConsumerButtons_Undo参数
            if (consumer == null) return;
            consumer.Consume(consumerRefsBuffer); // 读取
            if (consumerRefsBuffer.Count == 0) return;
            if (consumerRefsBuffer.Count > 4) throw new Exception(); // 不能太多
            IMap map = tile.GetMap();
            Vector2Int pos = tile.GetPos();
            // end

            TryAddConsumerButton_Undo(items, tile, map.Get(pos + Vector2Int.up), typeof(IUp), typeof(IDown), dontCreateButtons);
            TryAddConsumerButton_Undo(items, tile, map.Get(pos + Vector2Int.down), typeof(IDown), typeof(IUp), dontCreateButtons);
            TryAddConsumerButton_Undo(items, tile, map.Get(pos + Vector2Int.left), typeof(ILeft), typeof(IRight), dontCreateButtons);
            TryAddConsumerButton_Undo(items, tile, map.Get(pos + Vector2Int.right), typeof(IRight), typeof(ILeft), dontCreateButtons);

            consumerRefsBuffer.Clear();
        }

        public static void AutoProvide(ITile tile) {
            AddProviderButtons(null, tile, true);
        }
        public static void AddProviderButtons(List<IUIItem> items, ITile tile, bool dontCreateButtons = false) {
            // start
            ILinkProvider provider = tile as ILinkProvider;
            if (provider == null) return;
            if (providerRefsBuffer.Count != 0) throw new Exception();
            provider.Provide(providerRefsBuffer); // 获取供给
            if (providerRefsBuffer.Count == 0) return; // 没有供给
            if (providerRefsBuffer.Count > 4) throw new Exception();
            IMap map = tile.GetMap();
            Vector2Int pos = tile.GetPos();
            // end

            ITile upTile = map.Get(pos + Vector2Int.up);
            ITile downTile = map.Get(pos + Vector2Int.down);
            ITile leftTile = map.Get(pos + Vector2Int.left);
            ITile rightTile = map.Get(pos + Vector2Int.right);

            // 类型限制
            Type linkTileTypeRestriction = null;
            if (tile is ILinkTileTypeRestriction iLinkTileTypeRestriction) {
                linkTileTypeRestriction = iLinkTileTypeRestriction.LinkTileTypeRestriction;
            }

            TryAddProviderButton(items, tile, upTile, typeof(IUp), typeof(IDown), dontCreateButtons, linkTileTypeRestriction);
            TryAddProviderButton(items, tile, downTile, typeof(IDown), typeof(IUp), dontCreateButtons, linkTileTypeRestriction);
            TryAddProviderButton(items, tile, leftTile, typeof(ILeft), typeof(IRight), dontCreateButtons, linkTileTypeRestriction);
            TryAddProviderButton(items, tile, rightTile, typeof(IRight), typeof(ILeft), dontCreateButtons, linkTileTypeRestriction);

            //// priority for non road objects
            //bool upTileIsRoad = upTile is Road;
            //bool downTileIsRoad = downTile is Road;
            //bool leftTileIsRoad = leftTile is Road;
            //bool rightTileIsRoad = rightTile is Road;

            //if (!upTileIsRoad) TryAddProviderButton(items, tile, upTile, typeof(IUp), typeof(IDown), dontCreateButtons);
            //if (!downTileIsRoad) TryAddProviderButton(items, tile, downTile, typeof(IDown), typeof(IUp), dontCreateButtons);
            //if (!leftTileIsRoad) TryAddProviderButton(items, tile, leftTile, typeof(ILeft), typeof(IRight), dontCreateButtons);
            //if (!rightTileIsRoad) TryAddProviderButton(items, tile, rightTile, typeof(IRight), typeof(ILeft), dontCreateButtons);

            //if (upTileIsRoad) TryAddProviderButton(items, tile, upTile, typeof(IUp), typeof(IDown), dontCreateButtons);
            //if (downTileIsRoad) TryAddProviderButton(items, tile, downTile, typeof(IDown), typeof(IUp), dontCreateButtons);
            //if (leftTileIsRoad) TryAddProviderButton(items, tile, leftTile, typeof(ILeft), typeof(IRight), dontCreateButtons);
            //if (rightTileIsRoad) TryAddProviderButton(items, tile, rightTile, typeof(IRight), typeof(ILeft), dontCreateButtons);

            providerRefsBuffer.Clear();
        }

        public static void AutoProvide_Undo(ITile tile) {
            AddProviderButtons_Undo(null, tile, true);
        }
        public static void AddProviderButtons_Undo(List<IUIItem> items, ITile tile, bool dontCreateButtons = false) {
            // start
            ILinkProvider provider = tile as ILinkProvider;
            if (provider == null) return;
            if (providerRefsBuffer.Count != 0) throw new Exception();
            provider.Provide(providerRefsBuffer); // 获取供给
            if (providerRefsBuffer.Count == 0) return; // 没有供给
            if (providerRefsBuffer.Count > 4) throw new Exception();
            IMap map = tile.GetMap();
            Vector2Int pos = tile.GetPos();
            // end

            TryAddProviderButton_Undo(items, tile, map.Get(pos + Vector2Int.up), typeof(IUp), typeof(IDown), dontCreateButtons);
            TryAddProviderButton_Undo(items, tile, map.Get(pos + Vector2Int.down), typeof(IDown), typeof(IUp), dontCreateButtons);
            TryAddProviderButton_Undo(items, tile, map.Get(pos + Vector2Int.left), typeof(ILeft), typeof(IRight), dontCreateButtons);
            TryAddProviderButton_Undo(items, tile, map.Get(pos + Vector2Int.right), typeof(IRight), typeof(ILeft), dontCreateButtons);

            providerRefsBuffer.Clear();
        }

        private static void TryAddProviderButton_Undo(List<IUIItem> items, ITile providerTile, ITile consumerTile, Type providerDir, Type consumerDir, bool dontCreateButtons) {
            // start
            ILinkProvider provider = providerTile as ILinkProvider; // 肯定非null
            bool hasLink = provider.Refs.Has(providerDir); // 是否已经存在连接
            if (!hasLink) return;
            IRef providerLink = provider.Refs.Get(providerDir);
            if (providerLink.Value > 0) return; // 这里不是provider

            ILinkConsumer consumer = consumerTile as ILinkConsumer;
            if (consumer == null) return;
            if (consumerRefsBuffer.Count != 0) throw new Exception($"AddButton不可以在OnLink里递归");
            consumer.Consume(consumerRefsBuffer);
            IRef consumerLink = consumer.Refs.Get(consumerDir);
            // end

            foreach (var consumerRef in consumerRefsBuffer) {
                if (consumerRef.Type == consumerLink.Type) { // 找到consumerRef
                    // if (consumerRef.Value >= consumerLink.Value) {  // consumerRef有足够资源
                    foreach (var providerRef in providerRefsBuffer) {
                        if (providerRef.Type == providerLink.Type) {

                            //// before unlinking, try stop
                            //if (dontCreateButtons && consumerTile is IRunable runable) {
                            //    if (runable.CanStop()) runable.Stop();
                            //}

                            long quantity = Math.Min(consumerLink.Value, Math.Min(consumerRef.Value, providerRef.BaseValue - providerRef.Value));
                            if (quantity == 0) continue;
                            if (quantity < 0) throw new Exception();
                            // if (providerLink.Value != -quantity) throw new Exception($"{providerLink.Value} {quantity}");

                            void action() {

                                // 可以取消连接
                                providerLink.Type = providerRef.Type;
                                consumerLink.Type = consumerRef.Type;
                                providerLink.Value += quantity; // providerLink以负值表示输出
                                consumerLink.Value -= quantity; // consumerLink以正值表示输入
                                providerRef.Value += quantity;
                                consumerRef.Value -= quantity;

                                if (consumerLink.Value == 0) {
                                    if (providerLink.Value != 0) {
                                        throw new Exception();
                                    }
                                    provider.Refs.Remove(providerDir);
                                    consumer.Refs.Remove(consumerDir);
                                }

                                NeedUpdateNeighbors(providerTile);
                                NeedUpdateNeighbors(consumerTile);

                                (providerTile as ILinkEvent)?.OnLink(providerDir, quantity);
                                (consumerTile as ILinkEvent)?.OnLink(consumerDir, -quantity);

                                if (!dontCreateButtons) {
                                    providerTile.OnTap();
                                }
                            }
                            if (dontCreateButtons) {
                                action();
                            } else {
                                items.Add(UIItem.CreateButton($"<color=#ffaaaaff>取消</color>{Localization.Ins.Get(providerDir)}输出{Localization.Ins.ValPlus(consumerRef.Type, quantity)}", action));
                            }
                        }
                    }
                    // }
                    break; // 不用再找了
                }
            }

            consumerRefsBuffer.Clear();
        }

        private static void TryAddProviderButton(List<IUIItem> items, ITile providerTile, ITile consumerTile, Type providerDir, Type consumerDir, bool dontCreateButtons, Type linkTileTypeRestriction) {

            // 类型限制
            if (linkTileTypeRestriction != null && !linkTileTypeRestriction.IsAssignableFrom(consumerTile.GetType())) {
                return;
            }

            ILinkProvider provider = providerTile as ILinkProvider; // 肯定非null
            bool hasLink = provider.Refs.Has(providerDir); // 是否已经存在连接
            IRef providerLink = hasLink ? provider.Refs.Get(providerDir) : null;  // 若存在连接则获取连接
            if (hasLink && providerLink.Value > 0) return; // 这里不是provider

            ILinkConsumer consumer = consumerTile as ILinkConsumer;
            if (consumer == null) return;
            if (consumerRefsBuffer.Count != 0) throw new Exception($"AddButton不可以在OnLink里递归");
            consumer.Consume(consumerRefsBuffer);
            IRef consumerLink = hasLink ? consumer.Refs.Get(consumerDir) : null; // 若存在连接则获取连接
            if (hasLink != provider.Refs.Has(providerDir)) throw new Exception(); // assert !连接不成一对

            foreach (var providerRef in providerRefsBuffer) {
                if (hasLink && providerRef.Type != providerLink.Type) {
                    // 已经在此方向建立不兼容的连接
                    continue;
                }

                foreach (var consumerRef in consumerRefsBuffer) {
                    if (hasLink && consumerRef.Type != consumerLink.Type) {
                        // 已经在此方向建立不兼容的连接
                        continue;
                    }

                    long quantity = Math.Min(providerRef.Value, consumerRef.BaseValue - consumerRef.Value);

                    if (consumer is ILinkQuantityRestriction linkQuantityLimit) {
                        quantity = Math.Min(linkQuantityLimit.LinkQuantityRestriction, quantity);
                    }
                    if (provider is ILinkQuantityRestriction linkQuantityLimit2) {
                        quantity = Math.Min(linkQuantityLimit2.LinkQuantityRestriction, quantity);
                    }

                    if (quantity == 0) continue;
                    if (quantity < 0) throw new Exception();

                    if (consumerRef.Type == null || Tag.HasTag(providerRef.Type, consumerRef.Type)) {

                        if (consumerRef.Type == null && consumerTile is ILinkTypeRestriction restriction) { // 约束consumerRef.Type == null时的类型，一般用于不改变类型的 AbstractRoad
                            if (!Tag.HasTag(providerRef.Type, restriction.LinkTypeRestriction)) {
                                break;
                            }
                        }

                        if (hasLink && consumerLink.Value + quantity < 0) break; // 溢出了
                        //if (consumer is ILinkQuantityRestriction linkQuantityLimit) {
                        //    if (hasLink && consumerLink.Value + quantity > linkQuantityLimit.LinkQuantityRestriction) break;
                        //    if (!hasLink && quantity > linkQuantityLimit.LinkQuantityRestriction) break;
                        //}
                        void action() {

                            // 可以建立连接
                            if (!hasLink) {
                                consumerLink = consumer.Refs.Create(consumerDir);
                                providerLink = provider.Refs.Create(providerDir);
                            }
                            // 至此，consumerLink和providerLink肯定非空
                            providerLink.Type = providerRef.Type;
                            consumerLink.Type = consumerRef.Type ?? providerRef.Type;
                            if (consumerRef.Type == null) {
                                consumerRef.Type = consumerLink.Type;
                            }
                            providerLink.Value -= quantity; // providerLink以负值表示输出
                            consumerLink.Value += quantity; // consumerLink以正值表示输入
                            providerRef.Value -= quantity;
                            consumerRef.Value += quantity;

                            //providerTile.NeedUpdateSpriteKeys = true;
                            //consumerTile.NeedUpdateSpriteKeys = true;
                            NeedUpdateNeighbors(providerTile);
                            NeedUpdateNeighbors(consumerTile);

                            (providerTile as ILinkEvent)?.OnLink(providerDir, -quantity);
                            (consumerTile as ILinkEvent)?.OnLink(consumerDir, quantity);

                            //// after linking, try run
                            //if (dontCreateButtons && consumerTile is IRunable runable) {
                            //    if (runable.CanRun()) runable.Run();
                            //}

                            if (!dontCreateButtons) {
                                providerTile.OnTap();
                            }
                        }
                        if (dontCreateButtons) {
                            action();
                        } else {
                            items.Add(UIItem.CreateButton($"<color=#aaaaffff>建立</color>{Localization.Ins.Get(providerDir)}输出{Localization.Ins.ValPlus(providerRef.Type, -quantity)}", action));
                        }
                    }
                }
            }

            consumerRefsBuffer.Clear();
        }

        private static void TryAddConsumerButton_Undo(List<IUIItem> items, ITile consumerTile, ITile providerTile, Type consumerDir, Type providerDir, bool dontCreateButtons) {
            // start
            ILinkConsumer consumer = consumerTile as ILinkConsumer;
            bool hasLink = consumer.Refs.Has(consumerDir); // 是否已经存在连接
            if (!hasLink) return; // 没有连接，则不存在解除连接的问题
            IRef consumerLink = consumer.Refs.Get(consumerDir); // 若存在连接则获取连接
            if (consumerLink.Value < 0) return; // 这里不是consumer

            ILinkProvider provider = providerTile as ILinkProvider;
            if (provider == null) return;
            if (providerRefsBuffer.Count != 0) throw new Exception($"AddButton不可以在OnLink里递归");
            provider.Provide(providerRefsBuffer);
            IRef providerLink = provider.Refs.Get(providerDir);  // 若存在连接则获取连接
            // end

            foreach (var consumerRef in consumerRefsBuffer) {
                if (consumerRef.Type == consumerLink.Type) { // 找到consumerRef
                    // if (consumerRef.Value >= consumerLink.Value) {  // consumerRef有足够资源
                    foreach (var providerRef in providerRefsBuffer) {
                        if (providerRef.Type == providerLink.Type) {
                            long quantity = Math.Min(consumerLink.Value, Math.Min(consumerRef.Value, providerRef.BaseValue - providerRef.Value));
                            if (quantity == 0) continue;
                            if (quantity < 0) throw new Exception();
                            // if (providerLink.Value != -quantity) throw new Exception($"{providerLink.Value} {quantity}");
                            void action() {
                                // 可以取消连接
                                providerLink.Type = providerRef.Type;
                                consumerLink.Type = consumerRef.Type;
                                providerLink.Value += quantity; // providerLink以负值表示输出
                                consumerLink.Value -= quantity; // consumerLink以正值表示输入
                                providerRef.Value += quantity;
                                consumerRef.Value -= quantity;

                                if (consumerLink.Value == 0) {
                                    if (providerLink.Value != 0) {
                                        throw new Exception();
                                    }
                                    provider.Refs.Remove(providerDir);
                                    consumer.Refs.Remove(consumerDir);
                                }

                                NeedUpdateNeighbors(providerTile);
                                NeedUpdateNeighbors(consumerTile);

                                //// after unlinking, try stop
                                //if (dontCreateButtons && providerTile is IRunable runable) {
                                //    if (runable.CanStop()) runable.Stop();
                                //}

                                (providerTile as ILinkEvent)?.OnLink(providerDir, quantity);
                                (consumerTile as ILinkEvent)?.OnLink(consumerDir, -quantity);

                                if (!dontCreateButtons) {
                                    consumerTile.OnTap();
                                }
                            }
                            if (dontCreateButtons) {
                                action();
                            } else {
                                items.Add(UIItem.CreateButton($"<color=#ffaaaaff>取消</color>{Localization.Ins.Get(consumerDir)}输入{Localization.Ins.ValPlus(consumerRef.Type, -quantity)}", action));
                            }
                        }
                    }
                    // }
                    break; // 不用再找了
                }
            }

            providerRefsBuffer.Clear();
        }

        private static void TryAddConsumerButton(List<IUIItem> items, ITile consumerTile, ITile providerTile, Type consumerDir, Type providerDir, bool dontCreateButtons, Type linkTileTypeRestriction) {
            // 类型限制
            if (linkTileTypeRestriction != null && !linkTileTypeRestriction.IsAssignableFrom(providerTile.GetType())) {
                return;
            }

            // start
            ILinkConsumer consumer = consumerTile as ILinkConsumer; // 肯定非null
            bool hasLink = consumer.Refs.Has(consumerDir); // 是否已经存在连接
            IRef consumerLink = hasLink ? consumer.Refs.Get(consumerDir) : null; // 若存在连接则获取连接
            if (hasLink && consumerLink.Value < 0) return;  // 这里不是consumer, 是provider, 不会叠加

            ILinkProvider provider = providerTile as ILinkProvider;
            if (provider == null) return;

            if (providerRefsBuffer.Count != 0) throw new Exception($"AddButton不可以在OnLink里递归");
            provider.Provide(providerRefsBuffer);
            IRef providerLink = hasLink ? provider.Refs.Get(providerDir) : null;  // 若存在连接则获取连接
            if (hasLink != provider.Refs.Has(providerDir)) throw new Exception(); // assert !连接不成一对
                                                                                  // end


            foreach (var consumerRef in consumerRefsBuffer) {
                if (hasLink && consumerRef.Type != consumerLink.Type) {
                    // 已经在此方向建立不兼容的连接
                    continue;
                }
                foreach (var providerRef in providerRefsBuffer) {
                    if (hasLink && providerRef.Type != providerLink.Type) {
                        // 已经在此方向建立不兼容的连接
                        continue;
                    }

                    //// before linking, try run
                    //if (dontCreateButtons && providerTile is IRunable runable) {
                    //    if (runable.CanRun()) runable.Run();
                    //}

                    // providerItem.Value 是供应方能提供的最大值
                    // consumerItem.BaseValue - consumerItem.Value 是需求方能消耗的最大值
                    long quantity = Math.Min(providerRef.Value, consumerRef.BaseValue - consumerRef.Value);

                    if (consumer is ILinkQuantityRestriction linkQuantityLimit) {
                        quantity = Math.Min(linkQuantityLimit.LinkQuantityRestriction, quantity);
                    }
                    if (provider is ILinkQuantityRestriction linkQuantityLimit2) {
                        quantity = Math.Min(linkQuantityLimit2.LinkQuantityRestriction, quantity);
                    }

                    // 如果供需其中一个为0，则无法建立此对资源的连接
                    if (quantity == 0) continue;
                    if (quantity < 0) throw new Exception();
                    // 供给方类型为需求方类型子类，才能成功供给。需求方类型为null视为需求任意资源
                    if (consumerRef.Type == null || Tag.HasTag(providerRef.Type, consumerRef.Type)) {

                        if (consumerRef.Type == null && consumerTile is ILinkTypeRestriction restriction) { // 约束consumerRef.Type == null时的类型，一般用于不改变类型的 AbstractRoad
                            if (!Tag.HasTag(providerRef.Type, restriction.LinkTypeRestriction)) {
                                break;
                            }
                        }

                        if (hasLink && consumerLink.Value + quantity < 0) break; // 溢出了
                        //if (consumer is ILinkQuantityRestriction linkQuantityLimit) {
                        //    if (hasLink && consumerLink.Value + quantity > linkQuantityLimit.LinkQuantityRestriction) break;
                        //    if (!hasLink && quantity > linkQuantityLimit.LinkQuantityRestriction) break;
                        //}
                        void action() {

                            // 可以建立连接
                            if (!hasLink) {
                                consumerLink = consumer.Refs.Create(consumerDir);
                                providerLink = provider.Refs.Create(providerDir);
                            }
                            // 至此，consumerLink和providerLink肯定非空
                            providerLink.Type = providerRef.Type;
                            consumerLink.Type = consumerRef.Type ?? providerRef.Type;
                            if (consumerRef.Type == null) {
                                consumerRef.Type = consumerLink.Type;
                            }
                            providerLink.Value -= quantity; // providerLink以负值表示输出
                            consumerLink.Value += quantity; // consumerLink以正值表示输入
                            providerRef.Value -= quantity;
                            consumerRef.Value += quantity;

                            NeedUpdateNeighbors(providerTile);
                            NeedUpdateNeighbors(consumerTile);

                            (providerTile as ILinkEvent)?.OnLink(providerDir, -quantity);
                            (consumerTile as ILinkEvent)?.OnLink(consumerDir, quantity);

                            //// after linking, try run
                            //if (dontCreateButtons && consumerTile is IRunable runable) {
                            //    if (runable.CanRun()) runable.Run();
                            //}

                            if (!dontCreateButtons) {
                                consumerTile.OnTap();
                            }
                        }
                        if (dontCreateButtons) {
                            action();
                        } else {
                            items.Add(UIItem.CreateButton($"<color=#aaaaffff>建立</color>{Localization.Ins.Get(consumerDir)}输入{Localization.Ins.ValPlus(consumerRef.Type ?? providerRef.Type, quantity)}", action));
                        }
                    }
                }
            }
            providerRefsBuffer.Clear();
        }

        public static void NeedUpdateNeighbors(ITile tile) {
            IMap map = tile.GetMap();
            Vector2Int pos = tile.GetPos();
            tile.NeedUpdateSpriteKeys = true;
            map.Get(pos + Vector2Int.up).NeedUpdateSpriteKeys = true;
            map.Get(pos + Vector2Int.down).NeedUpdateSpriteKeys = true;
            map.Get(pos + Vector2Int.left).NeedUpdateSpriteKeys = true;
            map.Get(pos + Vector2Int.right).NeedUpdateSpriteKeys = true;
        }

        public static void NeedUpdateNeighbors8(ITile tile) {
            IMap map = tile.GetMap();
            Vector2Int pos = tile.GetPos();
            tile.NeedUpdateSpriteKeys = true;
            map.Get(pos + Vector2Int.up).NeedUpdateSpriteKeys = true;
            map.Get(pos + Vector2Int.down).NeedUpdateSpriteKeys = true;
            map.Get(pos + Vector2Int.left).NeedUpdateSpriteKeys = true;
            map.Get(pos + Vector2Int.right).NeedUpdateSpriteKeys = true;
            map.Get(pos + Vector2Int.up + Vector2Int.left).NeedUpdateSpriteKeys = true;
            map.Get(pos + Vector2Int.up + Vector2Int.right).NeedUpdateSpriteKeys = true;
            map.Get(pos + Vector2Int.down + Vector2Int.left).NeedUpdateSpriteKeys = true;
            map.Get(pos + Vector2Int.down + Vector2Int.right).NeedUpdateSpriteKeys = true;
        }
    }
}

