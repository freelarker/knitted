﻿using UnityEngine;

public class UnitsData : MonoBehaviourResourceSingleton<UnitsData> {
#pragma warning disable 0414
	private static string _path = "Config/UnitsData";
#pragma warning restore 0414

	[SerializeField]
	private int _maxUnitsHeroCanHire = 4;
	public int MaxUnitsHeroCanHire {
		get { return _maxUnitsHeroCanHire; }
	}

	[SerializeField]
	private EUnitEqupmentSlot[] _heroEquipmentSlots = new EUnitEqupmentSlot[0];
	private ArrayRO<EUnitEqupmentSlot> _heroEquipmentSlotsRO = null;
	[SerializeField]
	private EUnitEqupmentSlot[] _soldierEquipmentSlots = new EUnitEqupmentSlot[0];
	private ArrayRO<EUnitEqupmentSlot> _soldierEquipmentSlotsRO = null;

	[SerializeField]
	private BaseHeroData[] _heroesData = new BaseHeroData[0];
	[SerializeField]
	private BaseSoldierData[] _soldiersData = new BaseSoldierData[0];

	public UnitsData() {
		_heroEquipmentSlotsRO = new ArrayRO<EUnitEqupmentSlot>(_heroEquipmentSlots);
		_heroEquipmentSlots = null;
		_soldierEquipmentSlotsRO = new ArrayRO<EUnitEqupmentSlot>(_soldierEquipmentSlots);
		_soldierEquipmentSlots = null;
	}

	public ArrayRO<EUnitEqupmentSlot> GetUnitEquipmentSlots(BaseUnit unit) {
		if (unit is BaseHero) {
			return _heroEquipmentSlotsRO;
		}
		return _soldierEquipmentSlotsRO;
	}

	public BaseHeroData GetHeroData(EUnitKey key) {
		for (int i = 0; i < _heroesData.Length; i++) {
			if (_heroesData[i].Key == key) {
				return _heroesData[i];
			}
		}
		return null;
	}

	public BaseSoldierData GetSoldierData(EUnitKey key) {
		for (int i = 0; i < _soldiersData.Length; i++) {
			if (_soldiersData[i].Key == key) {
				return _soldiersData[i];
			}
		}
		return null;
	}
}