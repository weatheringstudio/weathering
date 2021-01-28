
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
	public static class GlobalGameEvents
	{
		public static void OnGameConstruct(IGlobals globals) {
			IValue sanity = globals.Values.GetOrCreate<Sanity>();
			sanity.Max = 100;
			sanity.Inc = 1;
			sanity.Del = Value.Second;

			globals.SetPreference(Sound.Key, Sound.Enabled);
        }

		public static void OnSave() {

		}

	}
}

