
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Weathering
{
	public class BarImage : MonoBehaviour
	{
		public Image RealImage;
		public Button Button;
		public Action OnButtonTapped;
		public void TapButton() {
			OnButtonTapped?.Invoke();
        }
	}
}

