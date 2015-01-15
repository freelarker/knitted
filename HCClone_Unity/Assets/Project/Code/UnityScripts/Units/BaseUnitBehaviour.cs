using UnityEngine;

public class BaseUnitBehaviour : MonoBehaviour {
	[SerializeField]
	private UnitPathfinding _unitPathfinder;

	private BaseUnit _unitData = null;
	public BaseUnit UnitData {
		get { return _unitData; }
	}

	public void Setup(BaseUnit unitData, string tag) {
		_unitData = unitData;
		gameObject.tag = tag;
	}

	public void Run() {
		//WARNING! temp
		_unitPathfinder.MoveToUnit(this, gameObject.CompareTag(GameConstants.Tags.UNIT_ALLY) ? FightManager.Instance.EnemyUnits : FightManager.Instance.AllyUnits);
	}

	private void OnFightPause() {
		
	}

	private void OnFightResume() {

	}
}
