public class PlayerResources {
	public int Fuel { get; set; }
	public int Credits { get; set; }
	public int Minerals { get; set; }

	public PlayerResources(int fuel, int credits, int minerals) {
		Fuel = fuel;
		Credits = credits;
		Minerals = minerals;
	}
}
