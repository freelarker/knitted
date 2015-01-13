public class MissionInfo {
	public EMissionKey Key { get; set; }
	public ArrayRO<BaseSoldier> SelectedSoldiers { get; set; }

	public void Clear() {
		Key = EMissionKey.None;
		SelectedSoldiers = null;
	}
}
