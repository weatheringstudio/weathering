﻿

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
		public static long DefaultInventoryOfResourceQuantityCapacity { get; } = 1000000000000000;
		public static int DefaultInventoryOfResourceTypeCapacity { get; } = 30;
		public static long DefaultInventoryOfSupplyQuantityCapacity { get; } = 10000000000;
		public static int DefaultInventoryOfSupplyTypeCapacity { get; } = 10;


		public const int VersionCode = 20210415;
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
			inventory.QuantityCapacity = DefaultInventoryOfResourceQuantityCapacity;
			inventory.TypeCapacity = 10;

			inventory.Add<TutorialMapTheBook>(1);
			inventory.Add<TutorialMapTheDiary>(1);
			inventory.Add<TutorialMapTheCurse>(1);


			Globals.Ins.Values.GetOrCreate<QuestResource>().Del = Value.Second;

			Globals.Ins.Bool<Totem>(true);

			Globals.Ins.Values.GetOrCreate<KnowledgeOfNature>().Max = 100;
			Globals.Ins.Values.GetOrCreate<KnowledgeOfNature>().Del = Value.Second;

			Globals.Ins.Values.GetOrCreate<KnowledgeOfHandcraft>().Max = 100;
			Globals.Ins.Values.GetOrCreate<KnowledgeOfHandcraft>().Del = Value.Second;

		}

		public static void OnGameConstruct() {

		}

		public static void OnGameEnable() {

		}

		public static void OnSave() {

		}


	}





	[Depend]
	public class Book { }

	[Depend(typeof(Book))]
	public class TutorialMapTheBook { }

	[Depend(typeof(Book))]
	public class TutorialMapTheDiary { }

	[Depend(typeof(Book))]
	public class TutorialMapTheCurse { }
}

