using UnityEngine;

public class SkillStunGrenade : BaseUnitSkill {
	private string _grenadePrefabPath = "Skills/StunGrenade_grenade";
	private float _minThrowTime = 0.65f;

	private StunGrenadeController _grenadeController = null;

	public SkillStunGrenade(SkillParameters skillParameters) : base(skillParameters) { }

	public override void Use(BaseUnitBehaviour caster) {
		//check caster is alive
		if (caster.UnitData.IsDead) {
			return;
		}

		//check aggro price
		BaseHero heroData = caster.UnitData as BaseHero;
		if (heroData.AggroCrystals < _skillParameters.AggroCrystalsCost) {
			return;
		}

		//check cooldown
		if (_lastUsageTime != 0f && Time.time - _lastUsageTime < _skillParameters.CooldownTime) {
			return;
		}

		base.Use(caster);
		StartUsage();
	}

	public override void Break() {
		base.Break();
	}

	protected override void StartUsage() {
		BaseUnitBehaviour target = GetFarthestOpponent();
		if (target != null) {
			CreateGrenade();
			if (_grenadeController != null && !_grenadeController.IsInFlight) {
				(_caster.UnitData as BaseHero).UseSkill(_skillParameters);
				_lastUsageTime = Time.time;

				ThrowGrenade(target);
			}
		}
	}

	protected override void EndUsage() {
		_caster = null;

		//TODO: start icon cooldown
	}

	public override void OnCasterStunned() { }

	public override void OnCasterDeath() { }

	public override void OnCasterTargetDeath() { }

	private BaseUnitBehaviour GetFarthestOpponent() {
		BaseUnitBehaviour result = null;
		ArrayRO<BaseUnitBehaviour> opposedUnits = _caster.IsAlly ? FightManager.SceneInstance.EnemyUnits : FightManager.SceneInstance.AllyUnits;
		for (int i = 0; i < opposedUnits.Length; i++) {
			if (opposedUnits[i] != null && !opposedUnits[i].UnitData.IsDead) {
				if (result == null) {
					result = opposedUnits[i];
				} else if (Vector3.Distance(_caster.CachedTransform.position, opposedUnits[i].CachedTransform.position) > Vector3.Distance(_caster.CachedTransform.position, result.CachedTransform.position)) {
					result = opposedUnits[i];
				}
			}
		}
		return result;
	}

	private void CreateGrenade() {
		if (_grenadeController == null) {
			GameObject grenadeGO = GameObject.Instantiate(Resources.Load(_grenadePrefabPath) as GameObject) as GameObject;
			_grenadeController = grenadeGO.GetComponent<StunGrenadeController>();
			if (_grenadeController == null) {
				grenadeGO.AddComponent<StunGrenadeController>();
			}
			_grenadeController.transform.SetParent(_caster.CachedTransform.parent);
		}
		_grenadeController.transform.position = _caster.ModelView.WeaponBoneRight.position;
	}

	private void ThrowGrenade(BaseUnitBehaviour target) {
		_grenadeController.Throw(Mathf.Max(Vector3.Distance(_caster.CachedTransform.position, target.CachedTransform.position) * 0.1f, _minThrowTime), _grenadeController.transform.position, target.CachedTransform.position, 2f, OnGrenadeTargetReached);
		//TODO: play throw animation
	}

	private void OnGrenadeTargetReached(Vector3 position) {
		ArrayRO<BaseUnitBehaviour> opposedUnits = _caster.IsAlly ? FightManager.SceneInstance.EnemyUnits : FightManager.SceneInstance.AllyUnits;
		for (int i = 0; i < opposedUnits.Length; i++) {
			if (opposedUnits[i] != null && !opposedUnits[i].UnitData.IsDead) {
				if (Vector3.Distance(position, opposedUnits[i].CachedTransform.position) <= _skillParameters.Radius) {
					opposedUnits[i].Stun(_skillParameters.Duration);
				}
			}
		}

		EndUsage();
	}
}
