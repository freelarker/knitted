using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class UnitPathfinding : Pathfinding {
	private enum EUnitMovementState {
		None,
		MoveToPrepPoint,
		MoveToAttackPoint,
		WatchEnemy,
		NoEnemy
	}

	private int _searchesPerSecond = 3;
	private float _minDistanceToTargetUnit = 1f;
	private float _speed = 1f;

	private Vector3 _targetPosition = Vector3.zero;
	private Transform _nearestTarget = null;

	private Transform _cachedTransform;
	private WaitForSeconds _cachedWaitForSeconds;

	private Dictionary<EUnitMovementState, Action> _movementStateActions = new Dictionary<EUnitMovementState, Action>();
	private EUnitMovementState _currentState = EUnitMovementState.None;
	private EUnitMovementState CurrentState {
		set {
			_currentState = value;
			Update();
		}
	}

	private Action _onTargetReached;

	public void Awake() {
		_movementStateActions.Add(EUnitMovementState.MoveToPrepPoint, MoveToPreparationPoint);
		_movementStateActions.Add(EUnitMovementState.MoveToAttackPoint, MoveToAttackPoint);
		_movementStateActions.Add(EUnitMovementState.WatchEnemy, WatchEnemy);

		_cachedTransform = transform;
		_cachedWaitForSeconds = new WaitForSeconds(1f / _searchesPerSecond);
	}

	public void Update() {
		if (_currentState != EUnitMovementState.None && _currentState != EUnitMovementState.NoEnemy) {
			_movementStateActions[_currentState]();
		}
	}

	public void Reset() {
		_nearestTarget = null;
		_onTargetReached = null;
		StopAllCoroutines();
	}

	private IEnumerator FindPathTimer() {
		FindPath(_cachedTransform.position, _targetPosition);
		yield return _cachedWaitForSeconds;
		StartCoroutine(FindPathTimer());
	}

	private void FindNearestTarget(ArrayRO<BaseUnitBehaviour> possibleTargets) {
		BaseUnitBehaviour nearestTarget = null;

		float distance = Mathf.Infinity;
		Vector3 position = transform.position;
		for (int i = 0; i < possibleTargets.Length; i++) {
			if (possibleTargets[i].UnitData.IsDead) {
				continue;
			}

			Vector3 diff = possibleTargets[i].transform.position - position;
			float curDistance = diff.sqrMagnitude;
			if (curDistance < distance) {
				nearestTarget = possibleTargets[i];
				distance = curDistance;
			}
		}

		if (nearestTarget != null) {
			_nearestTarget = nearestTarget.transform;
			_targetPosition = _nearestTarget.transform.position;
		} else {
			Debug.LogWarning(string.Format("{0} \"{1}\" reporting: No target found!", gameObject.tag, gameObject.name));
		}
	}

	#region movement
	public void MoveToTarget(BaseUnitBehaviour self, ArrayRO<BaseUnitBehaviour> possibleTargets, Action<BaseUnitBehaviour> onTargetFound, Action onTargetReached) {
		Reset();

		_minDistanceToTargetUnit = self.UnitData.AttackRange;
		_onTargetReached = onTargetReached;

		FindNearestTarget(possibleTargets);
		if (_nearestTarget == null) {
			Reset();
			CurrentState = EUnitMovementState.NoEnemy;
			return;
		}
		onTargetFound(_nearestTarget.gameObject.GetComponent<BaseUnitBehaviour>());

		//setup path to appear on screen
		if (_currentState == EUnitMovementState.None) {
			_targetPosition.z = transform.position.z;
			_targetPosition.x = gameObject.CompareTag(GameConstants.Tags.UNIT_ALLY) ? FightManager.Instance.AllyStartLine.position.x : FightManager.Instance.EnemyStartLine.position.x;

			FindPath(_cachedTransform.position, _targetPosition);

			CurrentState = EUnitMovementState.MoveToPrepPoint;
		} else {
			OnPreparationPointReached();
		}
	}

	private void MoveToPreparationPoint() {
		if (!PerformMovement()) {
			OnPreparationPointReached();
		}
	}

	private void OnPreparationPointReached() {
		CurrentState = EUnitMovementState.MoveToAttackPoint;

		//start searching path to moving target unit
		StartCoroutine(FindPathTimer());
	}

	private void MoveToAttackPoint() {
		//TODO: setup correct target position
		_targetPosition = _nearestTarget.position;

		PerformMovement(_minDistanceToTargetUnit);

		if (Vector3.Distance(_cachedTransform.position, _targetPosition) <= _minDistanceToTargetUnit) {
			OnAttackPointReached();
		}
	}

	private void OnAttackPointReached() {
		Action onTargetReached = _onTargetReached;
		_onTargetReached = null;
		StopAllCoroutines();

		CurrentState = EUnitMovementState.WatchEnemy;

		onTargetReached();
	}

	private void WatchEnemy() {
		_cachedTransform.LookAt(_nearestTarget.transform);
	}

	private bool PerformMovement(float minDistance = 0.4f) {
		if (Path.Count > 0) {
			_cachedTransform.position = Vector3.MoveTowards(_cachedTransform.position, Path[0], Time.deltaTime * _speed);
			if (Vector3.Distance(_cachedTransform.position, Path[0]) < minDistance) {
				Path.RemoveAt(0);
			}
			return true;
		}
		return false;
	}
	#endregion
}
