public class ItemSlot {
	public EUnitEqupmentSlot SlotName { get; set; }
	public EItemKey ItemKey { get; set; }

	public ItemSlot(EUnitEqupmentSlot slotName) {
		SlotName = slotName;
		ItemKey = EItemKey.None;
	}

	public ItemSlot(EUnitEqupmentSlot slotName, EItemKey itemKey) {
		SlotName = slotName;
		ItemKey = itemKey;
	}
}