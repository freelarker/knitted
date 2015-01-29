using UnityEngine;

[System.Serializable]
public class CBConstructionRequirement {
	[SerializeField]
	private int _buildTime = 0;	//Construction time (in seconds)
	public int BuildTime {
		get { return _buildTime; }
	}

	[SerializeField]
	private int _costCredits = 0;	//Construction cost in credits
	public int CostCredits {
		get { return _costCredits; }
	}

	[SerializeField]
	private int _costFuel = 0;	//Construction cost in fuel
	public int CostFuel {
		get { return _costFuel; }
	}

	[SerializeField]
	private int _costMinerals = 0;	//Construction cost in minerals
	public int CostMinerals {
		get { return _costMinerals; }
	}

	[SerializeField]
	private ConstructionBuildingRequirement[] _buildingRequirements = null;
	private ArrayRO<ConstructionBuildingRequirement> _buildingRequirementsRO = null;
	public ArrayRO<ConstructionBuildingRequirement> BuildingRequirements {
		get {
			if (_buildingRequirementsRO == null) {
				_buildingRequirementsRO = new ArrayRO<ConstructionBuildingRequirement>(_buildingRequirements);
			}
			return _buildingRequirementsRO;
		}
	}

}
