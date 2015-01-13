using System.Collections.Generic;

/// <summary>
/// Information about heroes player have
/// </summary>
public class PlayerHeroes {
	public BaseHero Current { get; private set; }

	public PlayerHeroes(BaseHero currentHero) {
		Current = currentHero;
	}
}
