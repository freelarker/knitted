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
	private float _unitsZDistance = 1f;

	private MissionData _currentMissionData = null;
	private int _currentMapIndex = 0;

	public ArrayRO<BaseUnitBehaviour> AllyUnits {
		get { return _fightGraphics.AllyUnits; }
	}
	public ArrayRO<BaseUnitBehaviour> EnemyUnits {
		get { return _fightGraphics.EnemyUnits; }
	}

	private int _alliesCount = 0;
	private int _enemiesCount = 0;

	public void Awake() {
		_instance = this;

		EventsAggregator.Fight.AddListener(EFightEvent.AllyDeath, OnAllyDeath);
		EventsAggregator.Fight.AddListener(EFightEvent.EnemyDeath, OnEnemyDeath);
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

	public void OnDestroy() {
		EventsAggregator.Fight.RemoveListener(EFightEvent.AllyDeath, OnAllyDeath);
		EventsAggregator.Fight.RemoveListener(EFightEvent.EnemyDeath, OnEnemyDeath);
	}

	#region map start
	public void LoadMap() {
		MissionMapData mapData = _currentMissionData.GetMap(_currentMapIndex);
		_fightGraphics.Load(mapData);
		InitializeUnits(mapData);
	}

	private void InitializeUnits(MissionMapData mapData) {
		_alliesCount = _fightGraphics.AllyUnits.Length;
		_enemiesCount = _fightGraphics.EnemyUnits.Length;

		InitializeUnitsData(mapData);
		InitializeUnitsPositions();
		
		//WARNING! temp
		for (int i = 0; i < _fightGraphics.AllyUnits.Length; i++) {
			_fightGraphics.AllyUnits[i].Run();
		}

		for (int i = 0; i < _fightGraphics.EnemyUnits.Length; i++) {
			_fightGraphics.EnemyUnits[i].Run();
		}
	}

	private void InitializeUnitsData(MissionMapData mapData) {
		_fightGraphics.AllyUnits[_fightGraphics.AllyUnits.Length - 1].Setup(Global.Instance.Player.Heroes.Current, GameConstants.Tags.UNIT_ALLY);
		for(int i = 0; i < Global.Instance.CurrentMission.SelectedSoldiers.Length; i++) {
			_fightGraphics.AllyUnits[i].Setup(Global.Instance.CurrentMission.SelectedSoldiers[i], GameConstants.Tags.UNIT_ALLY);	//TODO: get BaseSoldier from city
		}

		BaseUnitData bud = null;
		for (int i = 0; i < mapData.Units.Length; i++) {
			bud = UnitsData.Instance.GetUnitData(mapData.Units[i]);
			if (bud is BaseHeroData) {
				_fightGraphics.EnemyUnits[i].Setup(new BaseHero(bud as BaseHeroData, 0), GameConstants.Tags.UNIT_ENEMY);	//TODO: setup enemy hero inventory
			} else {
				_fightGraphics.EnemyUnits[i].Setup(new BaseSoldier(bud as BaseSoldierData), GameConstants.Tags.UNIT_ENEMY);	//TODO: setup enemy soldier upgrades
			}
		}
	}

	private void InitializeUnitsPositions() {
		Transform unitTransform;

		float zPosition = 0f;
		for (int i = 0; i < _fightGraphics.AllyUnits.Length; i++) {

			unitTransform = _fightGraphics.AllyUnits[i].transform;
			unitTransform.parent = _allyUnitsRoot;
			unitTransform.localPosition = new Vector3(_allyStartLine.position.x - MissionsData.Instance.UnitsXPositionStartOffset - _fightGraphics.AllyUnits[i].UnitData.AttackRange, 0f, 2f - i);
			//TODO: position units by Z
		}

		for (int i = 0; i < _fightGraphics.EnemyUnits.Length; i++) {
			unitTransform = _fightGraphics.EnemyUnits[i].transform;
			unitTransform.parent = _enemyUnitsRoot;
			unitTransform.localPosition = new Vector3(_enemyStartLine.position.x + MissionsData.Instance.UnitsXPositionStartOffset + _fightGraphics.EnemyUnits[i].UnitData.AttackRange, 0f, 4f - i * 2);
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
		Debug.Log("Map complete");

		Global.Instance.Player.Heroes.Current.ResetDamageTaken();
		//NextMap();
	}

	private void MapFail() {
		Debug.Log("Map fail");

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
		EventsAggregator.Fight.Broadcast(EFightEvent.Pause);
	}

	public void Resume() {
		EventsAggregator.Fight.Broadcast(EFightEvent.Resume);
	}
	#endregion

	#region listeners
	private void OnAllyDeath() {
		_alliesCount--;
		if (_alliesCount <= 0) {
			MapFail();
		}
	}

	private void OnEnemyDeath() {
		_enemiesCount--;
		if (_enemiesCount <= 0) {
			MapComlete();
		}
	}
	#endregion
}
