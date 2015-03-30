using UnityEngine;
using UnityEngine.UI;

public class UIHeroAggro : MonoBehaviour {
	[SerializeField]
	private Image _imgAggroMeter;
	[SerializeField]
	private Text _lblAggroValue;

	public void Start() {
		UpdateAggro(Global.Instance.Player.Heroes.Current.AggroCrystals, Global.Instance.Player.Heroes.Current.AggroCrystalsMaximum);
		EventsAggregator.Units.AddListener<int, int>(EUnitEvent.AggroCrystalsUpdate, UpdateAggro);
	}

	public void OnDestroy() {
		EventsAggregator.Units.RemoveListener<int, int>(EUnitEvent.AggroCrystalsUpdate, UpdateAggro);
	}

	private void UpdateAggro(int aggroValue, int aggroMax) {
		_imgAggroMeter.fillAmount = aggroValue * 1f / aggroMax;
		_lblAggroValue.text = aggroValue.ToString();
	}
}
