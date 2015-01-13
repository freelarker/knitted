﻿using UnityEngine;

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
	protected int _baseDamage = 0;	//base damage amount (without upgrades)
	public int BaseDamage {
		get { return _baseDamage; }
	}

	[SerializeField]
	protected float _baseDamageRange = 1;	//base damage range (without upgrades)
	public float BaseDamageRange {
		get { return _baseDamageRange; }
	}

	[SerializeField]
	protected float _baseDamageSpeed = 1;	//base damage speed (without upgrades)
	public float BaseDamageSpeed {
		get { return _baseDamageSpeed; }
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
}