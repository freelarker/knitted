using UnityEngine;
using UnityEngine.UI;

public class UIWindowHeroInfo : UIWindow {
	[SerializeField]
	private Button _btnBack;

	public void Start() {
		_btnBack.onClick.AddListener(OnBtnBackClick);
	}

	#region listeners
	private void OnBtnBackClick() {
		Hide();
	}
	#endregion
}
