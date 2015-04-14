using System;
using System.Collections;
using UnityEngine;

public class TracerParticleController : MonoBehaviour {
	private float _speed = 15f;

	private float _timeEnd;
	private Vector3 _positionEnd;

	private Transform _cachedTransform;

	private Action<Vector3> _flightEndCallback;

	public void Awake() {
		_cachedTransform = transform;
	}

	public void Update() {
		_cachedTransform.position = Vector3.MoveTowards(_cachedTransform.position, _positionEnd, _speed * Time.deltaTime);

		if (Time.time >= _timeEnd) {
			Blow();
		}
	}

	public void Play(float distance, Vector3 startPosition, Action<Vector3> flightEndCallback) {
		_flightEndCallback = flightEndCallback;

		_cachedTransform.position = startPosition;
		_positionEnd = _cachedTransform.position + _cachedTransform.forward * distance;
		_timeEnd = Time.time + distance / _speed;

		gameObject.SetActive(true);
		
	}

	private void Blow() {
		Stop();

		if (_flightEndCallback != null) {
			_flightEndCallback(_positionEnd);
			_flightEndCallback = null;
		}
	}

	public void Stop() {
		gameObject.SetActive(false);
	}
}
