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
		BaseHero heroData = caster.UnitData as BaseHero;
		if (heroData.AggroCrystals < _skillParameters.AggroCrystalsCost) {
			return;
		}

		//check target in range
		if (!caster.TargetInRange) {
			return;
		}

		//check cooldown
		if (_lastUsageTime != 0f && Time.time - _lastUsageTime < _skillParameters.CooldownTime) {
			return;
		}

		//check already in use
		if(_shotsLeft != -1) {
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

		_shotsLeft = (int)_skillParameters.Duration;
		_wfs = new WaitForSeconds(0.25f);			//duration betweeen attacks

		(_caster.UnitData as BaseHero).UseSkill(_skillParameters);
		_caster.StopTargetAttack(true);

		GameTimer.Instance.RunCoroutine(Shoot);
	}

	protected override void EndUsage() {
		base.EndUsage();

		BaseUnitBehaviour caster = _caster;

		GameTimer.Instance.FinishCoroutine(Shoot);
		Clear();
		_lastUsageTime = Time.time;

		caster.StartTargetAttack();
		
		//TODO: start visual cooldown (on skill icon)
	}

	protected override void Clear() {
		base.Clear();

		_shotsLeft = -1;
		_wfs = null;
	}

	public override void OnCasterDeath() {
		Break();
	}

	public override void OnCasterTargetDeath() {
		Break();
	}

	#region shooting
	private IEnumerator Shoot() {
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
		//TODO: play shot animation
		EventsAggregator.Fight.Broadcast<BaseUnitBehaviour, BaseUnitBehaviour>(EFightEvent.PerformAttack, _caster, _caster.TargetUnit);
	}
	#endregion
}
