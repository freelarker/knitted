using UnityEngine;

[System.Serializable]
public class  BaseHeroData : BaseUnitData {
	[SerializeField]
	protected int _baseLeadership = 0;	//base leadership (without upgrades and level-ups)
	public int BaseLeadership {
		get { return _baseLeadership; }
	}

	[SerializeField]
	protected int _baseAggroCrystalsPerAttack = 0;	//base aggro crystals hero receives per attack (without upgrades and level-ups)
	public int BaseAggroCrystalPerAttack {
		get { return _baseAggroCrystalsPerAttack; }
	}

	[SerializeField]
	protected int _baseAggroCrystalsMaximum = 0;	//base aggro crystals cap (without upgrades and level-ups)
	public int BaseAggroCrystalsMaximum {
		get { return _baseAggroCrystalsMaximum; }
	}

	[SerializeField]
	protected ItemSlotConfig[] _availableItemTypes = new ItemSlotConfig[0];
	protected ArrayRO<ItemSlotConfig> _availableItemTypesRO = null;
	public ArrayRO<ItemSlotConfig> AvailableItemTypes {
		get {
			if (_availableItemTypesRO == null) {
				_availableItemTypesRO = new ArrayRO<ItemSlotConfig>(_availableItemTypes);
				_availableItemTypes = null;
			}
			return _availableItemTypesRO;
		}
	}
}
