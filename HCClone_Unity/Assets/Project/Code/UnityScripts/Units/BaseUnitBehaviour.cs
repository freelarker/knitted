using UnityEngine;
using System.Collections;

public class BaseUnitBehaviour : MonoBehaviour {
	[SerializeField]
	private UnitPathfinding _unitPathfinder;

	[SerializeField]
	private UnitModelView _model;
	public UnitModelView ModelView {
		get { return _model; }
	}

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
		if (_model == null) {
			_model = gameObject.GetComponentInChildren<UnitModelView>();
		}

		EventsAggregator.Units.AddListener<BaseUnit, HitInfo>(EUnitEvent.HitReceived, OnHitReceived);
		EventsAggregator.Units.AddListener<BaseUnit, HitInfo>(EUnitEvent.DeathCame, OnUnitDeath);
		EventsAggregator.Fight.AddListener(EFightEvent.Pause, OnFightPause);
		EventsAggregator.Fight.AddListener(EFightEvent.Resume, OnFightResume);
	}

	public void OnDestroy() {
		EventsAggregator.Units.RemoveListener<BaseUnit, HitInfo>(EUnitEvent.HitReceived, OnHitReceived);
		EventsAggregator.Units.RemoveListener<BaseUnit, HitInfo>(EUnitEvent.DeathCame, OnUnitDeath);
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
		_unitPathfinder.Reset(true);

		EventsAggregator.Fight.Broadcast<BaseUnit>(gameObject.tag == GameConstants.Tags.UNIT_ALLY ? EFightEvent.AllyDeath : EFightEvent.EnemyDeath, _unitData);

		_model.PlayDeathAnimation(OnDeathAnimationEnd);
	}

	private IEnumerator AttackTarget() {
		_model.PlayAttackAnimation();

		EventsAggregator.Fight.Broadcast<BaseUnitBehaviour, BaseUnitBehaviour>(EFightEvent.PerformAttack, this, _targetUnit);
		
		if (_targetUnit != null && !_targetUnit.UnitData.IsDead) {
			yield return _cachedWaitForSeconds;
			StartCoroutine(AttackTarget());
		}
	}

	private void OnHitReceived(BaseUnit unit, HitInfo hitInfo) {
		if (unit == _unitData) {
			_model.PlayHitAnimation(unit.Health, hitInfo);
		}
	}

	private IEnumerator Vanish() {
		yield return new WaitForSeconds(1f);

		GameObject.Destroy(gameObject);
	}
	#endregion

	#region listeners
	private void OnUnitDeath(BaseUnit unitData, HitInfo hitInfo) {
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

	private void OnDeathAnimationEnd() {
		StartCoroutine(Vanish());
	}
	#endregion
}
