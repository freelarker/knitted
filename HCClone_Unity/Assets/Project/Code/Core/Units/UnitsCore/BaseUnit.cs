using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public abstract class BaseUnit {
	protected BaseUnitData _data;
	public BaseUnitData Data {
		get { return _data; }
	}

	public int Health { get; set; }	//health amount after all upgrades applied
	public int Damage { get; set; }	//damage amount after all upgrades applied
	public float DamageRange { get; set; }	//damage range after all upgrades applied
	public float DamageSpeed { get; set; }	//damage speed after all upgrades applied

	public int AggroCrystalsForDeathToEnemy { get; private set; }	//amount of aggro crystals unit gives to enemies after death
	public int AggroCrystalsForDeathToAlly { get; private set; }	//amount of aggro crystals unit gives to allies after death

	public UnitInventory Inventory { get; private set; }

	public BaseUnit(BaseUnitData data) {
		_data = data;
		Inventory = new UnitInventory(CreateSlotsData(), OnEquipmentUpdate);
	}

	protected virtual Dictionary<EUnitEqupmentSlot, EItemType[]> CreateSlotsData() {
		Dictionary<EUnitEqupmentSlot, EItemType[]> slotsData = new Dictionary<EUnitEqupmentSlot, EItemType[]>();
		ArrayRO<EUnitEqupmentSlot> availableSlots = UnitsData.Instance.GetUnitEquipmentSlots(this);
		for (int i = 0; i < availableSlots.Length; i++) {
			slotsData.Add(availableSlots[i], new EItemType[0]);
		}
		return slotsData;
	}

	protected virtual void RecalculateParams() {
		Health = _data.BaseHealth;
		Damage = _data.BaseDamage;
		DamageRange = _data.BaseDamageRange;
		DamageSpeed = _data.BaseDamageSpeed;

		ArrayRO<EUnitEqupmentSlot> equipmentSlots = UnitsData.Instance.GetUnitEquipmentSlots(this);
		BaseItem itemData = null;
		for (int i = 0; i < equipmentSlots.Length; i++) {
			itemData = ItemsData.Instance.GetItem(Inventory.GetItemInSlot(i));
			if (itemData != null) {
				Health += itemData.ModHealth;
				Damage += itemData.ModDamage;
				DamageRange += itemData.ModDamageRange;
				DamageSpeed += itemData.ModDamageSpeed;
			}
		}

		//TODO: broadcast message
	}

	protected virtual void OnEquipmentUpdate(EUnitEqupmentSlot slot, EItemKey oldItem, EItemKey newItem) {
		RecalculateParams();

		//TODO: broadcast message
	}
}
