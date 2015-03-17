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

	private float _particleWorldOffset = 0f;

	public void Setup(Transform particleParent) {
		_tpc.transform.SetParent(particleParent, true);
		_particleWorldOffset = _particleParent.TransformPoint(_particleParent.localPosition).x;
	}

	public void Attack(float distanceToTarget) {
		StartCoroutine(AttackInternal(distanceToTarget));
	}

	public void Stop() {
		_tpc.Particles.Stop(true);
	}

	private IEnumerator AttackInternal(float distanceToTarget) {
		//yield return new WaitForEndOfFrame();

		distanceToTarget -= _particleWorldOffset;

		_tpc.transform.position = _particleParent.position;
		_tpc.transform.rotation = _particleParent.rotation;

		_tpc.SetParticleDistance(distanceToTarget);

		_tpc.Particles.Simulate(_bulletParticleDelay, true);
		_tpc.Particles.Play(true);

		yield break;
	}
}
