using UnityEngine;

public class TracerParticleController : MonoBehaviour {
	private float _speed = 15f;

	private float _timeEnd;
	private Vector3 _positionEnd;

	private Transform _cachedTransform;

	public void Awake() {
		_cachedTransform = transform;
	}

	public void Update() {
		_cachedTransform.position = Vector3.MoveTowards(_cachedTransform.position, _positionEnd, _speed * Time.deltaTime);

		if (Time.time >= _timeEnd) {
			Stop();
		}
	}

	public void Play(float distance, Vector3 startPosition) {
		_cachedTransform.position = startPosition;
		_positionEnd = _cachedTransform.position + _cachedTransform.forward * distance;
		_timeEnd = Time.time + distance / _speed;

		gameObject.SetActive(true);
		
	}

	public void Stop() {
		gameObject.SetActive(false);
	}
}
