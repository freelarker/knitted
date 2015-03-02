using UnityEngine;

public class test : MonoBehaviour {
	public void Start() {
		Debug.LogWarning("=== " + Screen.width + ", " + Screen.height);
	}

	//public void OnGUI() {
	//	if (GUI.Button(new Rect(5, 5, 100, 50), "Switch")) {
	//		Debug.LogWarning("=== " + gameObject.GetComponent<CanvasResolutionAdapter>());
	//		Debug.LogWarning("=== " + gameObject.GetComponent<CanvasResolutionAdapter>().enabled);
	//	}
	//}
}
