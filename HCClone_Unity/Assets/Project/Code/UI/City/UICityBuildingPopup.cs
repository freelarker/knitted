﻿using UnityEngine;
using UnityEngine.UI;

public class UICityBuildingPopup : MonoBehaviour {
	[SerializeField]
	private Text _lblUpgrade;
	[SerializeField]
	private Text _lblEnter;
	[SerializeField]
	private Text _lblBuildingName;

	[SerializeField]
	private Button _btnUpgrade;
	[SerializeField]
	private Button _btnEnter;
	[SerializeField]
	private Button _btnBackground;

	private ECityBuildingKey _buildingKey;

	public void Start() {
		_btnUpgrade.onClick.AddListener(OnBtnUpgradeClick);
		_btnEnter.onClick.AddListener(OnBtnEnterClick);

		_btnBackground.onClick.AddListener(Hide);
	}

	public void Show(ECityBuildingKey buildingKey) {
		_buildingKey = buildingKey;

		//TODO: setup labels
		SetupLabel();

		gameObject.SetActive(true);
	}

	public void Hide() {
		gameObject.SetActive(false);
	}

	private void OnBtnUpgradeClick() {
		//TODO: show window
	}

	private void OnBtnEnterClick() {
		//TODO: show window
	}

	//WARNING! temp!
	private void SetupLabel() {
		string strBuildingName = string.Empty;
		string strEnterCaption = "Enter";
		
		switch (_buildingKey) {
			case ECityBuildingKey.TownHall:
				strBuildingName = "Main";
				strEnterCaption = "Info";
				break;
			case ECityBuildingKey.Barracks:
				strBuildingName = "Barracks";
				break;
			case ECityBuildingKey.Fort:
				strBuildingName = "Fort";
				break;
			case ECityBuildingKey.HeroesHall:
				strBuildingName = "Heroes Hall";
				break;
			case ECityBuildingKey.Market:
				strBuildingName = "Merchant";
				break;
			case ECityBuildingKey.Warehouse:
				strBuildingName = "Warehouse";
				break;
		}

		CityBuildingInfo buildingInfo = Global.Instance.Player.City.GetBuilding(_buildingKey);
		int buildingLevel = buildingInfo != null ? buildingInfo.Level : 1;

		_lblBuildingName.text = string.Format("{0} {1} lvl", strBuildingName, buildingLevel);
		_lblEnter.text = strEnterCaption;
	}
}