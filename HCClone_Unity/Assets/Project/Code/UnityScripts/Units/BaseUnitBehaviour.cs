using UnityEngine;
using System.Collections;

public class BaseUnitBehaviour : MonoBehaviour {
	[SerializeField]
	private UnitPathfinding _unitPathfinder;

	private BaseUnit _unitData = null;
	public BaseUnit UnitData {
		get { return _unitData; }
	}

	private BaseUnitBehaviour _targetUnit;
	private bool _isAlly = false;
	public bool IsAlly {
		get { return _isAlly; }
	}

	private WaitForSeconds _cachedWaitForSeconds;

	public void Awake() {
		EventsAggregator.Units.AddListener<BaseUnit>(EUnitEvent.DeathCame, OnUnitDeath);
		EventsAggregator.Fight.AddListener(EFightEvent.Pause, OnFightPause);
		EventsAggregator.Fight.AddListener(EFightEvent.Resume, OnFightResume);
	}

	public void OnDestroy() {
		EventsAggregator.Units.RemoveListener<BaseUnit>(EUnitEvent.DeathCame, OnUnitDeath);
		EventsAggregator.Fight.RemoveListener(EFightEvent.Pause, OnFightPause);
		EventsAggregator.Fight.RemoveListener(EFightEvent.Resume, OnFightResume);
	}

	public void Setup(BaseUnit unitData, string tag) {
		_unitData = unitData;
		gameObject.tag = tag;
		_isAlly = gameObject.CompareTag(GameConstants.Tags.UNIT_ALLY);

		_cachedWaitForSeconds = new WaitForSeconds(1f / unitData.AttackSpeed);
	}

	public void Run() {
		//WARNING! temp
		_unitPathfinder.MoveToTarget(this, _isAlly ? FightManager.Instance.EnemyUnits : FightManager.Instance.AllyUnits, OnTargetFound, OnTargetReached);
	}

	#region unit controller
	private void OnTargetFound(BaseUnitBehaviour target) {
		//TODO: play move animation
		_targetUnit = target;
	}

	private void OnTargetReached() {
		StartCoroutine(AttackTarget());
	}

	private void OnTargetDeath() {
		StopAllCoroutines();
		_targetUnit = null;
		_unitPathfinder.MoveToTarget(this, _isAlly ? FightManager.Instance.EnemyUnits : FightManager.Instance.AllyUnits, OnTargetFound, OnTargetReached);
	}

	private void OnSelfDeath() {
		StopAllCoroutines();
		_targetUnit = null;
		_unitPathfinder.Reset();

		EventsAggregator.Fight.Broadcast<BaseUnit>(gameObject.tag == GameConstants.Tags.UNIT_ALLY ? EFightEvent.AllyDeath : EFightEvent.EnemyDeath, _unitData);

		//WARNING! temp
		GameObject.Destroy(gameObject);

		////TODO: play death animation
	}

	private IEnumerator AttackTarget() {
		//TODO: play attack animation

		EventsAggregator.Fight.Broadcast<BaseUnitBehaviour, BaseUnitBehaviour>(EFightEvent.PerformAttack, this, _targetUnit);
		yield return _cachedWaitForSeconds;

		if (!_targetUnit.UnitData.IsDead) {
			StartCoroutine(AttackTarget());
		}
	}
	#endregion

	#region listeners
	private void OnUnitDeath(BaseUnit unitData) {
		if (unitData == _unitData) {
			OnSelfDeath();
		} else if (_targetUnit != null && unitData == _targetUnit._unitData) {
			OnTargetDeath();
		}
	}

	private void OnFightPause() {
		
	}

	private void OnFightResume() {

	}
	#endregion
}
