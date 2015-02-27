using UnityEngine;

[System.Serializable]
public class MissionMapData {
	[SerializeField]
	private EUnitKey[] _units = null;
	private ArrayRO<EUnitKey> _unitsRO = null;
	public ArrayRO<EUnitKey> Units {
		get {
			if (_unitsRO == null) {
				_unitsRO = new ArrayRO<EUnitKey>(_units);
				_units = null;
			}

			return _unitsRO;
		}
	}
}
