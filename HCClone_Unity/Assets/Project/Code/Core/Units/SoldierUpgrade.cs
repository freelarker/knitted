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

	public EItemKey BaseWeaponKey {
		get { return WeaponKeyAtLevel(1); }
	}

	public EItemKey BaseArmorKey {
		get { return ArmorKeyAtLevel(1); }
	}

	public EItemKey WeaponKeyAtLevel(int level) {
		return (_levelsData != null && level > 0 && level <= _levelsData.Length) ? _levelsData[level - 1].WeaponKey : EItemKey.None;
	}

	public EItemKey ArmorKeyAtLevel(int level) {
		return (_levelsData != null && level > 0 && level <= _levelsData.Length) ? _levelsData[level - 1].ArmorKey : EItemKey.None;
	}
}
