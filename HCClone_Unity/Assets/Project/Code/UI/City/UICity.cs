using UnityEngine;
using UnityEngine.UI;

public class UICity : MonoBehaviour {
	#region local scene instance
	private static UICity _sceneInstance = null;
	public static UICity SceneInstance {
		get { return _sceneInstance; }
	}
	#endregion

	[SerializeField]
	private UICityBuildingIcon _cbiTownHall;
	[SerializeField]
	private UICityBuildingIcon _cbiFort;
	[SerializeField]
	private UICityBuildingIcon _cbiBarracks;
	[SerializeField]
	private UICityBuildingIcon _cbiMarket;
	[SerializeField]
	private UICityBuildingIcon _cbiWarehouse;
	[SerializeField]
	private UICityBuildingIcon _cbiHeroesHall;

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

	[SerializeField]
	private UICityBuildingPopup _buildingPopup;
	public UICityBuildingPopup BuildingPopup {
		get { return _buildingPopup; }
	}

	public void Awake() {
		_sceneInstance = this;

		if (!Global.IsInitialized) {
			Global.Instance.Initialize();
		}
	}

	public void Start() {
		//TODO: UI scale

		_btnBMMissions.onClick.AddListener(OnMissionsClick);
		_btnBMCrusade.onClick.AddListener(OnCrusadeClick);
		_btnBMRaids.onClick.AddListener(OnRaidsClick);

		_btnRightMenuToggle.onClick.AddListener(ToggleRightMenu);
		_btnRMHeroesHall.onClick.AddListener(OnRMHeroesHallClick);
		_btnRMBarracks.onClick.AddListener(OnRMBarracksClick);
		_btnRMInventory.onClick.AddListener(OnInventoryClick);
		_btnRMQuests.onClick.AddListener(OnQuestsClick);

		_buildingPopup.Hide();
	}

	public void OnDestroy() {
		_sceneInstance = null;
	}

	#region button listeners
	//bottom menu
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

	//right menu
	private void ToggleRightMenu() {
		_goMenuRight.SetActive(!_goMenuRight.activeInHierarchy);
	}

	private void OnInventoryClick() {
		//TODO: show player inventory
	}

	private void OnQuestsClick() {
		//TODO: show quests
	}

	private void OnRMHeroesHallClick() {
		//TODO: show hh window
	}

	private void OnRMBarracksClick() {
		//TODO: show barracks window
	}
	#endregion
}
