

using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
	/// <summary>
	/// 发布时和测试时，需要改哪几个地方？
	/// GlobalGameEvents的设置
	/// </summary>
	public static class GameConfig
	{
		public static System.Type InitialMap { get; private set; } = typeof(IslandMap);
		public const int VersionCode = 20210203;
		public static void OnGameConstruct(IGlobals globals) {
			// 全局理智
			IValue sanity = globals.Values.Create<Sanity>();
			sanity.Max = 100;
			sanity.Inc = 1;
			sanity.Del = Value.Second;
			// 全局农场科技
			IValue farmTech = globals.Values.Create<FarmTech>();
			farmTech.Del = 360 * Value.Second;


			// 初始音效音量
			IValue soundEffectVolume = globals.Values.Create<SoundEffectVolume>();
			soundEffectVolume.Max = 800;
			// 初始音乐音量
			IValue musicEffectVolume = globals.Values.Create<SoundMusicVolume>();
			musicEffectVolume.Max = 500;

			// 提示设置
            Globals.Ins.Bool<InventoryQueryInformationOfCostDisabled>(true);
			Globals.Ins.Bool<InventoryQueryInformationOfRevenueDisabled>(true);
		}

		public static void OnGameConstruct() {
			// SpecialPages.AskFont();
		}

		public static void OnGameEnable() {

		}

		public static void OnGameUpdate() {
			// render
			if (Input.GetKeyDown(KeyCode.Space)) {
				GameMenu.Entry.SaveGame();
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

