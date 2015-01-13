using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class BaseHero : BaseUnit {
	protected new BaseHeroData _data;
	public new BaseHeroData Data {
		get { return _data; }
	}

	protected int _level = 1;	//hero level
	public int Level {
		get { return _level; }
		set {
			if (value > _level) {
				_level = value;
				RecalculateParams();
			}
		}
	}

	public int Leadership { get; private set; }	//hero leadership after all upgrades and level-ups
	public int Experience { get; private set; }	//hero experience
	public int AggroCrystals { get; private set; }	//aggro crystals amount after all upgrades and level-ups

	//TODO: each action will cost different amount of aggro
	//public int AggroPerAction { get; private set; }

	public BaseHero(BaseHeroData data, int experience) : base(data) {
		_data = data;
		AddExperience(experience);
	}

	protected override Dictionary<EUnitEqupmentSlot, EItemType[]> CreateSlotsData() {
		Dictionary<EUnitEqupmentSlot, EItemType[]> slotsData = new Dictionary<EUnitEqupmentSlot, EItemType[]>();
		ArrayRO<EUnitEqupmentSlot> availableSlots = UnitsData.Instance.GetUnitEquipmentSlots(this);
		for (int i = 0; i < availableSlots.Length; i++) {
			slotsData.Add(availableSlots[i], new EItemType[0]);
			for (int j = 0; j < _data.AvailableItemTypes.Length; j++) {
				if (_data.AvailableItemTypes[j].SlotKey == availableSlots[i]) {
					slotsData[availableSlots[i]] = _data.AvailableItemTypes[j].AvailableItemTypes.DataCopy;
					break;
				}
			}
		}
		return slotsData;
	}

	protected override void RecalculateParams() {
		base.RecalculateParams();

		//TODO:
		// - recalculate health after level-ups
		// - recalculate damage after level-ups
		// - recalculate damage range after level-ups
		// - recalculate damage speed after level-ups
		// - recalculate aggro crystals after level-ups
		// - recalculate leadership after level-ups

		//TODO: broadcast message
	}

	protected void AddExperience(int expAmount) {
		if (expAmount > 0) {
			Experience += expAmount;

			//TODO: check new level and level up if necessary
		}
	}
}
