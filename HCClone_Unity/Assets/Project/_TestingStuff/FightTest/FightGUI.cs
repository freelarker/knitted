using UnityEngine;
using UnityEngine.UI;

public class FightGUI : MonoBehaviour {
	[SerializeField]
	private Button _btnPause;

	public void Start() {
		_btnPause.onClick.AddListener(TogglePause);
		UpdateText();
	}

	private void TogglePause() {
		FightManager.Instance.TogglePause();
		UpdateText();
	}

	private void UpdateText() {
		_btnPause.GetComponentInChildren<Text>().text = FightManager.Instance.IsPaused ? "Resume" : "Pause";
	}
}
