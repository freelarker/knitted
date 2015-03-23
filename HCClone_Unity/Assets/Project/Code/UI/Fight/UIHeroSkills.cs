using UnityEngine;
using UnityEngine.UI;

public class UIHeroSkills : MonoBehaviour {
	[SerializeField]
	private UIHeroSkillButton[] _skillButtons;

	public void Start() {
		HeroSkillsData heroSkills = SkillsConfig.Instance.GetHeroSkillsData(Global.Instance.Player.Heroes.Current.Data.Key);
		if (heroSkills != null) {
			for (int i = 0; i < heroSkills.Skills.Length; i++) {
				if (i >= _skillButtons.Length) {
					break;
				}
				_skillButtons[i].Setup(heroSkills.Skills[i].Key, heroSkills.Skills[i].AggroCrystalsCost);
			}
		}
	}
}
