using UnityEngine;

[System.Serializable]
public class BaseItem {
	[SerializeField]
	protected EItemKey _key = EItemKey.None;		//item key
	public EItemKey Key {
		get { return _key; }
	}

	[SerializeField]
	protected EItemType _type = EItemType.Idle;	//item type
	public EItemType Type {
		get { return _type; }
	}

	[SerializeField]
	protected EUnitEqupmentSlot _slot = EUnitEqupmentSlot.Other;	//slot where item should be equipped
	public EUnitEqupmentSlot Slot {
		get { return _slot; }
	}	

	//modifiers
	[SerializeField]
	protected int _modHealth = 0;
	public int ModHealth {
		get { return _modHealth; }
	}

	[SerializeField]
	protected int _modDamage = 0;
	public int ModDamage {
		get { return _modDamage; }
	}

	[SerializeField]
	protected float _modDamageRange = 0;
	public float ModDamageRange {
		get { return _modDamageRange; }
	}

	[SerializeField]
	protected float _modDamageSpeed = 0;
	public float ModDamageSpeed {
		get { return _modDamageSpeed; }
	}

	[SerializeField]
	protected int _levelRequirement = 1;
	public float LevelRequirement {
		get { return _levelRequirement; }
	}

	public BaseItem() {	}

	public BaseItem(EItemKey key, EItemType type, EUnitEqupmentSlot slot) {
		_key = key;
		_type = type;
		_slot = slot;
	}
}
