using UnityEngine;
using System.Collections.Generic;
using System;

public class HCCPathfinder {
	private HCCGridData _gridData = null;

	private HCCCell[,] _closedSet = null;
	private HCCCell[,] _cameFrom = null;
	private float[,] _gScore = null;
	private float[,] _fScore = null;

	private Vector2 v1 = Vector2.zero;
	private Vector2 v2 = Vector2.zero;

	public HCCPathfinder(HCCGridData gridData) {
		_gridData = gridData;

		_closedSet = new HCCCell[_gridData.XSize, _gridData.ZSize];
		_cameFrom = new HCCCell[_gridData.XSize, _gridData.ZSize];
		_gScore = new float[_gridData.XSize, _gridData.ZSize];
		_fScore = new float[_gridData.XSize, _gridData.ZSize];
	}

	public List<HCCCell> FindPath(HCCGridObject self, HCCGridObject target) {
		HCCGridRect selfGridRect = self.GridRect;

		HCCGridPoint start = HCCGridController.Instance.GridView.WorldToGridPos(self.transform.position);
		HCCGridPoint end = HCCGridController.Instance.GridView.WorldToGridPos(target.transform.position);

		List<HCCCell> openSet = new List<HCCCell>() { _gridData.GetCell(start.X, start.Z) };
		_gScore[start.X, start.Z] = 0f;
		_fScore[start.X, start.Z] = _gScore[start.X, start.Z] + HeuristicCost(start, end);

		HCCCell current;
		while (openSet.Count > 0) {
			int currentIndex = GetLowestFScoreIndex(openSet);
			current = openSet[currentIndex];
			if (current.GridPosition.X == end.X && current.GridPosition.Z == end.Z) {
				List<HCCCell> result = ReconstructPath(current.GridPosition);
				Cleanup();
				return result;
			}

			openSet.RemoveAt(currentIndex);
			_closedSet[current.GridPosition.X, current.GridPosition.Z] = current;

			for (int i = current.GridPosition.X - 1; i <= current.GridPosition.X + 1; i++) {
				for (int j = current.GridPosition.Z - 1; j <= current.GridPosition.Z + 1; j++) {
					if (i == current.GridPosition.X && j == current.GridPosition.Z) {
						continue;
					}

					HCCCell neighbour = _gridData.GetCell(i, j);
					if (neighbour == null || _closedSet[neighbour.GridPosition.X, neighbour.GridPosition.Z] != null) {
						continue;
					}

					//project update 1: pass static objects
					//if (neighbour.IsBusyByStaticObject) {
					//	_closedSet[neighbour.GridPosition.X, neighbour.GridPosition.Z] = neighbour;
					//	continue;
					//}

					//project update 2: pass all objects except self and target
					bool blockNeighbour = false;
					selfGridRect = self.GridRectAtPosition(HCCGridController.Instance.GridView.GridToWorldPos(neighbour.GridPosition));
					for (int q = selfGridRect.XMin; q <= selfGridRect.XMax; q++) {
						for (int k = selfGridRect.ZMin; k <= selfGridRect.ZMax; k++) {
							HCCCell cellData = _gridData.GetCell(q, k);
							if (cellData != null && cellData.ObjectData != null) {
								if (cellData.ObjectData != self && cellData.ObjectData != target) {
									blockNeighbour = true;
									break;
								}
							}
						}
						if (blockNeighbour) {
							break;
						}
					}
					if (blockNeighbour) {
						_closedSet[neighbour.GridPosition.X, neighbour.GridPosition.Z] = neighbour;
						continue;
					}
					//if (neighbour.ObjectData != null) {
					//	if (neighbour.ObjectData != self && neighbour.ObjectData != target) {
					//		_closedSet[neighbour.GridPosition.X, neighbour.GridPosition.Z] = neighbour;
					//		continue;
					//	}
					//}
					
					//project update 3: pass all paths except self and target
					//if (neighbour.ObjectPathReservation != null) {
					//	if (neighbour.ObjectPathReservation != self || neighbour.ObjectPathReservation != target) {
					//		_closedSet[neighbour.GridPosition.X, neighbour.GridPosition.Z] = neighbour;
					//		continue;
					//	}
					//}

					float tentativeGScore = _gScore[current.GridPosition.X, current.GridPosition.Z] + HeuristicCost(current.GridPosition, neighbour.GridPosition);
					if (openSet.IndexOf(neighbour) == -1 || tentativeGScore < _gScore[neighbour.GridPosition.X, neighbour.GridPosition.Z]) {
						_cameFrom[neighbour.GridPosition.X, neighbour.GridPosition.Z] = current;
						_gScore[neighbour.GridPosition.X, neighbour.GridPosition.Z] = tentativeGScore;
						_fScore[neighbour.GridPosition.X, neighbour.GridPosition.Z] = _gScore[neighbour.GridPosition.X, neighbour.GridPosition.Z] + HeuristicCost(neighbour.GridPosition, end);
						if (openSet.IndexOf(neighbour) == -1) {
							openSet.Add(neighbour);
						}
					}
				}
			}
		}

		Cleanup();
		return new List<HCCCell>();
	}

