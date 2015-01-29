using UnityEngine;

[System.Serializable]
public class SoldierUpgradeLevel {
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
