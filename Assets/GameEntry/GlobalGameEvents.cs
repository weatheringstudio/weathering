

using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
	/// <summary>
	/// GameEntry类专用，其他类不需要调用GlobalGameEvents
	/// </summary>
	public static class GlobalGameEvents
	{
		public static void OnGameConstruct(IGlobals globals) {
			IValue sanity = globals.Values.Create<Sanity>();
			sanity.Max = 100;
			sanity.Inc = 1;
			sanity.Del = Value.Second;

			IValue soundEffectVolume = globals.Values.Create<SoundEffectVolume>();
			soundEffectVolume.Max = 800;

			IValue musicEffectVolume = globals.Values.Create<SoundMusicVolume>();
			musicEffectVolume.Max = 500;

            IValue farmTech = globals.Values.Create<FarmTech>();
            farmTech.Del = 360 * Value.Second;

            Globals.Ins.Bool<InventoryQueryInformationOfCostDisabled>(true);
			Globals.Ins.Bool<InventoryQueryInformationOfRevenueDisabled>(true);

		}

		/// <summary>
		/// 第一次进入游戏时弹出的菜单
		/// </summary>
		public static void OnEnterInitialMap() {
			if (true) {
				GameMenu.Ins.ChangeFont();
				GameMenu.Ins.SynchronizeFont();
			}
			else {
				// AskFont();
			}
        }

		private static void AskFont() {
			var items = new List<IUIItem>();

			items.Add(UIItem.CreateButton("改变字体", () => {
				GameMenu.Ins.ChangeFont();
				GameMenu.Ins.SynchronizeFont();
				OnEnterInitialMap();
			}));

			items.Add(UIItem.CreateButton("使用当前字体", () => {
				GameMenu.Ins.ChangeFont();
				UI.Ins.Active = false;
			}));

			UI.Ins.ShowItems("是否切换字体", items);
		}

		public static void OnGameEnable() {
			GameMenu.Ins.SynchronizeFont();
		}

		public static void OnGameUpdate() {
			// render
			if (Input.GetKeyDown(KeyCode.Space)) {
				GameEntry.Ins.SaveGame();
			}
			if (Input.GetKeyDown(KeyCode.Escape)) {
				if (UI.Ins.Active) {
					UI.Ins.Active = false;
				} else {
					GameMenu.Ins.OnTap();
				}
			}
			if (Input.GetKeyDown(KeyCode.Z)) {
			}
		}

		public static void OnSave() {

		}

	}
}

