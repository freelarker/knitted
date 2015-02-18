using UnityEngine;

public class test : MonoBehaviour {
	public SkinnedMeshRenderer target;
	public SkinnedMeshRenderer source;

	public void OnGUI() {
		if (GUI.Button(new Rect(5, 5, 100, 50), "Switch")) {
			target.sharedMesh = source.sharedMesh;
			target.material = source.material;

			target.sharedMesh.RecalculateBounds();
			target.sharedMesh.RecalculateNormals();

			//target.transform.parent.GetComponent<Animation>().Play("Take 001");
		}
	}
}
