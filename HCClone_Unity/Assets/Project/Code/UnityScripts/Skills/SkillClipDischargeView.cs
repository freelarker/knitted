using UnityEngine;
using System.Collections;

public class SkillClipDischargeView : MonoBehaviour {
	[SerializeField]
	private TracerParticleController _tracersPrefab;
	[SerializeField]
	private Vector3 _tracerLocalPosition;

	private TracerParticleController _tracersInstance;

	public void Play(Transform parent, float distanceToTarget) {
		_tracersInstance = (GameObject.Instantiate(_tracersPrefab.gameObject) as GameObject).GetComponent<TracerParticleController>();
		_tracersInstance.transform.parent = parent;
		_tracersInstance.transform.localRotation = Quaternion.identity;
		_tracersInstance.transform.localScale = Vector3.one;
		_tracersInstance.transform.localPosition = _tracerLocalPosition;
		_tracersInstance.SetParticleDistance(distanceToTarget);
	}

	public void OnDestroy() {
		if (_tracersInstance != null) {
			GameObject.Destroy(_tracersInstance.gameObject);
		}
	}
}
