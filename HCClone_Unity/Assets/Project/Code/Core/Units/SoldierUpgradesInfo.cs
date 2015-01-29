using UnityEngine;

/// <summary>
/// Data that will be stored player upgrades progress
/// </summary>
public class SoldierUpgradesInfo {
	public EUnitKey UnitKey { get; private set; }
	public int WeaponLevel { get; set; }
	public int ArmorLevel { get; set; }

	public SoldierUpgradesInfo(EUnitKey unitKey, int weaponLevel, int armorLevel) {
		if (weaponLevel < 1 || weaponLevel > GameConstants.City.MAX_UNIT_UPGRADE_LEVEL) {
			Debug.LogError("Wrong unit weapon level: " + weaponLevel);
			weaponLevel = 1;
		}
		if (armorLevel < 1 || armorLevel > GameConstants.City.MAX_UNIT_UPGRADE_LEVEL) {
			Debug.LogError("Wrong unit armor level: " + armorLevel);
			armorLevel = 1;
		}

		UnitKey = unitKey;
		WeaponLevel = weaponLevel;
		ArmorLevel = armorLevel;
	}
}
