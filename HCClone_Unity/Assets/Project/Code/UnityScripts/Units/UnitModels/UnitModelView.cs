using UnityEngine;
using System.Collections.Generic;

public class UnitModelView : MonoBehaviour {
	[SerializeField]
	private SkinnedMeshRenderer _headArmorMeshRenderer;
	[SerializeField]
	private SkinnedMeshRenderer _bodyArmorMeshRenderer;

	[SerializeField]
	private Transform _weaponBoneRight;
	[SerializeField]
	private Transform _weaponBoneLeft;

	[SerializeField]
	private Animator _animator;

	private EUnitAnimationState _animRun = EUnitAnimationState.Run_Gun;
	private EUnitAnimationState _animAttack = EUnitAnimationState.Strike_Gun;

	private Dictionary<EUnitAnimationState, string> _animationClipName = new Dictionary<EUnitAnimationState, string>() {
		{ EUnitAnimationState.Run_Gun, "Run_Gun" },
		{ EUnitAnimationState.Run_Rifle, "Run_Rifle" },
		{ EUnitAnimationState.GetDamage_1, "GetDamage_1" },
		{ EUnitAnimationState.GetDamage_2, "GetDamage_2" },
		{ EUnitAnimationState.Strike_Gun, "Strike_Gun" },
		{ EUnitAnimationState.Strike_Rifle, "Strike_Rifle" },
		{ EUnitAnimationState.Death_FallForward, "Death_FallForward" },
		{ EUnitAnimationState.Death_FallBack, "Death_FallBack" }
	};

	private float _runAnimationSpeed = 1f;	//how fast movement animation will play
	private float _attackAnimationSpeed = 1f;	//how fast attack animation will play
	private float _hitAnimationSpeed = 1f;	//how fast hit animation will play
	private float _deathAnimationSpeed = 1f;	//how death movement animation will play

	private float _defaultMovementSpeed = 1.5f;	//how fast (in unity meters) unit will move with default animation speed
	public float MovementSpeed {
		set { _runAnimationSpeed = value / _deathAnimationSpeed; }
	}

	public void Awake() {
		if (_animator == null) {
			_animator = gameObject.GetComponent<Animator>();
		}
	}

	public void SetWeaponType(EItemKey weaponRKey, EItemKey weaponLKey) {
		if (weaponRKey != EItemKey.None && weaponLKey != EItemKey.None) {
			//TODO: setup dual animation
		} else {
			EItemType weaponType = ItemsConfig.Instance.GetItem(weaponRKey != EItemKey.None ? weaponRKey : weaponLKey).Type;
			switch (weaponType) {
				case EItemType.W_Gun:
					_animRun = EUnitAnimationState.Run_Gun;
					_animAttack = EUnitAnimationState.Strike_Gun;
					break;
				case EItemType.W_Rifle:
					_animRun = EUnitAnimationState.Run_Rifle;
					_animAttack = EUnitAnimationState.Strike_Rifle;
					break;
			}
		}
	}

	public void SetupGraphics(GameObject rhWeaponResource, GameObject lhWeaponResource, GameObject headArmorResource, GameObject bodyArmorResource) {
		//weapon
		if (_weaponBoneRight != null && rhWeaponResource != null) {
			GameObject weaponInstance = GameObject.Instantiate(rhWeaponResource) as GameObject;
			weaponInstance.transform.parent = _weaponBoneRight;
			weaponInstance.transform.localPosition = Vector3.zero;
			weaponInstance.transform.localRotation = Quaternion.identity;
		}
		if (_weaponBoneLeft != null && lhWeaponResource != null) {
			GameObject weaponInstance = GameObject.Instantiate(lhWeaponResource) as GameObject;
			weaponInstance.transform.parent = _weaponBoneLeft;
			weaponInstance.transform.localPosition = Vector3.zero;
			weaponInstance.transform.localRotation = Quaternion.identity;
		}

		//armor
		if (_headArmorMeshRenderer != null && headArmorResource != null) {
			GameObject headArmorInstance = GameObject.Instantiate(headArmorResource) as GameObject;
			SwitchMesh(headArmorInstance.GetComponent<MeshFilter>().mesh, headArmorInstance.GetComponent<MeshRenderer>().material, _headArmorMeshRenderer);
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
		_animator.speed = _runAnimationSpeed;
		_animator.Play(_animationClipName[_animRun]);
	}

	public void PlayHitAnimation(bool isCritical) {
		_animator.speed = _hitAnimationSpeed;
		_animator.Play(!isCritical ? _animationClipName[EUnitAnimationState.GetDamage_1] : _animationClipName[EUnitAnimationState.GetDamage_2]);
	}

	public void PlayAttackAnimation() {
		_animator.speed = _attackAnimationSpeed;
		_animator.Play(_animationClipName[_animAttack], 0, 0f);
	}

	public void PlayDeathAnimation() {
		_animator.speed = _deathAnimationSpeed;
	}
}
