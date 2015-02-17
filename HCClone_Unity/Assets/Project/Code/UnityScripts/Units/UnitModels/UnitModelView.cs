using UnityEngine;
using System.Collections.Generic;

public class UnitModelView : MonoBehaviour {
	[SerializeField]
	private SkinnedMeshRenderer _weaponMesh;
	[SerializeField]
	private SkinnedMeshRenderer _armorMesh;

	[SerializeField]
	private Transform _weaponBone;

	[SerializeField]
	private Animator _animator;

	public void Awake() {
		if (_animator == null) {
			_animator = gameObject.GetComponent<Animator>();
		}
	}

	public void Setup(GameObject weaponResource, GameObject armorResource) {
		if (_weaponBone != null && weaponResource != null) {
			GameObject weaponInstance = GameObject.Instantiate(weaponResource) as GameObject;
			weaponInstance.transform.parent = _weaponBone;
			weaponInstance.transform.localPosition = Vector3.zero;
			weaponInstance.transform.localRotation = Quaternion.identity;

			_weaponMesh = weaponInstance.GetComponent<SkinnedMeshRenderer>();
		}
		if (armorResource != null) {
			GameObject armorInstance = GameObject.Instantiate(armorResource) as GameObject;
			_armorMesh = armorInstance.GetComponent<SkinnedMeshRenderer>();
		}

		Debug.LogWarning("=== " + transform.parent.gameObject.name + " --- w: " + weaponResource + ", a: " + armorResource);
		//TODO: setup armor mesh
		//TODO: setup weapon mesh
	}

	public void PlayRunAnimation() {
		_animator.Play(_weaponMesh != null ? EUnitAnimationState.Run_Gun.ToString() : EUnitAnimationState.Run_NoGun.ToString());
	}

	public void PlayHitAnimation(bool isCritical) {
		_animator.Play(!isCritical ? EUnitAnimationState.GetDamage_1.ToString() : EUnitAnimationState.GetDamage_2.ToString());
	}

	public void PlayAttackAnimation() {
		
	}

	public void PlayDeathAnimation() {

	}
}
