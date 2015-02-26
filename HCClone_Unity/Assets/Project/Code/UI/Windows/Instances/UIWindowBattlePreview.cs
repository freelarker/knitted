using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class UIWindowBattlePreview : UIWindow {
	[SerializeField]
	private Text _txtTitleCaption;

	[SerializeField]
	private Text _txtChancesCaption;
	[SerializeField]
	private Text _txtChancesAmount;

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
	private float _imagesOffset = 20f;

	[SerializeField]
	private Button _btnBack;
	[SerializeField]
	private Button _btnPlay;

	private EPlanetKey _planetKey = EPlanetKey.None;
	private EMissionKey _missionKey = EMissionKey.None;
	private EUnitKey[] _enemies = null;
	private Image[] _enemyImages = null;

	public void Awake() {
		_onPostHide += OnWindowHide;
	}

	public void Start() {
		_btnBack.onClick.AddListener(Hide);
		_btnPlay.onClick.AddListener(OnPlayClick);
	}

	public void Show(EPlanetKey planetKey, EMissionKey missionKey) {
		Setup(planetKey, missionKey);
		Show();
	}

	public void Setup(EPlanetKey planetKey, EMissionKey missionKey) {
		_planetKey = planetKey;
		_missionKey = missionKey;

		MissionData md = MissionsConfig.Instance.GetPlanet(planetKey).GetMission(missionKey);
		if (md != null) {
			//TODO: setup attempts
			//TODO: setup fuel

			//setup enemies
			List<EUnitKey> unitKeys = new List<EUnitKey>();
			MissionMapData mmd = null;
			for (int i = 0; i < md.MapsCount; i++) {
				mmd = md.GetMap(i);
				for (int j = 0; j < mmd.Units.Length; j++) {
					if(unitKeys.IndexOf(mmd.Units[j]) == -1) {
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
					_enemyImages[i].rectTransform.anchoredPosition = _imgEnemy.rectTransform.anchoredPosition + new Vector2(i * (_imgEnemy.rectTransform.rect.width + _imagesOffset), 0f);
				}

				Image enemyIcon = _enemyImages[i].transform.GetChild(0).gameObject.GetComponent<Image>();
				Sprite enemyIconResource = UIResourcesManager.Instance.GetResource<Sprite>(GetUintIconResourcePath(_enemies[i]));
				if (enemyIconResource != null) {
					enemyIcon.sprite = enemyIconResource;
					enemyIcon.SetNativeSize();
				}
			}

			//TODO: setup loot
		}
	}

	private void OnPlayClick() {
		Global.Instance.CurrentMission.PlanetKey = _planetKey;
		Global.Instance.CurrentMission.MissionKey = _missionKey;

		Hide();

		//TODO: show
	}

	private void OnWindowHide() {
		_planetKey = EPlanetKey.None;
		_missionKey = EMissionKey.None;

		//clear enemies
		if (_enemies != null) {
			for (int i = 0; i < _enemies.Length; i++) {
				UIResourcesManager.Instance.FreeResource(GetUintIconResourcePath(_enemies[i]));
				if (i > 0) {
					GameObject.Destroy(_enemyImages[i].gameObject);
				}
			}
		}

		_enemies = null;
		_enemyImages = null;
	}

	private string GetUintIconResourcePath(EUnitKey unitKey) {
		return string.Format("{0}/{1}", GameConstants.Paths.UI_UNIT_ICONS_RESOURCES, UnitsConfig.Instance.GetSoldierData(unitKey).IconName);
	}
}
