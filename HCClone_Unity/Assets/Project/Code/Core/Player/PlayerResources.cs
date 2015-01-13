public class PlayerResources {
	public int Ammo { get; set; }
	public int Credits { get; set; }
	public int Minerals { get; set; }

	public PlayerResources(int ammo, int credits, int minerals) {
		Ammo = ammo;
		Credits = credits;
		Minerals = minerals;
	}
}
