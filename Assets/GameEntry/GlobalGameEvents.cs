

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

		public static void OnGameConstruct() {
			// SpecialPages.AskFont();
		}

		public static void OnGameEnable() {
			GameMenu.Ins.ChangeFont();
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