	//working, simple
	public List<HCCCell> FindPath(HCCGridPoint start, HCCGridPoint end, HCCGridObject[] excludedObjects) {
		List<HCCCell> openSet = new List<HCCCell>() { _gridData.GetCell(start.X, start.Z) };
		_gScore[start.X, start.Z] = 0f;
		_fScore[start.X, start.Z] = _gScore[start.X, start.Z] + HeuristicCost(start, end);

		HCCCell current;
		while (openSet.Count > 0) {
			int currentIndex = GetLowestFScoreIndex(openSet);
			current = openSet[currentIndex];
			if (current.GridPosition.X == end.X && current.GridPosition.Z == end.Z) {
				List<HCCCell> result = ReconstructPath(current.GridPosition);
				Cleanup();
				return result;
			}

			openSet.RemoveAt(currentIndex);
			_closedSet[current.GridPosition.X, current.GridPosition.Z] = current;

			for (int i = current.GridPosition.X - 1; i <= current.GridPosition.X + 1; i++) {
				for (int j = current.GridPosition.Z - 1; j <= current.GridPosition.Z + 1; j++) {
					if (i == current.GridPosition.X && j == current.GridPosition.Z) {
						continue;
					}

					HCCCell neighbour = _gridData.GetCell(i, j);
					if (neighbour == null || _closedSet[neighbour.GridPosition.X, neighbour.GridPosition.Z] != null) {
						continue;
					}

					//project update 1: pass static objects
					//if (neighbour.IsBusyByStaticObject) {
					//	_closedSet[neighbour.GridPosition.X, neighbour.GridPosition.Z] = neighbour;
					//	continue;
					//}

					//project update 2: pass all objects except excluded
					if (excludedObjects != null && neighbour.ObjectData != null) {
						bool objectIsExcluded = false;
						for (int p = 0; p < excludedObjects.Length; p++) {
							if (neighbour.ObjectData == excludedObjects[p]) {
								objectIsExcluded = true;
								break;
							}
						}
						if (!objectIsExcluded) {
							_closedSet[neighbour.GridPosition.X, neighbour.GridPosition.Z] = neighbour;
							continue;
						}
					}

					float tentativeGScore = _gScore[current.GridPosition.X, current.GridPosition.Z] + HeuristicCost(current.GridPosition, neighbour.GridPosition);
					if (openSet.IndexOf(neighbour) == -1 || tentativeGScore < _gScore[neighbour.GridPosition.X, neighbour.GridPosition.Z]) {
						_cameFrom[neighbour.GridPosition.X, neighbour.GridPosition.Z] = current;
						_gScore[neighbour.GridPosition.X, neighbour.GridPosition.Z] = tentativeGScore;
						_fScore[neighbour.GridPosition.X, neighbour.GridPosition.Z] = _gScore[neighbour.GridPosition.X, neighbour.GridPosition.Z] + HeuristicCost(neighbour.GridPosition, end);
						if (openSet.IndexOf(neighbour) == -1) {
							openSet.Add(neighbour);
						}
					}
				}
			}
		}

		Cleanup();
		return new List<HCCCell>();
	}

