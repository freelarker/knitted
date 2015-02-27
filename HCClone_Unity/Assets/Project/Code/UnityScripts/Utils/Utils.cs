using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Utils {
	public class UI {
		public static Canvas GetCanvas(RenderMode renderMode) {
			Canvas canvas = null;

			Canvas[] sceneCanvases = GameObject.FindObjectsOfType<Canvas>();
			for (int i = 0; i < sceneCanvases.Length; i++) {
				if (sceneCanvases[i].renderMode == renderMode) {
					canvas = sceneCanvases[i];
					break;
				}
			}

			if (canvas == null) {
				GameObject canvasGO = new GameObject("Canvas");
				canvasGO.layer = LayerMask.NameToLayer("UI");
				canvasGO.AddComponent<GraphicRaycaster>();
				canvas = canvasGO.GetComponent<Canvas>();
				canvas.renderMode = renderMode;

				if (GameObject.FindObjectOfType<EventSystem>() == null) {
					GameObject eventSystemGO = new GameObject("EventSystem");
					eventSystemGO.AddComponent<EventSystem>();
#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IPHONE)
					goES.AddComponent<TouchInputModule>();
#else
					eventSystemGO.AddComponent<StandaloneInputModule>();
#endif
				}
			}

			return canvas;
		}

		public static Canvas GetWindowsCanvas() {
			Canvas canvas = null;

			GameObject canvasGO = GameObject.Find("CanvasWindows");
			if (canvasGO != null) {
				canvas = canvasGO.GetComponent<Canvas>();
			}

			if (canvas == null) {
				if(canvasGO == null) {
					canvasGO = new GameObject("CanvasWindows");
				}
				canvasGO.layer = LayerMask.NameToLayer("UI");
				canvasGO.AddComponent<GraphicRaycaster>();
				canvas = canvasGO.GetComponent<Canvas>();
				canvas.renderMode = RenderMode.ScreenSpaceOverlay;

				if (GameObject.FindObjectOfType<EventSystem>() == null) {
					GameObject eventSystemGO = new GameObject("EventSystem");
					eventSystemGO.AddComponent<EventSystem>();
#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IPHONE)
					goES.AddComponent<TouchInputModule>();
#else
					eventSystemGO.AddComponent<StandaloneInputModule>();
#endif
				}
			}

			if (canvas.gameObject.GetComponent<CanvasResolutionAdapter>() == null) {
				canvas.gameObject.AddComponent<CanvasResolutionAdapter>();
			}

			return canvas;
		}

		public static void AdaptCanvasResolution(int defaultWidth, int defaultHeight, Canvas canvas) {
			float widthRatio = 1f * Screen.width / GameConstants.DEFAULT_RESOLUTION_WIDTH;
			float heightRatio = 1f * Screen.height / GameConstants.DEFAULT_RESOLUTION_HEIGHT;

			canvas.scaleFactor = Mathf.Min(widthRatio, heightRatio);
		}
	}

	private static DateTime _unixEpochStart = new DateTime(1970, 1, 1);

	public static int UnixTimestamp {
		get { return (int)(DateTime.UtcNow - _unixEpochStart).TotalSeconds; }
	}
}
