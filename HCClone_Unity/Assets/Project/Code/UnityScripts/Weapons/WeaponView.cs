using System.Collections;
using UnityEngine;

public class WeaponView : MonoBehaviour {
	[SerializeField]
	private Transform _particleParent;

	[SerializeField]
	private TracerParticleController _tpc;
	public TracerParticleController ParticleController {
		get { return _tpc; }
	}

	[SerializeField]
	private float _bulletParticleDelay = 0f;

	private float _particleGunOffset = 0f;

	public void Setup(Transform particleParent) {
		_particleGunOffset = particleParent.InverseTransformPoint(_particleParent.TransformPoint(_particleParent.localPosition)).x;
		_tpc.transform.parent = particleParent;
	}

	public void Attack(float distanceToTarget) {
		if (distanceToTarget > 0f) {
			StartCoroutine(AttackInternal(distanceToTarget));
		}
	}

	public void Stop() {
		_tpc.Particles.Stop(true);
	}

	private IEnumerator AttackInternal(float distanceToTarget) {
		distanceToTarget -= _particleGunOffset;
		
		_tpc.SetParticleDistance(distanceToTarget);

		_tpc.Particles.Simulate(_bulletParticleDelay, true);
		_tpc.Particles.Play(true);

		yield break;
	}
}