	/*
	public List<HCCCell> FindPath(HCCGridPoint start, HCCGridPoint end, HCCGridRect startRect, params HCCGridObject[] excludedObjects) {
		int xSize = startRect.XSize;
		int zSize = startRect.ZSize;
		int xMinus = start.X - startRect.XMin;
		int zMinus = start.Z - startRect.ZMin;

		List<HCCCell> openSet = new List<HCCCell>() { _gridData.GetCell(start.X, start.Z) };
		_gScore[start.X, start.Z] = 0f;
		_fScore[start.X, start.Z] = _gScore[start.X, start.Z] + HeuristicCost(start, end);

		HCCCell current;
		while (openSet.Count > 0) {
			int currentIndex = GetLowestFScoreIndex(openSet);
			current = openSet[currentIndex];

			for (int q = current.GridPosition.X - xMinus; q < current.GridPosition.X - xMinus + xSize; q++) {
				for (int k = current.GridPosition.Z - zMinus; k < current.GridPosition.Z - zMinus + zSize; k++) {
					if (q == end.X && k == end.Z) {
						List<HCCCell> result = ReconstructPath(current.GridPosition);
						Cleanup();
						return result;
					}
				}
			}

			openSet.RemoveAt(currentIndex);
			_closedSet[current.GridPosition.X, current.GridPosition.Z] = current;

			for (int i = current.GridPosition.X - 1; i <= current.GridPosition.X + 1; i++) {
				for (int j = current.GridPosition.Z - 1; j <= current.GridPosition.Z + 1; j++) {
					if (i == current.GridPosition.X && j == current.GridPosition.Z) {
						continue;
					}

					HCCCell neighbour = _gridData.GetCell(i, j);
					//check null or in closed set already
					if (neighbour == null || _closedSet[neighbour.GridPosition.X, neighbour.GridPosition.Z] != null) {
						continue;
					}
					//check at the end of the grid
					if (neighbour.GridPosition.X - xMinus >= _gridData.XSize - xSize || neighbour.GridPosition.Z - zMinus >= _gridData.ZSize - zSize) {
						_closedSet[neighbour.GridPosition.X, neighbour.GridPosition.Z] = neighbour;
						continue;
					}

					//check blocked by something
					bool isBlockedBySomething = false;
					for (int q = neighbour.GridPosition.X - xMinus; q < neighbour.GridPosition.X - xMinus + xSize; q++) {
						for (int k = neighbour.GridPosition.Z - zMinus; k < neighbour.GridPosition.Z - zMinus + zSize; k++) {
							if (_gridData.GetCell(q, k).ObjectData != null) {
								bool objectIsExcluded = false;
								for (int p = 0; p < excludedObjects.Length; p++) {
									if (excludedObjects[p] == neighbour.ObjectData) {
										objectIsExcluded = true;
										break;
									}
								}
								if (!objectIsExcluded) {
									isBlockedBySomething = true;
									break;
								}
							}
						}
						if (isBlockedBySomething) {
							break;
						}
					}
					if (isBlockedBySomething) {
						_closedSet[neighbour.GridPosition.X, neighbour.GridPosition.Z] = neighbour;
						continue;
					}

					float tentativeGScore = _gScore[current.GridPosition.X, current.GridPosition.Z] + HeuristicCost(current.GridPosition, neighbour.GridPosition);
					if (openSet.IndexOf(neighbour) == -1 || tentativeGScore < _gScore[neighbour.GridPosition.X, neighbour.GridPosition.Z]) {
						_cameFrom[neighbour.GridPosition.X, neighbour.GridPosition.Z] = current;
						_gScore[neighbour.GridPosition.X, neighbour.GridPosition.Z] = tentativeGScore;
						_fScore[neighbour.GridPosition.X, neighbour.GridPosition.Z] = _gScore[neighbour.GridPosition.X, neighbour.GridPosition.Z] + HeuristicCost(neighbour.GridPosition, end);
						if (openSet.IndexOf(neighbour) == -1) {
							openSet.Add(neighbour);
						}
					}
				}
			}
		}

		Cleanup();
		return new List<HCCCell>();
	}

	public List<HCCCell> FindPath(HCCGridRect start, HCCGridPoint end, params HCCGridObject[] excludedObjects) {
		int xSize = start.XSize;
		int zSize = start.ZSize;

		List<HCCCell> openSet = new List<HCCCell>() { _gridData.GetCell(start.XMin, start.ZMin) };
		_gScore[start.XMin, start.ZMin] = 0f;
		_fScore[start.XMin, start.ZMin] = _gScore[start.XMin, start.ZMin] + HeuristicCost(start.Min, end);

		HCCCell current;
		while (openSet.Count > 0) {
			int currentIndex = GetLowestFScoreIndex(openSet);
			current = openSet[currentIndex];

			for (int q = current.GridPosition.X; q < current.GridPosition.X + xSize; q++) {
				for (int k = current.GridPosition.Z; k < current.GridPosition.Z + zSize; k++) {
					if (q == end.X && k == end.Z) {
						List<HCCCell> result = ReconstructPath(current.GridPosition);
						Cleanup();
						return result;
					}
				}
			}

			openSet.RemoveAt(currentIndex);
			_closedSet[current.GridPosition.X, current.GridPosition.Z] = current;

			for (int i = current.GridPosition.X - 1; i <= current.GridPosition.X + 1; i++) {
				for (int j = current.GridPosition.Z - 1; j <= current.GridPosition.Z + 1; j++) {
					if (i == current.GridPosition.X && j == current.GridPosition.Z) {
						continue;
					}

					HCCCell neighbour = _gridData.GetCell(i, j);
					//check null or in closed set already
					if (neighbour == null || _closedSet[neighbour.GridPosition.X, neighbour.GridPosition.Z] != null) {
						continue;
					}
					//check at the end of the grid
					if (neighbour.GridPosition.X >= _gridData.XSize - xSize || neighbour.GridPosition.Z >= _gridData.ZSize - zSize) {
						_closedSet[neighbour.GridPosition.X, neighbour.GridPosition.Z] = neighbour;
						continue;
					}

					//check blocked by something
					bool isBlockedBySomething = false;
					for (int q = neighbour.GridPosition.X; q < neighbour.GridPosition.X + xSize; q++) {
						for (int k = neighbour.GridPosition.Z; k < neighbour.GridPosition.Z + zSize; k++) {
							if (_gridData.GetCell(q, k).ObjectData != null) {
								bool objectIsExcluded = false;
								for (int p = 0; p < excludedObjects.Length; p++) {
									if (excludedObjects[p] == neighbour.ObjectData) {
										objectIsExcluded = true;
										break;
									}
								}
								if (!objectIsExcluded) {
									isBlockedBySomething = true;
									break;
								}
							}
						}
						if (isBlockedBySomething) {
							break;
						}
					}
					if (isBlockedBySomething) {
						_closedSet[neighbour.GridPosition.X, neighbour.GridPosition.Z] = neighbour;
						continue;
					}

					float tentativeGScore = _gScore[current.GridPosition.X, current.GridPosition.Z] + HeuristicCost(current.GridPosition, neighbour.GridPosition);
					if (openSet.IndexOf(neighbour) == -1 || tentativeGScore < _gScore[neighbour.GridPosition.X, neighbour.GridPosition.Z]) {
						_cameFrom[neighbour.GridPosition.X, neighbour.GridPosition.Z] = current;
						_gScore[neighbour.GridPosition.X, neighbour.GridPosition.Z] = tentativeGScore;
						_fScore[neighbour.GridPosition.X, neighbour.GridPosition.Z] = _gScore[neighbour.GridPosition.X, neighbour.GridPosition.Z] + HeuristicCost(neighbour.GridPosition, end);
						if (openSet.IndexOf(neighbour) == -1) {
							openSet.Add(neighbour);
						}
					}
				}
			}
		}

		Cleanup();
		return new List<HCCCell>();
	}
	*/

