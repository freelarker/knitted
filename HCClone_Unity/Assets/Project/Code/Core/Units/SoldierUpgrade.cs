using UnityEngine;

[System.Serializable]
public class SoldierUpgrade {
	[SerializeField]
	private EUnitKey _soldierKey = EUnitKey.Idle;
	public EUnitKey SoldierKey {
		get { return _soldierKey; }
	}

	[SerializeField]
	private SoldierUpgradeLevel[] _levelsData = null;
	private ArrayRO<SoldierUpgradeLevel> _levelsDataRO = null;
	public ArrayRO<SoldierUpgradeLevel> LevelsData {
		get {
			if (_levelsDataRO == null) {
				_levelsDataRO = new ArrayRO<SoldierUpgradeLevel>(_levelsData);
			}
			return _levelsDataRO;
		}
	}

	public SoldierUpgradeLevel GetUpgradeLevel(int level) {
		if (_levelsData != null) {
			for (int i = 0; i < _levelsData.Length; i++) {
				if (_levelsData[i].Level == level) {
					return _levelsData[i];
				}
			}
		}
		return null;
	}
}
