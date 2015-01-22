using System.Collections.Generic;

public class PlayerInventory {
	//TODO: items equip state
	private List<PlayerItem> _items = new List<PlayerItem>();
	private ListRO<PlayerItem> _itemsRO = null;
	public ListRO<PlayerItem> Items {
		get { return _itemsRO; }
	}

	public PlayerInventory() {
		_itemsRO = new ListRO<PlayerItem>(_items);
	}

	~PlayerInventory() {

	}

	public void AddItem(BaseItem item) {
		_items.Add(new PlayerItem(item));
	}

	public bool RemoveItem(BaseItem item) {
		for (int i = 0; i < _items.Count; i++) {
			if (_items[i].ItemData == item) {
				_items.RemoveAt(i);
				return true;
			}
		}
		return false;
	}

	public void Equip(BaseHero hero, int slotId, EItemKey itemKey) {
		hero.Inventory.Equip(slotId, itemKey);
	}

	public void Unequip(BaseHero hero, int slotId) {
		hero.Inventory.Equip(slotId, EItemKey.None);
	}

	#region listeners
	private void OnItemEquip(BaseUnit unit) {
		if (UnitsData.Instance.IsHero(unit.Data.Key)) {
			
		}
	}
	#endregion
}
