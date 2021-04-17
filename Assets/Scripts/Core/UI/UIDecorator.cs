
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
    public static class UIDecorator
    {
        public static Action ConfirmBefore(Action onConfirm, Action onCancel = null, string content = null) {
            return () => {
                UI.Ins.ShowItems("是否确定", new List<IUIItem> {
                    new UIItem {
                        Type = IUIItemType.MultilineText,
                        Content = content ?? "确定要这么做吗? "
                    },
                    new UIItem {
                        Type = IUIItemType.Button,
                        Content = "确定",
                        OnTap = onConfirm
                    },
                    new UIItem {
                        Type = IUIItemType.Button,
                        Content = "取消",
                        OnTap = onCancel ?? (() => {
                            UI.Ins.Active = false;
                        })
                    }
                });
            };
        }
        public static Action InformAfter(Action callback, string content = null) {
            return () => {
                callback();
                UI.Ins.ShowItems("提示", new List<IUIItem> {
                    new UIItem {
                        Type = IUIItemType.MultilineText,
                        Content = content ?? "已经完成"
                    },
                    new UIItem {
                        Type = IUIItemType.Button,
                        Content = "确定",
                        OnTap = () => UI.Ins.Active = false
                    },
                });
            };
        }
    }
}

