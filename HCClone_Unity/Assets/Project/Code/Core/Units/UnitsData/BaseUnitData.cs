using UnityEngine;

[System.Serializable]
public abstract class BaseUnitData {
	[SerializeField]
	private EUnitKey _key = EUnitKey.Idle;	//unit key
	public EUnitKey Key {
		get { return _key; }
	}

	[SerializeField]
	protected int _baseHealth = 0;	//base health amount (without upgrades)
	public int BaseHealth {
		get { return _baseHealth; }
	}

	[SerializeField]
	protected int _baseArmor = 0;	//base armor amount (without upgrades)
	public int BaseArmor {
		get { return _baseArmor; }
	}

	[SerializeField]
	protected int _baseDamage = 0;	//base damage amount (without upgrades)
	public int BaseDamage {
		get { return _baseDamage; }
	}

	[SerializeField]
	protected float _baseAttackRange = 1;	//base attack range (without upgrades)
	public float BaseAttackRange {
		get { return _baseAttackRange; }
	}

	[SerializeField]
	protected float _baseAttackSpeed = 1;	//base attack speed (without upgrades)
	public float BaseAttackSpeed {
		get { return _baseAttackSpeed; }
	}

	[SerializeField]
	protected int _baseCritChance = 0;	//base critical hit chance (without upgrades)
	public int BaseCritChance {
		get { return _baseCritChance; }
	}

	[SerializeField]
	protected float _aggroCrystalsForDeathToEnemy = 0;	//amount of aggro crystals unit gives to enemies after death
	public float AggroCrystalsForDeathToEnemy {
		get { return _aggroCrystalsForDeathToEnemy; }
	}

	[SerializeField]
	protected float _aggroCrystalsForDeathToAlly = 0;	//amount of aggro crystals unit gives to allies after death
	public float AggroCrystalsForDeathToAlly {
		get { return _aggroCrystalsForDeathToAlly; }
	}

	[SerializeField]
	protected string _prefabName = string.Empty;
	public string PrefabName {
		get { return _prefabName; }
	}
}
