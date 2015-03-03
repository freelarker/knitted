using UnityEngine;
using UnityEngine.UI;

public class UIFight : MonoBehaviour {
	[SerializeField]
	private Canvas _canvasUI;
	public Canvas CanvasUI {
		get { return _canvasUI; }
	}
	[SerializeField]
	private Canvas _canvasBG;
	public Canvas CanvasBG {
		get { return _canvasBG; }
	}

	[SerializeField]
	private Button _btnPause;
	[SerializeField]
	private Button _btnWithdraw;

	[SerializeField]
	private Button _btnAbility1;
	[SerializeField]
	private Button _btnAbility2;
	[SerializeField]
	private Button _btnAbility3;
	[SerializeField]
	private Button _btnAbility4;
	[SerializeField]
	private Button _btnAbility5;

	public void Start() {
		_btnPause.onClick.AddListener(FightManager.Instance.TogglePause);
		_btnWithdraw.onClick.AddListener(FightManager.Instance.Withdraw);
	}
}
