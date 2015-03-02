using UnityEngine;
using UnityEngine.UI;

public class UICity : MonoBehaviour {
	[SerializeField]
	private Button _btnTownHall;
	[SerializeField]
	private Button _btnFort;
	[SerializeField]
	private Button _btnBarracks;
	[SerializeField]
	private Button _btnMarket;
	[SerializeField]
	private Button _btnWarehouse;
	[SerializeField]
	private Button _btnHeroesHall;

	[SerializeField]
	private Button _btnBMMissions;
	[SerializeField]
	private Button _btnBMCrusade;
	[SerializeField]
	private Button _btnBMRaids;

	[SerializeField]
	private GameObject _goMenuRight;
	[SerializeField]
	private Button _btnRightMenuToggle;
	[SerializeField]
	private Button _btnRMHeroesHall;
	[SerializeField]
	private Button _btnRMBarracks;
	[SerializeField]
	private Button _btnRMInventory;
	[SerializeField]
	private Button _btnRMQuests;

	public void Start() {
		//TODO: UI scale

		_btnTownHall.onClick.AddListener(OnTownHallClick);
		_btnFort.onClick.AddListener(OnFortClick);
		_btnBarracks.onClick.AddListener(OnBarracksClick);
		_btnMarket.onClick.AddListener(OnMarketClick);
		_btnWarehouse.onClick.AddListener(OnWarehouseClick);
		_btnHeroesHall.onClick.AddListener(OnHeroesHallClick);

		_btnBMMissions.onClick.AddListener(OnMissionsClick);
		_btnBMCrusade.onClick.AddListener(OnCrusadeClick);
		_btnBMRaids.onClick.AddListener(OnRaidsClick);

		_btnRightMenuToggle.onClick.AddListener(ToggleRightMenu);
		_btnRMHeroesHall.onClick.AddListener(OnHeroesHallClick);
		_btnRMBarracks.onClick.AddListener(OnBarracksClick);
		_btnRMInventory.onClick.AddListener(OnInventoryClick);
		_btnRMQuests.onClick.AddListener(OnQuestsClick);
	}

	#region button listeners
	//buildings
	private void OnTownHallClick() {
		//TODO: show town hall window
	}

	private void OnFortClick() {
		//TODO: show fort window
	}

	private void OnBarracksClick() {
		//TODO: show barracks window
	}

	private void OnMarketClick() {
		//TODO: show market window
	}

	private void OnWarehouseClick() {
		//TODO: show warehouse window
	}

	private void OnHeroesHallClick() {
		//TODO: show heroes hall window
	}

	//missions
	private void OnMissionsClick() {
		//TODO: load last planet's scene
		Application.LoadLevel("Planet1");
	}

	private void OnCrusadeClick() {
		//TODO: show crusade
	}

	private void OnRaidsClick() {
		//TODO: show raids
	}

	//other
	private void ToggleRightMenu() {
		_goMenuRight.SetActive(!_goMenuRight.activeInHierarchy);
	}

	private void OnInventoryClick() {
		//TODO: show player inventory
	}

	private void OnQuestsClick() {
		//TODO: show quests
	}
	#endregion
}
