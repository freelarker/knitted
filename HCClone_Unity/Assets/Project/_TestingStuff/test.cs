using UnityEngine;

public class test : MonoBehaviour {
	public SkinnedMeshRenderer target;
	public SkinnedMeshRenderer source;

	public void OnGUI() {
		if (GUI.Button(new Rect(5, 5, 100, 50), "Switch")) {
			Debug.LogWarning("=== " + gameObject.GetComponent<CanvasResolutionAdapter>());
			Debug.LogWarning("=== " + gameObject.GetComponent<CanvasResolutionAdapter>().enabled);
		}
	}
}
