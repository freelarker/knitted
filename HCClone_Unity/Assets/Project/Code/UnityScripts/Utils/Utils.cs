using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Utils {
	public class UI {
		public static Canvas GetCanvas() {
			Canvas canvas = GameObject.FindObjectOfType<Canvas>();
			if (canvas == null) {
				GameObject canvasGO = new GameObject("Canvas");
				canvasGO.layer = LayerMask.NameToLayer("UI");
				canvasGO.AddComponent<GraphicRaycaster>();
				canvas = canvasGO.GetComponent<Canvas>();
				canvas.renderMode = RenderMode.ScreenSpaceOverlay;

				GameObject eventSystemGO = new GameObject("EventSystem");
				eventSystemGO.AddComponent<EventSystem>();
#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IPHONE)
				goES.AddComponent<TouchInputModule>();
#else
				eventSystemGO.AddComponent<StandaloneInputModule>();
#endif
			}

			return canvas;
		}
	}

	private static DateTime _unixEpochStart = new DateTime(1970, 1, 1);

	public static int UnixTimestamp {
		get { return (int)(DateTime.UtcNow - _unixEpochStart).TotalSeconds; }
	}
}
