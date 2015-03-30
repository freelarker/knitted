using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class UIHeroSkillButton : MonoBehaviour {
	[SerializeField]
	private Image _imgAbilitiIcon;
	[SerializeField]
	private Text _txtAbilityCost;

	private Button _buttonComponent;

	private ESkillKey _skillKey = ESkillKey.None;
	private int _aggroCost = 0;

	public void Awake() {
		_buttonComponent = gameObject.GetComponent<Button>();
		_buttonComponent.onClick.AddListener(OnClick);
	}

	public void OnDestroy() {
		if (_imgAbilitiIcon.sprite != null) {
			UIResourcesManager.Instance.FreeResource(_imgAbilitiIcon.sprite);
			_imgAbilitiIcon.sprite = null;
		}
	}

	public void Setup(SkillParameters skillParams) {
		_skillKey = skillParams.Key;
		_aggroCost = skillParams.AggroCrystalsCost;

		if (!skillParams.IconPath.Equals(string.Empty)) {
			Sprite skillSprite = UIResourcesManager.Instance.GetResource<Sprite>(string.Format("{0}/{1}", GameConstants.Paths.UI_ABILITY_ICONS_RESOURCES, skillParams.IconPath));
			if (skillSprite != null) {
				_imgAbilitiIcon.sprite = skillSprite;
				_imgAbilitiIcon.gameObject.SetActive(true);
			}
		}

		if (_txtAbilityCost != null) {
			_txtAbilityCost.text = _aggroCost.ToString();
		}
	}

	private void OnClick() {
		if (Global.Instance.Player.Heroes.Current.AggroCrystals >= _aggroCost) {
			EventsAggregator.Units.Broadcast<ESkillKey>(EUnitEvent.SkillUsage, _skillKey);
		} else {
			//TODO: play some red blink animation (unable to cast ability)
		}
	}
}
