﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HCCGridObject : MonoBehaviour {
	[SerializeField]
	private Renderer _modelRenderer = null;

	[SerializeField]
	private bool _isStatic = false;		//if object marked as static it's cells are unwalkable
	public bool IsStatic {
		get { return _isStatic; }
	}

	//for static bounds any static renderer can be assigned
	public float XMinWorldPos {
		get { return _modelRenderer.bounds.min.x; }
	}
	public float XMaxWorldPos {
		get { return _modelRenderer.bounds.max.x; }
	}
	public float ZMinWorldPos {
		get { return _modelRenderer.bounds.min.z; }
	}
	public float ZMaxWorldPos {
		get { return _modelRenderer.bounds.max.z; }
	}
	public float XWorldSize { get; private set; }
	public float ZWorldSize { get; private set; }

	public HCCGridRect GridRect {
		get { return HCCGridView.Instance.WorldPosToGridRect(this); }
	}

	public HCCGridRect GridRectAtPosition(Vector3 worldPosition) {
		return HCCGridView.Instance.WorldPosToGridRect(this, worldPosition);
	}

	public HCCGridPoint GridPosition {
		get { return HCCGridController.Instance.GridView.WorldToGridPos(_cachedTransform.position); }
	}

	public bool IsMoving {
		get { return Path.Count > 0; }
	}

	public EHCCGridDirection MoveDirection {
		get { return Path.Count == 0 ? EHCCGridDirection.None : HCCGridDirection.VectorToDirection(Path[0].GridPosition - GridPosition); }
	}

	public List<HCCCell> Path { get; private set; }

	private Transform _cachedTransform = null;
	private Vector3 _oldWorldPosition = Vector3.zero;
	private HCCGridRect _oldGridRect = new HCCGridRect();

	public void Awake() {
		Initialize();
	}

	public void Start() {
		Register();

		_oldWorldPosition = _cachedTransform.position;
		_oldGridRect = HCCGridController.Instance.GridView.WorldPosToGridRect(this);
	}

	public void Update() {
		if (!_isStatic && _cachedTransform.position != _oldWorldPosition) {
			_oldWorldPosition = _cachedTransform.position;
			UpdateGidPosition();
		}
	}

	public void OnDestroy() {
		Unregister();
	}

	private void Initialize() {
		Path = new List<HCCCell>();

		if (_modelRenderer == null) {
			_modelRenderer = gameObject.renderer;
		}
		_cachedTransform = transform;

		//init world size
		Bounds bounds = _modelRenderer.bounds;
		XWorldSize = Mathf.Abs(bounds.max.x - bounds.min.x);
		ZWorldSize = Mathf.Abs(bounds.max.z - bounds.min.z);
	}

	private void Register() {
		List<HCCCell> targetCells = HCCGridController.Instance.GetGridObjectCellsList(this);
		if (targetCells != null) {
			for (int i = 0; i < targetCells.Count; i++) {
				targetCells[i].ObjectData.Add(this);
			}
		}
	}

	private void Unregister() {
		List<HCCCell> cellsList = HCCGridController.Instance.GetGridRectCellsList(_oldGridRect);
		for (int i = 0; i < cellsList.Count; i++) {
			if (cellsList[i].ObjectData.IndexOf(this) != -1) {
				cellsList[i].ObjectData.Remove(this);
			}
		}
	}

	private void UpdateGidPosition() {
		HCCGridRect gridRect = HCCGridController.Instance.GridView.WorldPosToGridRect(this);
		//if(HCCGridController.Instance.IsGridSpaceAvailable(gridRect, this)true) {
			if (!gridRect.Equals(_oldGridRect)) {
				//TODO: make more exact check of all object cells
				List<HCCCell> cellsList = HCCGridController.Instance.GetGridRectCellsList(_oldGridRect);
				for (int i = 0; i < cellsList.Count; i++) {
					if (cellsList[i].ObjectData.IndexOf(this) != -1) {
						cellsList[i].ObjectData.Remove(this);
					}
				}

				cellsList = HCCGridController.Instance.GetGridRectCellsList(gridRect);
				for (int i = 0; i < cellsList.Count; i++) {
					cellsList[i].ObjectData.Add(this);
				}
			}
		//} else {
		//	Debug.LogError(string.Format("Object \"{0}\": wrong grid position!", gameObject.name));
		//}
		_oldGridRect = gridRect;
	}

	public void FindPath(HCCGridObject targetObject) {
		FreePath();
		Path = HCCGridController.Instance.Pathfinder.FindPath(this, targetObject);
		if (Path.Count > 0) {
			Path.RemoveAt(0);
		}
		ReservePath();
	}

	public void FindPath(Vector3 worldPosition, params HCCGridObject[] excludedObjects) {
		FindPath(HCCGridController.Instance.GridView.WorldToGridPos(worldPosition), excludedObjects);
	}

	public void FindPath(HCCGridPoint gridPoint, params HCCGridObject[] excludedObjects) {
		if(excludedObjects == null) {
			excludedObjects = new HCCGridObject[] { this };
		} else {
			HCCGridObject[] newExcluded = new HCCGridObject[excludedObjects.Length + 1];
			newExcluded[0] = this;
			for(int i = 1; i < newExcluded.Length; i++) {
				newExcluded[i] = excludedObjects[i - 1];
			}
			excludedObjects = newExcluded;
		}

		FreePath();
		Path = HCCGridController.Instance.Pathfinder.FindPath(HCCGridController.Instance.GridView.WorldToGridPos(_cachedTransform.position), gridPoint, excludedObjects);
		ReservePath();
	}

	public void PathPointReached() {
		Path[0].ObjectPathReservation = null;
		Path.RemoveAt(0);
	}

	private void ReservePath() {
		for (int i = 0; i < Path.Count; i++) {
			Path[i].ObjectPathReservation = this;
		}
	}

	private void FreePath() {
		for (int i = 0; i < Path.Count; i++) {
			Path[i].ObjectPathReservation = null;
		}
	}
}
