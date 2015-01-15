using UnityEngine;

public class FightManager : MonoBehaviour {
	private static FightManager _instance = null;
	public static FightManager Instance {
		get { return _instance; }
	}

	[SerializeField]
	private Transform _allyUnitsRoot;
	[SerializeField]
	private Transform _enemyUnitsRoot;

	[SerializeField]
	private Transform _allyStartLine;
	public Transform AllyStartLine {
		get { return _allyStartLine; }
	}
	[SerializeField]
	private Transform _enemyStartLine;
	public Transform EnemyStartLine {
		get { return _enemyStartLine; }
	}

	private FightGraphics _fightGraphics = new FightGraphics();

	private MissionData _currentMissionData = null;
	private int _currentMapIndex = 0;

	public ArrayRO<BaseUnitBehaviour> AllyUnits {
		get { return _fightGraphics.AllyUnits; }
	}
	public ArrayRO<BaseUnitBehaviour> EnemyUnits {
		get { return _fightGraphics.EnemyUnits; }
	}

	public void Awake() {
		_instance = this;
	}

	public void Start() {
		if (Global.Instance.CurrentMission.PlanetKey == EPlanetKey.None || Global.Instance.CurrentMission.MissionKey == EMissionKey.None) {
			//TODO: broadcast message
			Debug.LogError("Wrong mission info");
			return;
		}

		_currentMissionData = MissionsData.Instance.GetPlanet(Global.Instance.CurrentMission.PlanetKey).GetMission(Global.Instance.CurrentMission.MissionKey);

		LoadMap();
	}

	#region map start
	public void LoadMap() {
		_fightGraphics.Load(_currentMissionData.GetMap(_currentMapIndex));
		InitializeUnitPositions();

		//WARNING! temp
		for (int i = 0; i < _fightGraphics.AllyUnits.Length; i++) {
			_fightGraphics.AllyUnits[i].Run();
		}

		for (int i = 0; i < _fightGraphics.EnemyUnits.Length; i++) {
			_fightGraphics.EnemyUnits[i].Run();
		}
	}

	private void InitializeUnitPositions() {
		Transform unitTransform;

		for (int i = 0; i < _fightGraphics.AllyUnits.Length; i++) {
			unitTransform = _fightGraphics.AllyUnits[i].transform;
			unitTransform.parent = _allyUnitsRoot;
			unitTransform.localPosition = new Vector3(_allyStartLine.position.x - MissionsData.Instance.UnitsXPositionStartOffset - _fightGraphics.AllyUnits[i].UnitData.AttackRange, 0f, 1f);
			//TODO: position units by Z
		}

		for (int i = 0; i < _fightGraphics.EnemyUnits.Length; i++) {
			unitTransform = _fightGraphics.EnemyUnits[i].transform;
			unitTransform.parent = _enemyUnitsRoot;
			unitTransform.localPosition = new Vector3(_enemyStartLine.position.x + MissionsData.Instance.UnitsXPositionStartOffset + _fightGraphics.EnemyUnits[i].UnitData.AttackRange, 0f, 1f);
			//TODO: position units by Z
		}
	}
	#endregion

	#region maps switch
	public void NextMap() {
		_currentMapIndex++;

		if (_currentMapIndex < _currentMissionData.MapsCount) {
			LoadMap();
		} else {
			MissionComplete();
		}
	}

	private void MapComlete() {
		Global.Instance.Player.Heroes.Current.ResetDamageTaken();
		NextMap();
	}

	private void MapFail() {
		Global.Instance.Player.Heroes.Current.ResetDamageTaken();
		MissionFail();
	}
	#endregion

	#region mission results
	private void MissionComplete() {

	}

	private void MissionFail() {

	}
	#endregion

	#region pause
	public void Pause() {
		//TODO: broadcast pause
	}

	public void Resume() {
		//TODO: broadcast resume
	}
	#endregion
}
