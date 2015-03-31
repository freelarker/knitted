using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;

public class UnitModelView : MonoBehaviour {
	[SerializeField]
	private SkinnedMeshRenderer _headArmorMeshRenderer;
	[SerializeField]
	private SkinnedMeshRenderer _bodyArmorMeshRenderer;

	[SerializeField]
	private Transform _weaponBoneRight;
	public Transform WeaponBoneRight {
		get { return _weaponBoneRight; }
	}

	[SerializeField]
	private Transform _weaponBoneLeft;
	public Transform WeaponBoneLeft {
		get { return _weaponBoneLeft; }
	}

	[SerializeField]
	private Animator _animator;
	public Animator Animator {
		get { return _animator; }
	}

	private float _gunStanceOffset = 0.44f;
	private float _rifleStanceOffset = 0.13f;
	private float _weaponStanceOffset = 0f;

	private WeaponView _weaponViewRH = null;
	private WeaponView _weaponViewLH = null;

	private EUnitAnimationState _animRun = EUnitAnimationState.Run_Gun;
	private EUnitAnimationState _animAttack = EUnitAnimationState.Strike_Gun;
	private EUnitAnimationState _animDeath = EUnitAnimationState.Death_FallForward;

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

	private Dictionary<EUnitAnimationState, int> _mainAnimationState = new Dictionary<EUnitAnimationState, int>() {
		{ EUnitAnimationState.Run_Gun, 1 },
		{ EUnitAnimationState.Run_Rifle, 2 },
		{ EUnitAnimationState.Strike_Gun, 11 },
		{ EUnitAnimationState.Strike_Rifle, 12 },
		{ EUnitAnimationState.Death_FallForward, 31 },
		{ EUnitAnimationState.Death_FallBack, 32 }
	};

	private float _runAnimationSpeed = 1f;	//how fast movement animation will play
	private float _attackAnimationSpeed = 1f;	//how fast attack animation will play
	//private float _hitAnimationSpeed = 1f;	//how fast hit animation will play
	private float _deathAnimationSpeed = 1f;	//how death movement animation will play

	private float _defaultMovementSpeed = 1.5f;	//how fast (in unity meters) unit will move with default animation speed
	public float MovementSpeed {
		set { _runAnimationSpeed = value / _defaultMovementSpeed; }
	}

	public void Awake() {
		if (_animator == null) {
			_animator = gameObject.GetComponent<Animator>();
		}

		_animDeath = (EUnitAnimationState)UnityEngine.Random.Range(_mainAnimationState[EUnitAnimationState.Death_FallForward], _mainAnimationState[EUnitAnimationState.Death_FallBack]);
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
					_weaponStanceOffset = _gunStanceOffset;
					break;
				case EItemType.W_Rifle:
					_animRun = EUnitAnimationState.Run_Rifle;
					_animAttack = EUnitAnimationState.Strike_Rifle;
					_weaponStanceOffset = _rifleStanceOffset;
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

			_weaponViewRH = weaponInstance.GetComponent<WeaponView>();
		}
		if (_weaponBoneLeft != null && lhWeaponResource != null) {
			GameObject weaponInstance = GameObject.Instantiate(lhWeaponResource) as GameObject;
			weaponInstance.transform.parent = _weaponBoneLeft;
			weaponInstance.transform.localPosition = Vector3.zero;
			weaponInstance.transform.localRotation = Quaternion.identity;

			_weaponViewLH = weaponInstance.GetComponent<WeaponView>();
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

	public void SetupWeapon() {
		if (_weaponViewRH != null) {
			_weaponViewRH.Setup(transform);
		}
		if (_weaponViewLH != null) {
			_weaponViewLH.Setup(transform);
		}
	}

	public void SimulateAttack() {
		_animator.speed = 0f;
		_animator.Play(_animationClipName[_animAttack], 0, 0f);
	}

	private void SwitchMesh(Mesh sourceMesh, Material sourceMaterial, SkinnedMeshRenderer target) {
		target.sharedMesh = sourceMesh;
		target.material = sourceMaterial;

		target.sharedMesh.RecalculateBounds();
		target.sharedMesh.RecalculateNormals();
	}

	#region animations
	public void StopCurrentAnimation() {
		if (_animator.GetInteger("MAS") == _mainAnimationState[_animAttack]) {
			if (_weaponViewRH != null) {
				_weaponViewRH.Stop();
			}
			if (_weaponViewLH != null) {
				_weaponViewLH.Stop();
			}
		}
		_animator.StopPlayback();
	}

	public void StopAttackAnimation() {
		if (_animator.GetInteger("MAS") == _mainAnimationState[_animAttack]) {
			StopCurrentAnimation();
		}
	}

	public void PlayRunAnimation() {
		_animator.speed = _runAnimationSpeed;
		_animator.Play(_animationClipName[_animRun]);
		_animator.SetInteger("MAS", _mainAnimationState[_animRun]);
	}

	public void PlayHitAnimation(int totalHealth, HitInfo hitInfo) {
		float healthState1 = totalHealth * 0.75f;
		float healthState2 = totalHealth * 0.5f;
		float healthState3 = totalHealth * 0.25f;

		if ((hitInfo.HealthBefore > healthState1 && hitInfo.HealthAfter < healthState1) ||
			(hitInfo.HealthBefore > healthState3 && hitInfo.HealthAfter < healthState3)) {
			_animator.SetTrigger("GetDamage1");
		} else if (hitInfo.HealthBefore > healthState2 && hitInfo.HealthAfter < healthState2) {
			_animator.SetTrigger("GetDamage2");
		}
	}

	public void PlayAttackAnimation(float distanceToTarget) {
		_animator.speed = _attackAnimationSpeed;
		_animator.Play(_animationClipName[_animAttack], 0, 0f);
		_animator.SetInteger("MAS", _mainAnimationState[_animAttack]);

		distanceToTarget -= _weaponStanceOffset;
		if (_weaponViewRH != null) {
			_weaponViewRH.Attack(distanceToTarget);
		}
		if (_weaponViewLH != null) {
			_weaponViewLH.Attack(distanceToTarget);
		}
	}

	public void PlayDeathAnimation(Action onAnimationEnd) {
		_animator.speed = _deathAnimationSpeed;
		_animator.SetInteger("MAS", _mainAnimationState[_animDeath]);
		_animator.Play(_animationClipName[_animDeath]);

		StartCoroutine(DeathAnimationEndWaiter(onAnimationEnd));
	}

	public void PlayWinAnimation() {
		//TODO: win animation
		_animator.speed = 0f;
		_animator.StopPlayback();
	}

	private IEnumerator DeathAnimationEndWaiter(Action onAnimationEnd) {
		yield return null;
		yield return new WaitForSeconds(_animator.GetCurrentAnimatorStateInfo(0).length);
		onAnimationEnd();
	}
	#endregion
}
