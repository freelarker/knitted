using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class SearchPlayerAI : Pathfinding
{
	private enum UnitState {
		SearchingPathToCenter = 1,
		MovingToCenter,
		SearchingTarget,
		MovingToTarget,
		TargetCollide
	}

    public int SearchesPerSecond;
    public float MinDistanceToPlayer = 2.0f;
    public float AISpeed = 0.0f;
    private float AIPivotPointYValueOverGround = 0.0f;

	private GameObject[] _enemies;
    private Transform _myTarget;
	private Transform _myLine;
	private Vector3 _directionToCenter = Vector3.zero;

	private UnitState _currentState = UnitState.SearchingPathToCenter;
	private UnitState CurrentState {
		set {
			Debug.Log(gameObject.name + " reporting - set current state: " + value);
			_currentState = value;
		}
	}
	private Dictionary<UnitState, Action> _updateActions = new Dictionary<UnitState, Action>();

	public void Awake() {
		_updateActions.Add(UnitState.SearchingPathToCenter, FindPathToCenter);
		_updateActions.Add(UnitState.MovingToCenter, MoveToCenter);
		_updateActions.Add(UnitState.SearchingTarget, FindNearestTarget);
		_updateActions.Add(UnitState.MovingToTarget, MoveToEnemy);
		_updateActions.Add(UnitState.TargetCollide, TargetCollision);
	}

    void Start()
    {   
		if (gameObject.tag == "UnitEnemy")
		{
			_enemies = GameObject.FindGameObjectsWithTag ("UnitAlly");
			_myLine = GameObject.FindGameObjectWithTag("LineRight").transform;
			_directionToCenter.x = -1f;
		} 
		else 
		{
			_enemies = GameObject.FindGameObjectsWithTag("UnitEnemy");
			_myLine = GameObject.FindGameObjectWithTag("LineLeft").transform;
			_directionToCenter.x = 1f;
		}
    }

    void FixedUpdate()
    {
		_updateActions[_currentState]();
    }

    //A test move function, can easily be replaced
    public void MoveSearchingAI()
    {
		if (Path.Count > 0) {
			transform.position = Vector3.MoveTowards(transform.position, Path[0] + new Vector3(0, AIPivotPointYValueOverGround, 0), Time.deltaTime * AISpeed);
			if (Vector3.Distance(transform.position, Path[0]) < 0.4F) {
				Path.RemoveAt(0);
			}
		}
    }

    IEnumerator FindPathTimer()
    {
		FindPath(transform.position, _myTarget.position);
        yield return new WaitForSeconds((float)(1.0f / SearchesPerSecond));
        StartCoroutine(FindPathTimer());
	}

	#region functions for test
	private void FindPathToCenter() {
		Vector3 targetPosition = new Vector3(_myLine.position.x + _directionToCenter.x * (_myLine.localScale.x * 0.5f + transform.localScale.x), transform.position.y, transform.position.z);
		GameObject go = new GameObject(gameObject.name + "_target");
		go.transform.position = targetPosition;

		CurrentState = UnitState.MovingToCenter;
		FindPath(transform.position, targetPosition);
	}

	private void MoveToCenter() {
		if (Path.Count > 0) {
			MoveSearchingAI();
		} else {
			CurrentState = UnitState.SearchingTarget;
		}
	}

	// поиск ближайшего врага
	private void FindNearestTarget() {
		GameObject nearestTarget = null;

		float distance = Mathf.Infinity;
		Vector3 position = transform.position;
		foreach (GameObject enemy in _enemies) {
			Vector3 diff = enemy.transform.position - position;
			float curDistance = diff.sqrMagnitude;
			if (curDistance < distance) {
				nearestTarget = enemy;
				distance = curDistance;
			}
		}
		
		if (nearestTarget != null) {
			Debug.Log(gameObject.name + " reporting - nearest target found: " + nearestTarget.name);

			_myTarget = nearestTarget.transform;

			CurrentState = UnitState.MovingToTarget;
			StartCoroutine(FindPathTimer());
			_updateActions[_currentState]();
		} else {
			Debug.Log("No target found!");
		}
	}

	private void MoveToEnemy() {
		if (Vector3.Distance(transform.position, _myTarget.position) > MinDistanceToPlayer) {
			MoveSearchingAI();
		} else {
			CurrentState = UnitState.TargetCollide;
		}

		transform.LookAt(_myTarget);
	}

	private void TargetCollision() {
		Debug.Log(gameObject.name + " reporting - target reached");
		this.enabled = false;
	}
	#endregion
}
