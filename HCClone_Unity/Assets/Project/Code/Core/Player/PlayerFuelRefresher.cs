public class PlayerFuelRefresher {
	//TODO: check fuel max level and run timer only on low fuel

	private int _fuelRefreshTime = 6000;	//in seconds

	public PlayerFuelRefresher(int lastRefreshTime) {
		GameTimer.Instance.AddListener(Update, lastRefreshTime - Utils.UnixTimestamp + _fuelRefreshTime);
	}

	private void Update() {
		Global.Instance.Player.Resources.Fuel++;
		GameTimer.Instance.AddListener(Update, Utils.UnixTimestamp + _fuelRefreshTime);
	}
}
