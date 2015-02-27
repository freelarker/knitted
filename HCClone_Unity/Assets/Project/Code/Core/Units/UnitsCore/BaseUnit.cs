using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public abstract class BaseUnit {
	protected BaseUnitData _data;
	public BaseUnitData Data {
		get { return _data; }
	}

	public int Health { get; private set; }	//health amount after all upgrades applied
	public int Armor { get; private set; }	//armor amount after all upgrades applied
	public int ArmorDamageAbsorb { get; private set; }	//how much damage will be absorbed by armor all upgrades applied
	
	public int Damage { get; private set; }	//damage amount after all upgrades applied
	public float AttackRange { get; private set; }	//damage range after all upgrades applied
	public float AttackSpeed { get; private set; }	//damage speed after all upgrades applied
	public int CritChance { get; private set; }	//critical hit chance after all upgrades applied
	public float CritDamageMultiplier { get; private set; }	//critical hit damage multiplier after all upgrades applied

	public UnitInventory Inventory { get; private set; }

	public int DamageTaken { get; private set; }
	public bool IsDead { get { return DamageTaken >= Health; } }

	public BaseUnit(BaseUnitData data) {
		_data = data;
		Inventory = new UnitInventory(CreateSlotsData(), _data.BaseEquipment, OnEquipmentUpdate);

		RecalculateParamsInternal();
	}

	public AttackInfo GetAttackInfo() {
		bool isCrit = Random.Range(0, 100) < CritChance;
		return new AttackInfo(isCrit ? (int)(Damage * CritDamageMultiplier) : Damage, isCrit);
	}

	public void ApplyDamage(AttackInfo attackInfo) {
		attackInfo.DamageAmount -= ArmorDamageAbsorb;

		if (IsDead || attackInfo.DamageAmount <= 0) {
			return;
		}

		DamageTaken += attackInfo.DamageAmount;

		//broadcast hit
		EventsAggregator.Units.Broadcast<BaseUnit, HitInfo>(EUnitEvent.HitReceived, this, new HitInfo(Health - DamageTaken + attackInfo.DamageAmount, Health - DamageTaken, attackInfo.IsCritical));

		if (IsDead) {
			//broadcast death
			EventsAggregator.Units.Broadcast<BaseUnit>(EUnitEvent.DeathCame, this);
		}
	}

	public void ApplyHeal(AttackInfo attackInfo, bool revive) {
		if (attackInfo.DamageAmount <= 0) {
			return;
		}

		bool preHealDeadState = IsDead;

		if (IsDead && !revive) {
			return;
		}

		DamageTaken -= attackInfo.DamageAmount;
		
		if (preHealDeadState && !IsDead) {
			//broadcast revive
			EventsAggregator.Units.Broadcast<BaseUnit, HitInfo>(EUnitEvent.ReviveCame, this, new HitInfo(Health - DamageTaken - attackInfo.DamageAmount, Health - DamageTaken, attackInfo.IsCritical));
		} else {
			//broadcast heal
			EventsAggregator.Units.Broadcast<BaseUnit, HitInfo>(EUnitEvent.HitReceived, this, new HitInfo(Health - DamageTaken - attackInfo.DamageAmount, Health - DamageTaken, attackInfo.IsCritical));
		}
	}

	public void ResetDamageTaken() {
		DamageTaken = 0;
	}

	protected virtual Dictionary<EUnitEqupmentSlot, EItemType[]> CreateSlotsData() {
		Dictionary<EUnitEqupmentSlot, EItemType[]> slotsData = new Dictionary<EUnitEqupmentSlot, EItemType[]>();
		ArrayRO<EUnitEqupmentSlot> availableSlots = UnitsConfig.Instance.GetUnitEquipmentSlots(this);
		for (int i = 0; i < availableSlots.Length; i++) {
			slotsData.Add(availableSlots[i], new EItemType[0]);
		}
		return slotsData;
	}

	protected virtual void RecalculateParamsInternal() {
		Health = _data.BaseHealth;
		Armor = _data.BaseArmor;
		Damage = _data.BaseDamage;
		AttackRange = _data.BaseAttackRange;
		AttackSpeed = _data.BaseAttackSpeed;
		CritChance = _data.BaseCritChance;
		CritDamageMultiplier = _data.BaseCritDamageMultiplier;

		ArrayRO<EUnitEqupmentSlot> equipmentSlots = UnitsConfig.Instance.GetUnitEquipmentSlots(this);
		BaseItem itemData = null;
		for (int i = 0; i < equipmentSlots.Length; i++) {
			itemData = ItemsConfig.Instance.GetItem(Inventory.GetItemInSlot(i));
			if (itemData != null) {
				Health += itemData.ModHealth;
				Armor += itemData.ModArmor;
				Damage += itemData.ModDamage;
				AttackRange += itemData.ModDamageRange;
				AttackSpeed += itemData.ModDamageSpeed;
				CritChance += itemData.ModCritChance;
				CritDamageMultiplier = itemData.ModCritDamageMultiplier;
			}
		}
		ArmorDamageAbsorb = Mathf.CeilToInt(UnitsConfig.Instance.DamageReducePerOneArmor * Armor);
	}

	protected void RecalculateParams() {
		RecalculateParamsInternal();
		EventsAggregator.Units.Broadcast<BaseUnit>(EUnitEvent.RecalculateParams, this);
	}

	protected virtual void OnEquipmentUpdate(EUnitEqupmentSlot slot, EItemKey oldItemKey, EItemKey newItemKey) {
		RecalculateParams();

		EventsAggregator.Units.Broadcast<BaseUnit, EUnitEqupmentSlot, EItemKey, EItemKey>(EUnitEvent.EquipmentUpdate, this, slot, oldItemKey, newItemKey);
	}
}
