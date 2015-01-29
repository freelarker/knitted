using UnityEngine;

[System.Serializable]
public class MissionData {
	[SerializeField]
	private EMissionKey _key = EMissionKey.None;
	public EMissionKey Key {
		get { return _key; }
	}

	[SerializeField]
	private int _fuelWinCost = 0;
	public int FuelWinCost {
		get { return _fuelWinCost; }
	}

	[SerializeField]
	private int _fuelLoseCost = 0;
	public int FuelLoseCost {
		get { return _fuelLoseCost; }
	}

	[SerializeField]
	private int _creditsWinCost = 0;
	public int CreditsWinCost {
		get { return _creditsWinCost; }
	}

	[SerializeField]
	private int _creditsLoseCost = 0;
	public int CreditsLoseCost {
		get { return _creditsLoseCost; }
	}

	[SerializeField]
	private int _mineralsWinCost = 0;
	public int MineralsWinCost {
		get { return _mineralsWinCost; }
	}

	[SerializeField]
	private int _mineralsLoseCost = 0;
	public int MineralsLoseCost {
		get { return _mineralsLoseCost; }
	}

	[SerializeField]
	private int _rewardFuel = 0;
	public int RewardFuel {
		get { return _rewardFuel; }
	}

	[SerializeField]
	private int _rewardCredits = 0;
	public int RewardCredits {
		get { return _rewardCredits; }
	}

	[SerializeField]
	private int _rewardMinerals = 0;
	public int RewardMinerals {
		get { return _rewardMinerals; }
	}

	[SerializeField]
	private int _mineIncome = 0;
	public int MineIncome {
		get { return _mineIncome; }
	}
	public bool HasMine {
		get { return _mineIncome > 0; }
	}

	[SerializeField]
	private MissionMapData[] _maps = new MissionMapData[0];
	public int MapsCount {
		get { return _maps.Length; }
	}

	public MissionMapData GetMap(int index) {
		return index >= 0 || index < _maps.Length ? _maps[index] : null;
	}
}
