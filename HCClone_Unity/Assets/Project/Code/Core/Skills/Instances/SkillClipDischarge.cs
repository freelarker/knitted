using System.Collections;
using UnityEngine;

public class SkillClipDischarge : BaseUnitSkill {
	public SkillClipDischarge(SkillParameters skillParameters) : base(skillParameters) { }

	private int _shotsLeft = -1;
	private float _lastUsageTime = 0f;

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

	protected override void StartUsage() {
		Debug.LogWarning("=== StartUsage(ClipDischarge)" + Time.time);

		base.StartUsage();

		_shotsLeft = (int)_skillParameters.Duration;
		_wfs = new WaitForSeconds(0.25f);

		(_caster.UnitData as BaseHero).UseSkill(_skillParameters);
		_caster.StopTargetAttack(true);

		GameTimer.Instance.RunCoroutine(Shoot);
	}

	protected override void EndUsage() {
		Debug.LogWarning("=== EndUsage(ClipDischarge)" + Time.time);

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

		_lastUsageTime = 0;
		_shotsLeft = -1;
		_wfs = null;
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
