using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class UIHeroSkillButton : MonoBehaviour {
	private Button _buttonComponent;

	private ESkillKey _skillKey = ESkillKey.None;
	private int _aggroCost = 0;

	public void Awake() {
		_buttonComponent = gameObject.GetComponent<Button>();
		_buttonComponent.onClick.AddListener(OnClick);
	}

	public void Setup(ESkillKey skillKey, int aggroCost) {
		_skillKey = skillKey;
		_aggroCost = aggroCost;

		//TODO: load skill image
		//TODO: setup aggro cost
	}

	private void OnClick() {
		if (Global.Instance.Player.Heroes.Current.AggroCrystals >= _aggroCost) {
			EventsAggregator.Units.Broadcast<ESkillKey>(EUnitEvent.SkillUsage, _skillKey);
		} else {
			//TODO: play some red blink animation (unable to cast ability)
		}
	}
}
