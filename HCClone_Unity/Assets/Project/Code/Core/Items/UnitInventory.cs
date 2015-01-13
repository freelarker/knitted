﻿using System;
using System.Collections.Generic;

/// <summary>
/// Unit's inventory. Contains equipped items
/// </summary>

public class UnitInventory {
	private Action<EUnitEqupmentSlot, EItemKey, EItemKey> _onEquipmentUpdate = null;	//@params: slot, old item, new item

	private Dictionary<EUnitEqupmentSlot, EItemType[]> _slotsConfig;
	private ItemSlot[] _equipment = null;

	/// <summary>
	/// C'tor
	/// </summary>
	/// <param name="_slotsData">Data about slot: slot key && available item types for the slot</param>
	/// <param name="onEquipmentUpdate"></param>
	public UnitInventory(Dictionary<EUnitEqupmentSlot, EItemType[]> _slotsData, Action<EUnitEqupmentSlot, EItemKey, EItemKey> onEquipmentUpdate) {
		_slotsConfig = _slotsData;

		_equipment = new ItemSlot[_slotsConfig.Count];
		int i = 0;
		foreach(KeyValuePair<EUnitEqupmentSlot, EItemType[]> kvp in _slotsConfig) {
			_equipment[i] = new ItemSlot(kvp.Key, EItemKey.None);
			i++;
		}

		_onEquipmentUpdate = onEquipmentUpdate;
	}

	//equip item
	public void Equip(int slotId, EItemKey itemKey) {
		//check slotId
		if (slotId < 0 || _equipment.Length - 1 < slotId) {
			EventsAggregator.Items.Broadcast<int>(EItemEvent.WrongSlotId, slotId);
			return;
		}

		if (!CanEquipItem(itemKey)) {
			EventsAggregator.Items.Broadcast<int>(EItemEvent.WrongItem, slotId);
			return;
		}

		BaseItem item = ItemsData.Instance.GetItem(itemKey);

		EItemKey oldItemKey = GetItemInSlot(slotId);
		_equipment[slotId].ItemKey = item.Key;

		if (_onEquipmentUpdate != null) {
			_onEquipmentUpdate(item.Slot, oldItemKey, itemKey);
		}
	}

	//get item key in slot
	public EItemKey GetItemInSlot(int slotId) {
		if (slotId < 0 || _equipment.Length - 1 < slotId) {
			EventsAggregator.Items.Broadcast<int>(EItemEvent.WrongSlotId, slotId);
			return EItemKey.None;
		}

		return _equipment[slotId].ItemKey;
	}

	public bool CanEquipItem(EItemKey itemKey) {
		BaseItem item = ItemsData.Instance.GetItem(itemKey);

		//check item data found
		if (item == null) {
			return false;
		}

		//check slot is correct
		if(!_slotsConfig.ContainsKey(item.Slot)) {
			return false;
		}

		//no slot restrictions
		if (_slotsConfig[item.Slot].Length == 0) {
			return true;
		}

		//check item fits to slot
		for (int i = 0; i < _slotsConfig[item.Slot].Length; i++) {
			if (_slotsConfig[item.Slot][i] == item.Type) {
				return true;
			}
		}

		return false;
	}
}