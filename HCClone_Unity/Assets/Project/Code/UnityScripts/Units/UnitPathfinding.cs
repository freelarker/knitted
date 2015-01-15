using UnityEngine;
using System.Collections;

public class UnitPathfinding : Pathfinding {
	private int _searchesPerSecond = 3;
	private float _minDistanceToTargetUnit = 1f;
	private float _speed = 1f;

	private bool _appearedOnScreen = false;

	private Vector3 _targetPosition = Vector3.zero;
	private Transform _nearestTarget = null;

	private Transform _cachedTransform;

	public void Awake() {
		_cachedTransform = transform;
		enabled = false;
	}

	public void FixedUpdate() {
		if (!_appearedOnScreen) {
			MoveToPreparationPoint();
		} else {
			MoveToAttackPoint();
			if (Vector3.Distance(_cachedTransform.position, _targetPosition) <= _minDistanceToTargetUnit) {
				OnAttackPointReached();
			}
		}
	}

	private IEnumerator FindPathTimer() {
		FindPath(_cachedTransform.position, _targetPosition);
		yield return new WaitForSeconds((float)(1.0f / _searchesPerSecond));
		StartCoroutine(FindPathTimer());
	}

	private void FindNearestTarget(ArrayRO<BaseUnitBehaviour> possibleTargets) {
		BaseUnitBehaviour nearestTarget = null;

		float distance = Mathf.Infinity;
		Vector3 position = transform.position;
		for (int i = 0; i < possibleTargets.Length; i++) {
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
	public void MoveToUnit(BaseUnitBehaviour self, ArrayRO<BaseUnitBehaviour> possibleTargets) {
		_minDistanceToTargetUnit = self.UnitData.AttackRange;

		FindNearestTarget(possibleTargets);

		//setup path to appear on screen
		if (!_appearedOnScreen) {
			_targetPosition.z = transform.position.z;
			_targetPosition.x = gameObject.CompareTag(GameConstants.Tags.UNIT_ALLY) ? FightManager.Instance.AllyStartLine.position.x : FightManager.Instance.EnemyStartLine.position.x;

			FindPath(_cachedTransform.position, _targetPosition);
		}

		enabled = true;
	}

	private void MoveToPreparationPoint() {
		if (!PerformMovement()) {
			OnPreparationPointReached();
		}
	}

	private void OnPreparationPointReached() {
		_appearedOnScreen = true;

		//start searching path to moving target unit
		StartCoroutine(FindPathTimer());
	}

	private void MoveToAttackPoint() {
		//TODO: setup correct target position
		_targetPosition = _nearestTarget.position;

		PerformMovement(_minDistanceToTargetUnit);
	}

	private void OnAttackPointReached() {
		_cachedTransform.LookAt(_nearestTarget.transform);

		//clear data
		_nearestTarget = null;

		//TODO: broadcast message

		enabled = false;
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
