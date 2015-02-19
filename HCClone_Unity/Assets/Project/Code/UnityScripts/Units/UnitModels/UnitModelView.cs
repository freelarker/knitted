using UnityEngine;
using System.Collections.Generic;

public class UnitModelView : MonoBehaviour {
	[SerializeField]
	private SkinnedMeshRenderer _weaponMeshRenderer;

	[SerializeField]
	private SkinnedMeshRenderer _hearArmorMeshRenderer;
	[SerializeField]
	private SkinnedMeshRenderer _bodyArmorMeshRenderer;

	[SerializeField]
	private Transform _weaponBone;

	[SerializeField]
	private Animator _animator;

	public void Awake() {
		if (_animator == null) {
			_animator = gameObject.GetComponent<Animator>();
		}
	}

	public void Setup(GameObject weaponResource, GameObject headArmorResource, GameObject bodyArmorResource) {
		//Debug.LogWarning("=== " + transform.parent.gameObject.name + " --- w: " + weaponResource + ", hA: " + headArmorResource + ", bA: " + bodyArmorResource);

		//weapon
		if (_weaponBone != null && weaponResource != null) {
			GameObject weaponInstance = GameObject.Instantiate(weaponResource) as GameObject;
			weaponInstance.transform.parent = _weaponBone;
			weaponInstance.transform.localPosition = Vector3.zero;
			weaponInstance.transform.localRotation = Quaternion.identity;

			_weaponMeshRenderer = weaponInstance.GetComponent<SkinnedMeshRenderer>();
		}

		//armor
		if (_hearArmorMeshRenderer != null && headArmorResource != null) {
			GameObject headArmorInstance = GameObject.Instantiate(headArmorResource) as GameObject;
			SwitchMesh(headArmorInstance.GetComponent<MeshFilter>().mesh, headArmorInstance.GetComponent<MeshRenderer>().material, _hearArmorMeshRenderer);
			GameObject.Destroy(headArmorInstance);
		}
		if (_bodyArmorMeshRenderer != null && bodyArmorResource != null) {
			GameObject bodyArmorInstance = GameObject.Instantiate(bodyArmorResource) as GameObject;
			SwitchMesh(bodyArmorInstance.GetComponent<MeshFilter>().mesh, bodyArmorInstance.GetComponent<MeshRenderer>().material, _bodyArmorMeshRenderer);
			GameObject.Destroy(bodyArmorInstance);
		}
	}

	private void SwitchMesh(Mesh sourceMesh, Material sourceMaterial, SkinnedMeshRenderer target) {
		target.sharedMesh = sourceMesh;
		target.material = sourceMaterial;

		target.sharedMesh.RecalculateBounds();
		target.sharedMesh.RecalculateNormals();
	}

	public void PlayRunAnimation() {
		_animator.Play(_weaponMeshRenderer != null ? EUnitAnimationState.Run_Gun.ToString() : EUnitAnimationState.Run_NoGun.ToString());
	}

	public void PlayHitAnimation(bool isCritical) {
		_animator.Play(!isCritical ? EUnitAnimationState.GetDamage_1.ToString() : EUnitAnimationState.GetDamage_2.ToString());
	}

	public void PlayAttackAnimation() {
		
	}

	public void PlayDeathAnimation() {

	}
}
