using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(HCCGridObject))]
public class UnitPathfinding : MonoBehaviour {
	private enum EUnitMovementState {
		None,
		MoveToFreePoint,
		MoveToPrepPoint,
		MoveToAttackPoint,
		WatchEnemy,
		NoEnemy
	}

	[SerializeField]
	private HCCGridObject _gridObject;
	[SerializeField]
	private UnitModelView _model;

	private int _searchesPerSecond = 2;
	private float _minDistanceToTargetUnit = 1f;
	private float _speed = 1f;

	private EHCCGridDirection _freePointDirection = EHCCGridDirection.None;
	private HCCGridPoint _freePointDirectionVector = HCCGridPoint.Zero;

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
		if (_model == null) {
			_model = gameObject.GetComponentInChildren<UnitModelView>();
		}
		_model.MovementSpeed = _speed;

		_movementStateActions.Add(EUnitMovementState.MoveToFreePoint, MoveToFreePoint);
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
		//_gridObject.FindPath(_targetPosition, _gridObject, _nearestTarget.gameObject.GetComponent<HCCGridObject>());
		_gridObject.FindPath(_nearestTarget.gameObject.GetComponent<HCCGridObject>());
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
			//if there's interaction with some object we should get rid of it first
			//List<HCCGridObject> objectsAtMyRect = HCCGridController.Instance.GetObjectsAtRect(_gridObject.GridRect);
			//if (objectsAtMyRect.Count > 1) {
			//	CalculateFreePointDirection(objectsAtMyRect);
			//	CurrentState = EUnitMovementState.MoveToFreePoint;
			//} else {
			//	OnFreePointReached();
			//}
			OnFreePointReached();
		} else {
			OnPreparationPointReached();
		}

		_model.PlayRunAnimation();
	}

	private void CalculateFreePointDirection() {
		CalculateFreePointDirection(HCCGridController.Instance.GetObjectsAtRect(_gridObject.GridRect));
	}

	private void CalculateFreePointDirection(List<HCCGridObject> objectsAtMyRect) {
		if (objectsAtMyRect.Count > 1) {
			List<EHCCGridDirection> directions = new List<EHCCGridDirection>();
			for (int i = 0; i < objectsAtMyRect.Count; i++) {
				if (objectsAtMyRect[i] != _gridObject) {
					directions.Add(objectsAtMyRect[i].MoveDirection);
				}
			}
			directions = HCCGridDirection.GetFreeDirections(directions, false);

			if (directions.IndexOf(_freePointDirection) == -1) {
				_freePointDirection = directions[UnityEngine.Random.Range(0, directions.Count - 1)];
				_freePointDirectionVector = HCCGridDirection.DirectionToVector(_freePointDirection);
			}
		}
	}

	private void CheckFreePoint() {
		List<HCCGridObject> objectsAtMyRect = HCCGridController.Instance.GetObjectsAtRect(_gridObject.GridRect);
		if (objectsAtMyRect.Count > 1) {
			CalculateFreePointDirection(objectsAtMyRect);

			HCCGridPoint myPosition = _gridObject.GridPosition;
			_gridObject.Path.Add(HCCGridController.Instance.GridData.GetCell(myPosition.X + _freePointDirectionVector.X, myPosition.Z + _freePointDirectionVector.Z));
			_gridObject.Path.Add(HCCGridController.Instance.GridData.GetCell(myPosition.X + _freePointDirectionVector.X * 2, myPosition.Z + _freePointDirectionVector.Z * 2));
		} else {
			OnFreePointReached();
		}
	}

	private void MoveToFreePoint() {
		if (!PerformMovement()) {
			CheckFreePoint();
		}

		if (_gridObject.Path.Count > 0) {
			_cachedTransform.LookAt(HCCGridController.Instance.GridView.GridToWorldPos(_gridObject.Path[0].GridPosition));
		}
	}

	private void OnFreePointReached() {
		_freePointDirection = EHCCGridDirection.None;
		_freePointDirectionVector = HCCGridPoint.Zero;

		_targetPosition.z = transform.position.z;
		_targetPosition.x = gameObject.CompareTag(GameConstants.Tags.UNIT_ALLY) ? FightManager.Instance.AllyStartLine.position.x : FightManager.Instance.EnemyStartLine.position.x;

		_gridObject.FindPath(_targetPosition, _gridObject);

		CurrentState = EUnitMovementState.MoveToPrepPoint;
	}

	private void MoveToPreparationPoint() {
		if (!PerformMovement()) {
			OnPreparationPointReached();
		}

		if(_gridObject.Path.Count > 0) {
			_cachedTransform.LookAt(HCCGridController.Instance.GridView.GridToWorldPos(_gridObject.Path[0].GridPosition));
		}
	}

	private void OnPreparationPointReached() {
		CurrentState = EUnitMovementState.MoveToAttackPoint;

		//start searching path to moving target unit
		StartCoroutine(FindPathTimer());
	}

	private void MoveToAttackPoint() {
		_targetPosition = _nearestTarget.position;

		PerformMovement();
		if (_gridObject.Path.Count > 0) {
			_cachedTransform.LookAt(HCCGridController.Instance.GridView.GridToWorldPos(_gridObject.Path[0].GridPosition));
		}

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

	private bool PerformMovement(float minDistance = 0.6f) {
		if (_gridObject.Path.Count > 0) {
			Vector3 pos = HCCGridController.Instance.GridView.GridToWorldPos(_gridObject.Path[0].GridPosition);
			_cachedTransform.position = Vector3.MoveTowards(_cachedTransform.position, pos, Time.deltaTime * _speed);
			if (Vector3.Distance(_cachedTransform.position, pos) < minDistance) {
				_gridObject.PathPointReached();
			}
			return true;
		}
		return false;
	}
	#endregion
}
