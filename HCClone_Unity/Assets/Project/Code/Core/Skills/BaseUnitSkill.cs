public abstract class BaseUnitSkill {
	protected SkillParameters _skillParameters = null;
	public SkillParameters SkillParameters {
		get { return _skillParameters; }
	}

	protected BaseUnitBehaviour _caster;

	public BaseUnitSkill(SkillParameters skillParameters) {
		_skillParameters = skillParameters;
	}

	public virtual void Use(BaseUnitBehaviour caster) {
		_caster = caster;
	}

	protected virtual void StartUsage() {
		_caster.UnitData.ActiveSkills.RegisterSkill(this);
	}

	protected virtual void EndUsage() {
		_caster.UnitData.ActiveSkills.UnregisterSkill(this);
	}

	protected virtual void Clear() {
		_caster = null;
	}
}