	private float HeuristicCost(HCCGridPoint p1, HCCGridPoint p2) {
		return HeuristicCost(p1.X, p1.Z, p2.X, p2.Z);
	}

	private float HeuristicCost(int x1, int z1, int x2, int z2) {
		v1.x = x1;
		v1.y = z1;

		v2.x = x2;
		v2.y = z2;

		return Vector2.Distance(v1, v2);
	}

	private int GetLowestFScoreIndex(List<HCCCell> openSet) {
		int index = 0;
		for (int i = 1; i < openSet.Count; i++) {
			if (_fScore[openSet[i].GridPosition.X, openSet[i].GridPosition.Z] < _fScore[openSet[index].GridPosition.X, openSet[index].GridPosition.Z]) {
				index = i;
			}
		}
		return index;
	}

	private List<HCCCell> ReconstructPath(HCCGridPoint current) {
		List<HCCCell> result = new List<HCCCell>() { _gridData.GetCell(current.X, current.Z) };

		while(_cameFrom[current.X, current.Z] != null) {
			current = _cameFrom[current.X, current.Z].GridPosition;
			result.Add(_gridData.GetCell(current.X, current.Z));
		}
		result.Reverse();

		return result;
	}

	private void Cleanup() {
		for (int i = 0; i < _closedSet.GetLength(0); i++) {
			for (int j = 0; j < _closedSet.GetLength(1); j++) {
				_closedSet[i, j] = null;
				_cameFrom[i, j] = null;
				_gScore[i, j] = 0f;
				_fScore[i, j] = 0f;
			}
		}
	}
}
