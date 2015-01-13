using UnityEngine;

[System.Serializable]
public class ItemSlotConfig  {
	[SerializeField]
	private EUnitEqupmentSlot _slotKey = EUnitEqupmentSlot.Other;
	public EUnitEqupmentSlot SlotKey {
		get { return _slotKey; }
	}

	[SerializeField]
	private EItemType[] _availableItemTypes = new EItemType[0];
	private ArrayRO<EItemType> _availableItemTypesRO = null;
	public ArrayRO<EItemType> AvailableItemTypes {
		get { return _availableItemTypesRO; }
	}

	public ItemSlotConfig() {
		_availableItemTypesRO = new ArrayRO<EItemType>(_availableItemTypes);
		_availableItemTypes = null;
	}
}
