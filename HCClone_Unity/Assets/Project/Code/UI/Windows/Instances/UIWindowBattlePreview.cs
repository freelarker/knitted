using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class UIWindowBattlePreview : UIWindow {
	[SerializeField]
	private Text _txtTitleCaption;

	[SerializeField]
	private Text _txtAttemptsCaption;
	[SerializeField]
	private Text _txtAttemptsAmount;

	[SerializeField]
	private Text _txtFuelCaption;
	[SerializeField]
	private Text _txtFuelAmount;

	[SerializeField]
	private Text _txtEnemiesCaption;
	[SerializeField]
	private Text _txtLootCaption;

	[SerializeField]
	private Image _imgEnemy;
	[SerializeField]
	private float _offsetImageEnemy = 20f;

	[SerializeField]
	private Image _imgLoot;
	[SerializeField]
	private float _offsetImageLoot = 20f;

	[SerializeField]
	private Button _btnBack;
	[SerializeField]
	private Button _btnPlay;

	private EPlanetKey _planetKey = EPlanetKey.None;
	private EMissionKey _missionKey = EMissionKey.None;

	private EUnitKey[] _enemies = null;
	private Image[] _enemyImages = null;

	private EItemKey[] _lootItems = null;
	private Image[] _lootItemImages = null;

	public void Awake() {
		_onPostHide += OnWindowHide;
	}

	public void Start() {
		_btnBack.onClick.AddListener(OnBtnBackClick);
		_btnPlay.onClick.AddListener(OnBtnPlayClick);
	}

	public void Show(EPlanetKey planetKey, EMissionKey missionKey) {
		Setup(planetKey, missionKey);
		Show();
	}

	#region setup
	public void Setup(EPlanetKey planetKey, EMissionKey missionKey) {
		_planetKey = planetKey;
		_missionKey = missionKey;

		Global.Instance.CurrentMission.PlanetKey = _planetKey;
		Global.Instance.CurrentMission.MissionKey = _missionKey;

		MissionData md = MissionsConfig.Instance.GetPlanet(planetKey).GetMission(missionKey);
		if (md != null) {
			//TODO: setup title
			SetupAttempts(md);
			SetupFuel(md);
			SetupEnemies(md);
			SetupLoot(md);

		}
	}

	private void SetupAttempts(MissionData md) {
		int attemptsUsed = Global.Instance.Player.StoryProgress.GetMissionAttemptsUsed(_planetKey, _missionKey);
		_txtAttemptsAmount.text = string.Format("{0}/{1}", md.AttemptsDaily - attemptsUsed, md.AttemptsDaily);
		if (attemptsUsed >= md.AttemptsDaily) {
			_btnPlay.interactable = false;
		}
	}

	private void SetupFuel(MissionData md) {
		_txtFuelAmount.text = md.FuelWinCost.ToString();
		if (Global.Instance.Player.Resources.Fuel < md.FuelWinCost) {
			_btnPlay.interactable = false;
			_txtFuelAmount.color = Color.red;
		} else {
			_txtFuelAmount.color = Color.white;
		}
	}

	private void SetupEnemies(MissionData md) {
		float enemyImageWidth = _imgEnemy.transform.GetChild(0).GetComponent<RectTransform>().rect.width;

		List<EUnitKey> unitKeys = new List<EUnitKey>();
		MissionMapData mmd = null;
		for (int i = 0; i < md.MapsCount; i++) {
			mmd = md.GetMap(i);
			for (int j = 0; j < mmd.Units.Length; j++) {
				if (unitKeys.IndexOf(mmd.Units[j]) == -1) {
					unitKeys.Add(mmd.Units[j]);
				}
			}
		}
		_enemies = unitKeys.ToArray();

		_enemyImages = new Image[_enemies.Length];
		_enemyImages[0] = _imgEnemy;
		for (int i = 0; i < _enemies.Length; i++) {
			if (i > 0) {
				_enemyImages[i] = (GameObject.Instantiate(_imgEnemy.gameObject) as GameObject).GetComponent<Image>();
				_enemyImages[i].transform.SetParent(_imgEnemy.transform.parent, false);
				_enemyImages[i].rectTransform.anchoredPosition = _imgEnemy.rectTransform.anchoredPosition + new Vector2(i * (enemyImageWidth + _offsetImageEnemy), 0f);
			}

			Image enemyIcon = _enemyImages[i];
			Sprite enemyIconResource = UIResourcesManager.Instance.GetResource<Sprite>(GetUnitIconResourcePath(_enemies[i]));
			if (enemyIconResource != null) {
				enemyIcon.sprite = enemyIconResource;
			}
		}
	}

	private void SetupLoot(MissionData md) {
		float lootImageWidth = _imgLoot.transform.GetChild(0).GetComponent<RectTransform>().rect.width;

		ArrayRO<ItemDropChance> loot = MissionsConfig.Instance.GetPlanet(_planetKey).GetMission(_missionKey).RewardItems;
		if (loot.Length == 0) {
			_imgLoot.gameObject.SetActive(false);
		} else {
			_lootItems = new EItemKey[loot.Length];
			_lootItemImages = new Image[loot.Length];
			_lootItemImages[0] = _imgLoot;

			for (int i = 0; i < loot.Length; i++) {
				_lootItems[i] = loot[i].ItemKey;

				if (i > 0) {
					_lootItemImages[i] = (GameObject.Instantiate(_imgLoot.gameObject) as GameObject).GetComponent<Image>();
					_lootItemImages[i].transform.SetParent(_imgLoot.transform.parent, false);
					_lootItemImages[i].rectTransform.anchoredPosition = _imgLoot.rectTransform.anchoredPosition + new Vector2(i * (lootImageWidth + _offsetImageLoot), 0f);
				}

				Image lootIcon = _lootItemImages[i];
				Sprite lootIconResource = UIResourcesManager.Instance.GetResource<Sprite>(GetLootIconResourcePath(loot[i].ItemKey));
				if (lootIconResource != null) {
					lootIcon.sprite = lootIconResource;
				}
			}
		}
	}
	#endregion

	#region listeners
	private void OnBtnPlayClick() {
		//TODO: do not hide, just show new window using UIWindowsManager
		Hide();

		UIWindowsManager.Instance.GetWindow(EUIWindowKey.BattleSetup).Show();
	}

	private void OnBtnBackClick() {
		Global.Instance.CurrentMission.PlanetKey = _planetKey;
		Global.Instance.CurrentMission.MissionKey = _missionKey;

		Hide();
	}

	private void OnWindowHide() {
		_planetKey = EPlanetKey.None;
		_missionKey = EMissionKey.None;

		//clear enemies
		if (_enemies != null) {
			for (int i = 0; i < _enemies.Length; i++) {
				_enemyImages[i].sprite = null;
				if (i > 0) {
					GameObject.Destroy(_enemyImages[i].gameObject);
				}
				UIResourcesManager.Instance.FreeResource(GetUnitIconResourcePath(_enemies[i]));
			}
		}
		_enemies = null;
		_enemyImages = null;

		//clear loot
		if (_lootItems != null) {
			for (int i = 0; i < _lootItems.Length; i++) {
				_lootItemImages[i].sprite = null;
				if (i > 0) {
					GameObject.Destroy(_lootItemImages[i].gameObject);
				}
				UIResourcesManager.Instance.FreeResource(GetLootIconResourcePath(_lootItems[i]));
			}
		}
		_lootItems = null;
		_lootItemImages = null;
	}
	#endregion

	#region auxiliary
	private string GetUnitIconResourcePath(EUnitKey unitKey) {
		return string.Format("{0}/{1}", GameConstants.Paths.UI_UNIT_ICONS_RESOURCES, UnitsConfig.Instance.GetSoldierData(unitKey).IconName);
	}

	private string GetLootIconResourcePath(EItemKey itemKey) {
		return string.Format("{0}/{1}", GameConstants.Paths.UI_ITEM_ICONS_RESOURCES, ItemsConfig.Instance.GetItem(itemKey).IconName);
	}
	#endregion
}
