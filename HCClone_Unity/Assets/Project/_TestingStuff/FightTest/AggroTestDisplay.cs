using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class AggroTestDisplay : MonoBehaviour {
	private Text _lblAggro;

	public void Start() {
		_lblAggro = gameObject.GetComponent<Text>();

		EventsAggregator.Units.AddListener<int>(EUnitEvent.AggroCrystalsUpdate, UpdateAggro);
	}

	public void OnDestroy() {
		EventsAggregator.Units.RemoveListener<int>(EUnitEvent.AggroCrystalsUpdate, UpdateAggro);
	}

	private void UpdateAggro(int aggroValue) {
		_lblAggro.text = string.Format("Aggro: {0}", aggroValue);
	}
}
