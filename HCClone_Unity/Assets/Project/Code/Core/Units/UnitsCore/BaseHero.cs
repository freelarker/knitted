using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class BaseHero : BaseUnit {
	[SerializeField]
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
	public int AggroCrystalsMaximum { get; private set; }	//aggro crystals cap after all upgrages and level-ups
	private int _aggroCrystals = 0;	//aggro crystals amount after all upgrades and level-ups
	public int AggroCrystals {
		get { return _aggroCrystals; }
		private set {
			_aggroCrystals = (value < 0 ? 0 : (value > AggroCrystalsMaximum ? AggroCrystalsMaximum : value));
			EventsAggregator.Units.Broadcast<int>(EUnitEvent.AggroCrystalsUpdate, _aggroCrystals);
		}
	}


	

	//TODO: each action will cost different amount of aggro
	//public int AggroPerAction { get; private set; }

	public BaseHero(BaseHeroData data, int experience) : base(data) {
		_data = data;
		AddExperience(experience);

		RecalculateParamsInternal();

		EventsAggregator.Fight.AddListener<BaseUnit>(EFightEvent.AllyDeath, OnAllyDeath);
		EventsAggregator.Fight.AddListener<BaseUnit>(EFightEvent.EnemyDeath, OnEnemyDeath);
	}

	~BaseHero() {
		EventsAggregator.Fight.RemoveListener<BaseUnit>(EFightEvent.AllyDeath, OnAllyDeath);
		EventsAggregator.Fight.RemoveListener<BaseUnit>(EFightEvent.EnemyDeath, OnEnemyDeath);
	}

	public override void Attack(BaseUnit target) {
		base.Attack(target);

		AggroCrystals += _data.BaseAggroCrystalPerAttack;
	}

	protected override Dictionary<EUnitEqupmentSlot, EItemType[]> CreateSlotsData() {
		BaseHeroData heroData = base._data as BaseHeroData;
		Dictionary<EUnitEqupmentSlot, EItemType[]> slotsData = new Dictionary<EUnitEqupmentSlot, EItemType[]>();
		ArrayRO<EUnitEqupmentSlot> availableSlots = UnitsConfig.Instance.GetUnitEquipmentSlots(this);
		for (int i = 0; i < availableSlots.Length; i++) {
			slotsData.Add(availableSlots[i], new EItemType[0]);
			for (int j = 0; j < heroData.AvailableItemTypes.Length; j++) {
				if (heroData.AvailableItemTypes[j].SlotKey == availableSlots[i]) {
					slotsData[availableSlots[i]] = heroData.AvailableItemTypes[j].AvailableItemTypes.DataCopy;
					break;
				}
			}
		}
		return slotsData;
	}

	protected override void RecalculateParamsInternal() {
		base.RecalculateParamsInternal();

		//TODO:
		// - recalculate health after level-ups
		// - recalculate damage after level-ups
		// - recalculate damage range after level-ups
		// - recalculate damage speed after level-ups
		// - recalculate aggro crystals after level-ups
		// - recalculate leadership after level-ups
		//Debug.LogWarning("=== data: " + _data + ", base.data: " + base.Data);
		if (_data != null) {
			Leadership = _data.BaseLeadership;
			AggroCrystalsMaximum = _data.BaseAggroCrystalsMaximum;
		}

		EventsAggregator.Units.Broadcast<BaseUnit>(EUnitEvent.RecalculateParams, this);
	}

	public void AddExperience(int expAmount) {
		if (expAmount > 0) {
			Experience += expAmount;

			//TODO: check new level and level up if necessary
		}
	}

	#region listeners
	protected void OnAllyDeath(BaseUnit unit) {
		if (unit == this) {
			return;
		}

		AggroCrystals += unit.Data.AggroCrystalsForDeathToAlly;
	}

	protected void OnEnemyDeath(BaseUnit unit) {
		AggroCrystals += unit.Data.AggroCrystalsForDeathToEnemy;
	}
	#endregion
}
