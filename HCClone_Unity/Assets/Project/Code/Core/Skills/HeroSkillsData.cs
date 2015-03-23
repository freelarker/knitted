using UnityEngine;

[System.Serializable]
public class HeroSkillsData {
	[SerializeField]
	private EUnitKey _heroKey = EUnitKey.Idle;
	public EUnitKey HeroKey {
		get { return _heroKey; }
	}

	[SerializeField]
	private SkillParameters[] _skills = null;
	private ArrayRO<SkillParameters> _skillsRO = null;
	public ArrayRO<SkillParameters> Skills {
		get {
			if (_skillsRO == null) {
				_skillsRO = new ArrayRO<SkillParameters>(_skills);
			}
			return _skillsRO;
		}
	}
}
