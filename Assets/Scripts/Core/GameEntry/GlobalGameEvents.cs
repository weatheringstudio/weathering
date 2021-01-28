
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
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
		}

		public static void OnSave() {

		}

	}
}

