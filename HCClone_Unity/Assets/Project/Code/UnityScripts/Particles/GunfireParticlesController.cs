﻿using UnityEngine;
using System.Collections;
using System;

public class GunfireParticlesController : MonoBehaviour {
	[SerializeField]
	private GameObject _gunfireParticlePrefab;
	[SerializeField]
	private GameObject _tracerParticlePrefab;
	
	[SerializeField]
	private int _pelletsPerAttack = 1;
	[SerializeField]
	private float _firstPelletDelay = 0f;
	[SerializeField]
	private float _nextPelletDelay = 0f;

	[SerializeField]
	private float _pelletWorldLength = 0;

	private float _pelletStartOffset = 0f;
	private int _pelletExtendAmount = 4;

	private GameObject _gunfireParticleInstance;
	private TracerParticleController[] _tracerParticleInstances;

	private WaitForSeconds _wfsGunfireDuration;
	private WaitForSeconds _wfsFirstPellek;
	private WaitForSeconds _wfsNextPellek;

	private Transform _tracersRoot;

	public void Awake() {
		_wfsGunfireDuration = new WaitForSeconds(0.075f);
		if (_firstPelletDelay > 0f) {
			_wfsFirstPellek = new WaitForSeconds(_firstPelletDelay);
		}
		if(_nextPelletDelay > 0f) {
			_wfsNextPellek = new WaitForSeconds(_nextPelletDelay);
		}
	}

	public void Setup(Transform gunfireRoot, Transform tracersRoot, float pelletStartOffset) {
		_tracersRoot = tracersRoot;
		_pelletStartOffset = pelletStartOffset;

		_gunfireParticleInstance = (GameObject.Instantiate(_gunfireParticlePrefab) as GameObject);
		_gunfireParticleInstance.transform.SetParent(gunfireRoot);
		_gunfireParticleInstance.transform.localPosition = Vector3.zero;
		_gunfireParticleInstance.transform.localRotation = Quaternion.identity;
		_gunfireParticleInstance.SetActive(false);

		ExtendPelletsPool();
	}

	public void Play(float distanceToTarget) {
		Play(distanceToTarget, _tracersRoot.position);
	}

	public void Play(float distanceToTarget, Vector3 startPosition) {
		distanceToTarget = distanceToTarget - _pelletStartOffset - _pelletWorldLength;

		StartCoroutine(PlayInternal(distanceToTarget, startPosition));
	}

	public void Stop() {
		StopAllCoroutines();

		_gunfireParticleInstance.SetActive(false);
		for (int i = 0; i < _tracerParticleInstances.Length; i++) {
			_tracerParticleInstances[i].gameObject.SetActive(false);
		}
	}

	private IEnumerator PlayInternal(float distanceToTarget, Vector3 pelletStartPosition) {
		TracerParticleController currentPellet = null;

		if (_wfsFirstPellek != null) {
			yield return _wfsFirstPellek;
		}

		for (int i = 0; i < _pelletsPerAttack; i++) {
			currentPellet = GetFreePellet();
			if (currentPellet == null) {
				ExtendPelletsPool();
				currentPellet = GetFreePellet();
			}

			currentPellet.Play(distanceToTarget, pelletStartPosition);
			_gunfireParticleInstance.SetActive(true);
			yield return _wfsGunfireDuration;
			_gunfireParticleInstance.SetActive(false);

			if (_wfsNextPellek != null) {
				yield return _wfsNextPellek;
			}
		}
	}

	private void ExtendPelletsPool() {
		int i = 0;

		TracerParticleController[] pellets = null;
		if(_tracerParticleInstances != null) {
			pellets = new TracerParticleController[_tracerParticleInstances.Length + _pelletExtendAmount];
			Array.Copy(_tracerParticleInstances, pellets, _tracerParticleInstances.Length);
			i = _tracerParticleInstances.Length;
		} else {
			pellets = new TracerParticleController[_pelletExtendAmount];
		}

		GameObject tracerInstance = null;
		for (; i < pellets.Length; i++) {
			tracerInstance = (GameObject.Instantiate(_tracerParticlePrefab) as GameObject);
			tracerInstance.transform.SetParent(_tracersRoot);
			tracerInstance.transform.localPosition = Vector3.zero;
			tracerInstance.transform.localRotation = Quaternion.identity;

			pellets[i] = tracerInstance.AddComponent<TracerParticleController>();
			pellets[i].Stop();
		}

		_tracerParticleInstances = pellets;
	}

	private TracerParticleController GetFreePellet() {
		for (int i = 0; i < _tracerParticleInstances.Length; i++) {
			if (!_tracerParticleInstances[i].gameObject.activeInHierarchy) {
				return _tracerParticleInstances[i];
			}
		}
		return null;
	}
}