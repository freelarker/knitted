public class PlayerItem {
	private BaseItem _itemData = null;
	public BaseItem ItemData {
		get { return _itemData; }
	}

	public bool IsEquipped { get; set; }

	public PlayerItem(BaseItem itemData) {
		_itemData = itemData;
		IsEquipped = false;
	}
}
