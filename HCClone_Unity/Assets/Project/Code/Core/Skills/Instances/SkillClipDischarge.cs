using System.Collections;
using UnityEngine;

public class SkillClipDischarge : BaseUnitSkill {
	public SkillClipDischarge(SkillParameters skillParameters) : base(skillParameters) { }

	private int _shotsLeft = -1;

	private WaitForSeconds _wfs = null;

	public override void Use(BaseUnitBehaviour caster) {
		//check caster is alive
		if (caster.UnitData.IsDead) {
			return;
		}

		//check aggro price
		if ((caster.UnitData as BaseHero).AggroCrystals < _skillParameters.AggroCrystalsCost) {
			return;
		}

		//check target locked
		if (caster.UnitPathfinder.CurrentState != EUnitMovementState.WatchEnemy) {
			return;
		}

		//check cooldown
		if (_lastUsageTime != 0f && Time.time - _lastUsageTime < _skillParameters.CooldownTime) {
			return;
		}

		//check already in use
		if (_isUsing) {
			return;
		}

		base.Use(caster);

		StartUsage();
	}

	public override void Break() {
		base.Break();

		GameTimer.Instance.FinishCoroutine(Shoot);
		Clear();
	}

	protected override void StartUsage() {
		base.StartUsage();

		(_caster.UnitData as BaseHero).UseSkill(_skillParameters);
		_shotsLeft = (int)_skillParameters.Duration;
		_wfs = new WaitForSeconds(0.15f);			//duration betweeen attacks
		StartCooldown();

		(_caster.UnitData as BaseHero).UseSkill(_skillParameters);
		_caster.StopTargetAttack(true);

		GameTimer.Instance.RunCoroutine(Shoot);
	}

	protected override void EndUsage() {
		if (_caster != null) {
			base.EndUsage();

			BaseUnitBehaviour caster = _caster;

			GameTimer.Instance.FinishCoroutine(Shoot);
			Clear();

			caster.StartTargetAttack();
		}
	}

	protected override void Clear() {
		base.Clear();

		_shotsLeft = -1;
		_wfs = null;
	}

	public override void OnCasterStunned() {
		Break();
	}

	public override void OnCasterDeath() {
		Break();
	}

	public override void OnCasterTargetDeath() {
		EndUsage();
	}

	#region shooting
	private IEnumerator Shoot() {
		_caster.ModelView.PlaySkillAnimation(ESkillKey.ClipDischarge, _caster.DistanceToTarget);
		while (_shotsLeft > 0) {
			PerformShot();
			_shotsLeft--;
			if (_shotsLeft > 0) {
				yield return _wfs;
			}
		}

		EndUsage();
	}

	private void PerformShot() {
		EventsAggregator.Fight.Broadcast<BaseUnitBehaviour, BaseUnitBehaviour>(EFightEvent.PerformAttack, _caster, _caster.TargetUnit);
	}
	#endregion
}
