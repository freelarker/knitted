using UnityEngine;
using UnityEngine.UI;

public class UIMainMenu : MonoBehaviour {
	[SerializeField]
	private Button _btnCampaign;
	[SerializeField]
	private Button _btnArena;
	[SerializeField]
	private Button _btnSets;
	[SerializeField]
	private Button _btnShop;
	[SerializeField]
	private Button _btnHeroes;
	[SerializeField]
	private Button _btnSquad;

	public void Awake() {
		float widthRatio = 1f * Screen.width / 1048;
		float heightRatio = 1f * Screen.height / 540;

		gameObject.GetComponent<CanvasScaler>().scaleFactor = Mathf.Min(widthRatio, heightRatio);
	}

	public void Start() {
		_btnCampaign.onClick.AddListener(OnCampaignClick);
	}

	private void OnCampaignClick() {
		//TODO: load last planet's scene
		Application.LoadLevel("Planet1");
	}
}
