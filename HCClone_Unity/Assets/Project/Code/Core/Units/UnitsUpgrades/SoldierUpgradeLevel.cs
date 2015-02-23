using UnityEngine;

[System.Serializable]
public class SoldierUpgradeLevel {
	[SerializeField]
	private int _level = 0;
	public int Level {
		get { return _level; }
	}

	[SerializeField]
	private EItemKey _weaponKey = EItemKey.None;
	public EItemKey WeaponKey {
		get { return _weaponKey; }
	}

	[SerializeField]
	private EItemKey _armorKey = EItemKey.None;
	public EItemKey ArmorKey {
		get { return _armorKey; }
	}
}
