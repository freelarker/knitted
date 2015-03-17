using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class TracerParticleController : MonoBehaviour {
	private ParticleSystem _particles = null;
	public ParticleSystem Particles {
		get { return _particles; }
	}

	public void Awake() {
		_particles = gameObject.GetComponent<ParticleSystem>();
	}

	public void SetParticleDistance(float distance) {
		_particles.startLifetime = distance / _particles.startSpeed;
	}
}
