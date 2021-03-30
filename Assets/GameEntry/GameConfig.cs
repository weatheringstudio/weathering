

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
		public static bool CheatMode = false;
		public static long DefaultInventorySize { get; } = 1000000000000000;
		public static System.Type InitialMap { get; private set; } = typeof(Map_0_0);
		public const int VersionCode = 20210330;
		public static void OnConstruct(IGlobals globals) {

			// 全局理智
			IValue sanity = globals.Values.Create<Sanity>();
			sanity.Max = 100;
			sanity.Val = sanity.Max / 2;
			sanity.Inc = 1;
			sanity.Del = Value.Second;

			// 行动冷却
			IValue cooldown = globals.Values.Create<CoolDown>();
			cooldown.Inc = 1;
			cooldown.Max = 1;
			cooldown.Del = Value.Second;

			IInventory inventory = globals.Inventory;
			inventory.QuantityCapacity = DefaultInventorySize;
			inventory.TypeCapacity = 10;

			inventory.Add<TutorialMapTheBook>(1);
			inventory.Add<TutorialMapTheDiary>(1);
			inventory.Add<TutorialMapTheCurse>(1);


			Globals.Ins.Values.GetOrCreate<QuestResource>().Del = Value.Second;
			SpecialPages.OpenStartingPage();
		}

		public static void OnGameConstruct() {

		}

		public static void OnGameEnable() {

		}

		public static void OnSave() {

		}



	}
}

