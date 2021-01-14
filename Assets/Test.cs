
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
	public class Test : MonoBehaviour
	{

        public void SeeRes() {
            if (UI.Ins.Active) {
                UI.Ins.Active = false;
            } else {
                // UI.Ins.SimpleValue("资源", "任何时候点屏幕上方，可以关闭UI界面\n资源的存量和增速都在下面的滚动条里显示了", MapView.Ins.Map.Values[typeof(Food)], "粮食");
                if (Items == null) InitializeItems();
                UI.Ins.UIItems("资源总览。点屏幕上方关闭", Items);
            }
        }

        private void Update() {
            if (Input.GetKeyDown(KeyCode.A)) {
            }
        }

        private List<IUIItem> Items = null;
        private void InitializeItems() {
            IMap map = MapView.Ins.Map;
            Items = new List<IUIItem>();

            Items.Add(new UIItem {
                Content = "<color=purple>【base】</color>",
                Type = IUIItemType.ValueProgress,
                Value = map.Values.Get<BaseCount>()
            });
            Items.Add(new UIItem {
                Content = "<color=purple>【base】</color>",
                Type = IUIItemType.TimeProgress,
                Value = map.Values.Get<BaseCount>()
            });

            Items.Add(new UIItem {
                Content = "滚动条上的数字代表什么？",
                Type = IUIItemType.MultilineText,
            });
            Items.Add(new UIItem {
                Content = "<color=orange>【资源储量】</color> - 当前储量 - 储量上限",
                Type = IUIItemType.Button,
            });
            Items.Add(new UIItem {
                Content = "<color=orange>【资源产量】</color> - 单次产量 - 生产时间",
                Type = IUIItemType.Button,
            });

            Items.Add(new UIItem {
                Content = "体力",
                Type = IUIItemType.MultilineText,
            });
            Items.Add(new UIItem {
                Content = "<color=purple>【体力】</color>",
                Type = IUIItemType.ValueProgress,
                Value = map.Values.Get<Labor>()
            }); 
            Items.Add(new UIItem {
                Content = "<color=purple>【体力】</color>",
                Type = IUIItemType.TimeProgress,
                Value = map.Values.Get<Labor>()
            });

            Items.Add(new UIItem {
                Content = "粮食",
                Type = IUIItemType.MultilineText,
            });
            Items.Add(new UIItem {
                Content = "<color=yellow>【粮食】</color>",
                Type = IUIItemType.ValueProgress,
                Value = map.Values.Get<Food>()
            });
            Items.Add(new UIItem {
                Content = "<color=yellow>【粮食】</color>",
                Type = IUIItemType.TimeProgress,
                Value = map.Values.Get<Food>()
            });


            Items.Add(new UIItem {
                Content = "ぁ あ ぃ い ぅ う ぇ え ぉ お か が き ぎ く ぐ け げ こ ご さ ざ し じ す ず せ ぜ そ ぞ た だ ち ぢ っ つ づ て で と ど な に ぬ ね の は ば ぱ ひ び ぴ ふ ぶ ぷ へ べ ぺ ほ ぼ ぽ ま み む め も ゃ や ゅ ゆ ょ よ ら り る れ ろ ゎ わ ゐ ゑ を ん ゔ",
                Type = IUIItemType.MultilineText,
            });

            Items.Add(new UIItem {
                Content = "ぁ あ ぃ い ぅ う ぇ え ぉ お か が き ぎ く ぐ け げ こ ご さ ざ し じ す ず せ ぜ そ ぞ た だ ち ぢ っ つ づ て で と ど な に ぬ ね の は ば ぱ ひ び ぴ ふ ぶ ぷ へ べ ぺ ほ ぼ ぽ ま み む め も ゃ や ゅ ゆ ょ よ ら り る れ ろ ゎ わ ゐ ゑ を ん ゔ",
                Type = IUIItemType.MultilineText,
            });

            Items.Add(new UIItem {
                Content = "ぁ あ ぃ い ぅ う ぇ え ぉ お か が き ぎ く ぐ け げ こ ご さ ざ し じ す ず せ ぜ そ ぞ た だ ち ぢ っ つ づ て で と ど な に ぬ ね の は ば ぱ ひ び ぴ ふ ぶ ぷ へ べ ぺ ほ ぼ ぽ ま み む め も ゃ や ゅ ゆ ょ よ ら り る れ ろ ゎ わ ゐ ゑ を ん ゔ",
                Type = IUIItemType.MultilineText,
            });
        }
    }
}

