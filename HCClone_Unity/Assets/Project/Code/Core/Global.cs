public class Global {
	#region singleton
	private static Global _instance = null;
	public static Global Instance {
		get {
			if (_instance == null) {
				_instance = new Global();
			}
			return _instance;
		}
	}
	#endregion

	public NetworkManager Network { get; private set; }
	public PlayerData Player { get; private set; }
	public MissionInfo CurrentMission { get; private set; }

	public Global() {
		Network = new NetworkManager();
		Player = new PlayerData();
		CurrentMission = new MissionInfo();
	}

	public void Initialize() {
		Player.Load();
	}
}
