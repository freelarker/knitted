using System.Collections;
using UnityEngine;

public class WeaponView : MonoBehaviour {
	[SerializeField]
	private Transform _tracerParticleParent;
	[SerializeField]
	private Transform _gunfireParticleParent;

	[SerializeField]
	private ParticleSystem _tracerParticlePrefab;
	[SerializeField]
	private ParticleSystem _gunfireParticlePrefab; 

	[SerializeField]
	private float _bulletParticleDelay = 0f;

	private TracerParticleController _tpc;
	public TracerParticleController ParticleController {
		get { return _tpc; }
	}
	private ParticleSystem _gunfireParticleInstance;

	private float _particleGunOffset = 0f;

	public void Awake() {
		GameObject particleTracer = GameObject.Instantiate(_tracerParticlePrefab.gameObject) as GameObject;
		particleTracer.transform.SetParent(_tracerParticleParent);
		particleTracer.transform.localPosition = Vector3.zero;
		particleTracer.transform.localRotation = Quaternion.identity;
		particleTracer.transform.localScale = Vector3.one;
		_tpc = particleTracer.GetComponent<TracerParticleController>();

		_gunfireParticleInstance = (GameObject.Instantiate(_gunfireParticlePrefab.gameObject) as GameObject).GetComponent<ParticleSystem>();
		_gunfireParticleInstance.transform.SetParent(_gunfireParticleParent);
		_gunfireParticleInstance.transform.localPosition = Vector3.zero;
		_gunfireParticleInstance.transform.localRotation = Quaternion.identity;
		_gunfireParticleInstance.transform.localScale = Vector3.one;
	}

	public void Setup(Transform particleParent) {
		_particleGunOffset = particleParent.InverseTransformPoint(_tracerParticleParent.TransformPoint(_tracerParticleParent.localPosition)).x;
		_tpc.transform.parent = particleParent;
	}

	public void Attack(float distanceToTarget) {
		if (distanceToTarget > 0f) {
			distanceToTarget -= _particleGunOffset;

			_tpc.SetParticleDistance(distanceToTarget);

			_tpc.Particles.Simulate(_bulletParticleDelay, true);
			_tpc.Particles.Play(true);

			_gunfireParticleInstance.gameObject.SetActive(true);
			_gunfireParticleInstance.Simulate(0f, true);
			_gunfireParticleInstance.Play(true);
		}
	}

	public void Stop() {
		_tpc.Particles.Stop(true);
	}
}
