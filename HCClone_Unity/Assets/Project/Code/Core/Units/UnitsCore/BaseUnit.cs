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
	public float AttackRange { get; set; }	//damage range after all upgrades applied
	public float AttackSpeed { get; set; }	//damage speed after all upgrades applied

	public UnitInventory Inventory { get; private set; }

	public uint DamageTaken { get; private set; }
	public bool IsDead { get { return DamageTaken >= Health; } }

	public BaseUnit(BaseUnitData data) {
		_data = data;
		Inventory = new UnitInventory(CreateSlotsData(), OnEquipmentUpdate);

		RecalculateParams();
	}

	public void ApplyDamage(uint damageAmount) {
		if (IsDead) {
			return;
		}

		DamageTaken += damageAmount;
		
		if (IsDead) {
			//TODO: broadcast death
		} else {
			//TODO: broadcast hit
		}
	}

	public void ApplyHeal(uint healAmount, bool revive) {
		bool preHealDeadState = IsDead;

		if (IsDead && !revive) {
			return;
		}

		DamageTaken -= healAmount;
		
		if (preHealDeadState && !IsDead) {
			//TODO: broadcast revive
		} else {
			//TODO: broadcast heal
		}
	}

	public void ResetDamageTaken() {
		DamageTaken = 0;
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
		AttackRange = _data.BaseAttackRange;
		AttackSpeed = _data.BaseAttackSpeed;

		ArrayRO<EUnitEqupmentSlot> equipmentSlots = UnitsData.Instance.GetUnitEquipmentSlots(this);
		BaseItem itemData = null;
		for (int i = 0; i < equipmentSlots.Length; i++) {
			itemData = ItemsData.Instance.GetItem(Inventory.GetItemInSlot(i));
			if (itemData != null) {
				Health += itemData.ModHealth;
				Damage += itemData.ModDamage;
				AttackRange += itemData.ModDamageRange;
				AttackSpeed += itemData.ModDamageSpeed;
			}
		}

		//TODO: broadcast message
	}

	protected virtual void OnEquipmentUpdate(EUnitEqupmentSlot slot, EItemKey oldItem, EItemKey newItem) {
		RecalculateParams();

		//TODO: broadcast message
	}
}
