using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIWindowBattleSetup : UIWindow {
	[SerializeField]
	private Text _txtPlayerLeadershipCaption;
	[SerializeField]
	private Text _txtPlayerLeadershipAmount;

	[SerializeField]
	private Image _imgHero;
	[SerializeField]
	private Button _btnHiredSoldier;
	[SerializeField]
	private float _offsetImageHiredSoldiers = 40f;

	[SerializeField]
	private UIBattleSetupUnitInfo _availableSoldierInfo;
	[SerializeField]
	private float _offsetImageAvailableSoldiersX = 70f;
	[SerializeField]
	private float _offsetImageAvailableSoldiersY = 265f;

	[SerializeField]
	private Button _btnBack;
	[SerializeField]
	private Button _btnPlay;

	private BaseSoldierData[] _availableSoldiers = null;
	private int[] _hiredSoldiers = null;	//indexes in available units

	private UIBattleSetupUnitInfo[] _availableSoldiersInfo = null;
	private Button[] _hiredSoldiersButtons = null;

	private int _leadershipSpent = 0;

	public void Awake() {
		_btnPlay.onClick.AddListener(OnBtnPlayClick);
		_btnBack.onClick.AddListener(OnBtnBackClick);
	}

	public void Start() {
		//WARNING! temp: need to call setup before show
		//TODO: save previous setup
		Setup();
	}

	#region setup
	public void Setup() {
		SetupAvailableUnits();
		SetupHiredUnits();

		UpdateLeadership();
		UpdateSoldiersHireAvailability();
	}

	private void SetupAvailableUnits() {
		float availableSoldierImageWidth = _availableSoldierInfo.Button.image.rectTransform.rect.width;

		EUnitKey[] units = Global.Instance.Player.City.AvailableUnits.ToArray();
		_availableSoldiers = new BaseSoldierData[units.Length];
		for (int i = 0; i < units.Length; i++) {
			_availableSoldiers[i] = UnitsConfig.Instance.GetSoldierData(units[i]);
		}

		_availableSoldiersInfo = new UIBattleSetupUnitInfo[_availableSoldiers.Length];
		_availableSoldiersInfo[0] = _availableSoldierInfo;
		for (int i = 0; i < _availableSoldiers.Length; i++) {
			if (i > 0) {
				_availableSoldiersInfo[i] = (GameObject.Instantiate(_availableSoldierInfo.gameObject) as GameObject).GetComponent<UIBattleSetupUnitInfo>();
				_availableSoldiersInfo[i].transform.SetParent(_availableSoldierInfo.transform.parent, false);
				_availableSoldiersInfo[i].gameObject.GetComponent<RectTransform>().anchoredPosition = _availableSoldierInfo.gameObject.GetComponent<RectTransform>().anchoredPosition + new Vector2(i * (availableSoldierImageWidth + _offsetImageAvailableSoldiersX), 0f);
			}
			int iTmp = i;	//some spike: without this array.Length is passed to listener
			_availableSoldiersInfo[i].Button.onClick.AddListener(() => { HireSoldier(iTmp); });
			_availableSoldiersInfo[i].LeadershipCost.text = _availableSoldiers[i].LeadershipCost.ToString();

			Image soldierIcon = _availableSoldiersInfo[i].Button.image;
			Sprite enemyIconResource = UIResourcesManager.Instance.GetResource<Sprite>(GetUnitIconResourcePath(_availableSoldiers[i].IconName));
			if (enemyIconResource != null) {
				soldierIcon.sprite = enemyIconResource;
			}
		}
	}

	private void SetupHiredUnits() {
		Sprite heroIconResource = UIResourcesManager.Instance.GetResource<Sprite>(GetUnitIconResourcePath(Global.Instance.Player.Heroes.Current.Data.IconName));
		if (heroIconResource != null) {
			_imgHero.sprite = heroIconResource;
		}

		float hiredSoldierImageWidth = _btnHiredSoldier.image.rectTransform.rect.width;

		_hiredSoldiers = new int[UnitsConfig.Instance.MaxUnitsHeroCanHire];
		_hiredSoldiersButtons = new Button[_hiredSoldiers.Length];
		_hiredSoldiersButtons[0] = _btnHiredSoldier;
		for (int i = 0; i < _hiredSoldiers.Length; i++) {
			_hiredSoldiers[i] = -1;

			if (i > 0) {
				_hiredSoldiersButtons[i] = (GameObject.Instantiate(_btnHiredSoldier.gameObject) as GameObject).GetComponent<Button>();
				_hiredSoldiersButtons[i].transform.SetParent(_btnHiredSoldier.transform.parent, false);
				_hiredSoldiersButtons[i].gameObject.GetComponent<RectTransform>().anchoredPosition = _btnHiredSoldier.gameObject.GetComponent<RectTransform>().anchoredPosition + new Vector2(i * (hiredSoldierImageWidth + _offsetImageHiredSoldiers), 0f);
			}
			int iTmp = i;	//some spike: without this array.Length is passed to listener
			_hiredSoldiersButtons[i].onClick.AddListener(() => { DismissSoldier(iTmp); });

			if (_hiredSoldiers[i] >= 0) {
				_hiredSoldiersButtons[i].image.sprite = _availableSoldiersInfo[_hiredSoldiers[i]].Button.image.sprite;
			} else {
				_hiredSoldiersButtons[i].image.enabled = false;
			} 
		}
	}
	#endregion

	#region graphics dynamical update
	private void UpdateLeadership() {
		_leadershipSpent = 0;
		for (int i = 0; i < _hiredSoldiers.Length; i++) {
			if (_hiredSoldiers[i] >= 0) {
				_leadershipSpent += _availableSoldiers[_hiredSoldiers[i]].LeadershipCost;
			}
		}
		_txtPlayerLeadershipAmount.text = (Global.Instance.Player.Heroes.Current.Leadership - _leadershipSpent).ToString();
	}

	private void UpdateSoldiersHireAvailability() {
		int totalLeadershop = Global.Instance.Player.Heroes.Current.Leadership;
		for (int i = 0; i < _availableSoldiersInfo.Length; i++) {
			_availableSoldiersInfo[i].LeadershipCost.color = _leadershipSpent + _availableSoldiers[i].LeadershipCost > totalLeadershop ? Color.red : Color.white;
		}
	}
	#endregion

	#region hire/dismiss
	private void HireSoldier(int unitIndex) {
		if (_leadershipSpent + _availableSoldiers[unitIndex].LeadershipCost > Global.Instance.Player.Heroes.Current.Leadership) {
			return;
		}

		for (int i = 0; i < _hiredSoldiers.Length; i++) {
			if (_hiredSoldiers[i] < 0) {
				_hiredSoldiers[i] = unitIndex;
				_hiredSoldiersButtons[i].image.sprite = _availableSoldiersInfo[unitIndex].Button.image.sprite;
				_hiredSoldiersButtons[i].image.enabled = true;
				break;
			}
		}

		UpdateLeadership();
		UpdateSoldiersHireAvailability();
	}

	private void DismissSoldier(int unitIndex) {
		for (int i = unitIndex; i < _hiredSoldiers.Length - 1; i++) {
			_hiredSoldiers[i] = _hiredSoldiers[i + 1];

			_hiredSoldiersButtons[i].image.sprite = _hiredSoldiersButtons[i + 1].image.sprite;
			_hiredSoldiersButtons[i].image.enabled = _hiredSoldiersButtons[i].image.sprite != null;
		}

		_hiredSoldiers[_hiredSoldiers.Length - 1] = -1;
		_hiredSoldiersButtons[_hiredSoldiersButtons.Length - 1].image.sprite = null;
		_hiredSoldiersButtons[_hiredSoldiersButtons.Length - 1].image.enabled = false;

		UpdateLeadership();
		UpdateSoldiersHireAvailability();
	}
	#endregion

	#region button listeners
	private void OnBtnPlayClick() {
		List<BaseSoldier> soldiers = new List<BaseSoldier>();
		for (int i = 0; i < _hiredSoldiers.Length; i++) {
			if (_hiredSoldiers[i] >= 0) {
				soldiers.Add(new BaseSoldier(_availableSoldiers[_hiredSoldiers[i]]));
			}
		}
		Global.Instance.CurrentMission.SelectedSoldiers = new ArrayRO<BaseSoldier>(soldiers.ToArray());

		Application.LoadLevel(GameConstants.Scenes.FIGHT);
	}

	private void OnBtnBackClick() {
		Hide();

		//TODO: do not show, UIWindowsManager must show previous open window
		UIWindowsManager.Instance.GetWindow <UIWindowBattlePreview>(EUIWindowKey.BattlePreview).Show(Global.Instance.CurrentMission.PlanetKey, Global.Instance.CurrentMission.MissionKey);
	}
	#endregion

	#region auxiliary
	private string GetUnitIconResourcePath(string iconName) {
		return string.Format("{0}/{1}", GameConstants.Paths.UI_UNIT_ICONS_RESOURCES, iconName);
	}
	#endregion
}
