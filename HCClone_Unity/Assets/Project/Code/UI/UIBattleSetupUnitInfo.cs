using UnityEngine;
using UnityEngine.UI;

public class UIBattleSetupUnitInfo : MonoBehaviour {
	[SerializeField]
	private Button _button;
	public Button Button {
		get { return _button; }
	}

	[SerializeField]
	private Text _leadershipCost;
	public Text LeadershipCost {
		get { return _leadershipCost; }
	}

	//TODO: unit upgrades level

	public void Awake() {
		enabled = false;
	}
}
