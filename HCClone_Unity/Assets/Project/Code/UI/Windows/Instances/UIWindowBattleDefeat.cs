﻿using UnityEngine;
using UnityEngine.UI;

public class UIWindowBattleDefeat : UIWindow {
	[SerializeField]
	private Text _lblExpAmount;

	[SerializeField]
	private Image _imgHero;

	[SerializeField]
	private Button _btnPlay;
	[SerializeField]
	private Button _btnReplay;

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
			_lblExpAmount.text = string.Format("+ {0}", md.RewardExperienceWin);

			Sprite heroIconResource = UIResourcesManager.Instance.GetResource<Sprite>(GameConstants.Paths.GetUnitIconResourcePath(Global.Instance.Player.Heroes.Current.Data.IconName));
			if (heroIconResource != null) {
				_imgHero.sprite = heroIconResource;
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
		//clear labels
		_lblExpAmount.text = "+ 0";

		//hero
		if (_imgHero.sprite != null) {
			_imgHero.sprite = null;
			UIResourcesManager.Instance.FreeResource(GameConstants.Paths.GetUnitIconResourcePath(Global.Instance.Player.Heroes.Current.Data.IconName));
		}
	}
	#endregion
}
