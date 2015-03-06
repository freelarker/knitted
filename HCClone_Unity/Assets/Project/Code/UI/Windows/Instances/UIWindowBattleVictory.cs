﻿using UnityEngine;
using UnityEngine.UI;

public class UIWindowBattleVictory : UIWindow {
	[SerializeField]
	private Text _lblCreditsAmount;
	[SerializeField]
	private Text _lblExpAmount;

	[SerializeField]
	private Image _imgHero;

	[SerializeField]
	private Image _imgLoot;
	[SerializeField]
	private float _offsetImageLoot = 20f;

	[SerializeField]
	private Button _btnPlay;
	[SerializeField]
	private Button _btnReplay;
	
	private EItemKey[] _lootItems = null;
	private Image[] _lootItemImages = null;

	public void Awake() {
		_onPostHide += OnWindowHide;

		_btnPlay.onClick.AddListener(OnBtnPlayClick);
		_btnReplay.onClick.AddListener(OnBtnReplayClick);
	}

	public void Show(EPlanetKey planetKey, EMissionKey missionKey) {
		Setup(planetKey, missionKey);
		Show();
	}

	#region setup
	public void Setup(EPlanetKey planetKey, EMissionKey missionKey) {
		MissionData md = MissionsConfig.Instance.GetPlanet(planetKey).GetMission(missionKey);
		if (md != null) {
			_lblCreditsAmount.text = string.Format("+ {0}", md.RewardCredits);
			_lblExpAmount.text = string.Format("+ {0}", md.RewardExperienceWin);

			Sprite heroIconResource = UIResourcesManager.Instance.GetResource<Sprite>(GameConstants.Paths.GetUnitIconResourcePath(Global.Instance.Player.Heroes.Current.Data.IconName));
			if (heroIconResource != null) {
				_imgHero.sprite = heroIconResource;
			}

			SetupLoot(MissionsConfig.Instance.GetPlanet(planetKey).GetMission(missionKey).RewardItems);
		}
	}

	private void SetupLoot(ArrayRO<ItemDropChance> loot) {
		float lootImageWidth = _imgLoot.transform.GetChild(0).GetComponent<RectTransform>().rect.width;

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
				Sprite lootIconResource = UIResourcesManager.Instance.GetResource<Sprite>(GameConstants.Paths.GetLootIconResourcePath(loot[i].ItemKey));
				if (lootIconResource != null) {
					lootIcon.sprite = lootIconResource;
				}
			}
		}
	}
	#endregion

	#region listeners
	private void OnBtnPlayClick() {
		//TODO: load correct planet scene
		FightManager.Instance.Clear();
		Application.LoadLevel("Planet1");
	}

	private void OnBtnReplayClick() {
		//TODO: restart fight
	}

	private void OnWindowHide() {
		//clear loot
		if (_lootItems != null) {
			for (int i = 0; i < _lootItems.Length; i++) {
				_lootItemImages[i].sprite = null;
				if (i > 0) {
					GameObject.Destroy(_lootItemImages[i].gameObject);
				}
				UIResourcesManager.Instance.FreeResource(GameConstants.Paths.GetLootIconResourcePath(_lootItems[i]));
			}
		}
		_lootItems = null;
		_lootItemImages = null;

		//clear labels
		_lblCreditsAmount.text = "+ 0";
		_lblExpAmount.text = "+ 0";

		//hero
		if (_imgHero.sprite != null) {
			_imgHero.sprite = null;
			UIResourcesManager.Instance.FreeResource(GameConstants.Paths.GetUnitIconResourcePath(Global.Instance.Player.Heroes.Current.Data.IconName));
		}
	}
	#endregion
}
