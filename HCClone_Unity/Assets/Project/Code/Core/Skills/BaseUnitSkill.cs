using System;
public abstract class BaseUnitSkill {
	protected SkillParameters _skillParameters = null;
	public SkillParameters SkillParameters {
		get { return _skillParameters; }
	}

	protected BaseUnitBehaviour _caster;

	protected float _lastUsageTime = 0f;

	public BaseUnitSkill(SkillParameters skillParameters) {
		_skillParameters = skillParameters;
	}

	public virtual void Use(BaseUnitBehaviour caster) {
		_caster = caster;
	}

	public virtual void Break() {
		_caster.UnitData.ActiveSkills.UnregisterSkill(this);
	}

	protected virtual void StartUsage() {
		_caster.UnitData.ActiveSkills.RegisterSkill(this);
	}

	protected virtual void EndUsage() {
		_caster.UnitData.ActiveSkills.UnregisterSkill(this);
	}

	protected virtual void Clear() {
		_caster = null;
		_lastUsageTime = 0f;
	}

	public virtual void OnCasterStunned() { }

	public virtual void OnCasterDeath() {
		_caster.UnitData.ActiveSkills.UnregisterSkill(this);
	}

	public virtual void OnCasterTargetDeath() { }
}
