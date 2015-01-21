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

	private FightGraphics _graphics = new FightGraphics();
	private FightLogger _logger = new FightLogger();

	private float _unitsZDistance = 1f;

	private MissionData _currentMissionData = null;
	private int _currentMapIndex = 0;

	public ArrayRO<BaseUnitBehaviour> AllyUnits {
		get { return _graphics.AllyUnits; }
	}
	public ArrayRO<BaseUnitBehaviour> EnemyUnits {
		get { return _graphics.EnemyUnits; }
	}

	private int _alliesCount = 0;
	private int _enemiesCount = 0;

	public void Awake() {
		_instance = this;

		EventsAggregator.Fight.AddListener<BaseUnitBehaviour, BaseUnitBehaviour>(EFightEvent.PerformAttack, OnUnitAttack);
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
		EventsAggregator.Fight.RemoveListener<BaseUnitBehaviour, BaseUnitBehaviour>(EFightEvent.PerformAttack, OnUnitAttack);
		EventsAggregator.Fight.RemoveListener(EFightEvent.AllyDeath, OnAllyDeath);
		EventsAggregator.Fight.RemoveListener(EFightEvent.EnemyDeath, OnEnemyDeath);

		EventsAggregator.Network.RemoveListener<bool>(ENetworkEvent.FightDataCheckResponse, OnFightResultsCheckServerResponse);

		_logger.Clear();
		Global.Instance.Player.Heroes.Current.ResetDamageTaken();
		Global.Instance.CurrentMission.Clear();
	}

	#region map start
	public void LoadMap() {
		MissionMapData mapData = _currentMissionData.GetMap(_currentMapIndex);
		_graphics.Load(mapData);
		InitializeUnits(mapData);
	}

	private void InitializeUnits(MissionMapData mapData) {
		_alliesCount = _graphics.AllyUnits.Length;
		_enemiesCount = _graphics.EnemyUnits.Length;

		InitializeUnitsData(mapData);
		InitializeUnitsPositions();
		
		//WARNING! temp
		for (int i = 0; i < _graphics.AllyUnits.Length; i++) {
			_graphics.AllyUnits[i].Run();
		}

		for (int i = 0; i < _graphics.EnemyUnits.Length; i++) {
			_graphics.EnemyUnits[i].Run();
		}
	}

	private void InitializeUnitsData(MissionMapData mapData) {
		_graphics.AllyUnits[_graphics.AllyUnits.Length - 1].Setup(Global.Instance.Player.Heroes.Current, GameConstants.Tags.UNIT_ALLY);
		for(int i = 0; i < Global.Instance.CurrentMission.SelectedSoldiers.Length; i++) {
			_graphics.AllyUnits[i].Setup(Global.Instance.CurrentMission.SelectedSoldiers[i], GameConstants.Tags.UNIT_ALLY);	//TODO: get BaseSoldier from city
		}

		BaseUnitData bud = null;
		for (int i = 0; i < mapData.Units.Length; i++) {
			bud = UnitsData.Instance.GetUnitData(mapData.Units[i]);
			if (bud is BaseHeroData) {
				_graphics.EnemyUnits[i].Setup(new BaseHero(bud as BaseHeroData, 0), GameConstants.Tags.UNIT_ENEMY);	//TODO: setup enemy hero inventory
			} else {
				_graphics.EnemyUnits[i].Setup(new BaseSoldier(bud as BaseSoldierData), GameConstants.Tags.UNIT_ENEMY);	//TODO: setup enemy soldier upgrades
			}
		}
	}

	private void InitializeUnitsPositions() {
		Transform unitTransform;

		for (int i = 0; i < _graphics.AllyUnits.Length; i++) {

			unitTransform = _graphics.AllyUnits[i].transform;
			unitTransform.parent = _allyUnitsRoot;
			unitTransform.localPosition = new Vector3(_allyStartLine.position.x - MissionsData.Instance.UnitsXPositionStartOffset - _graphics.AllyUnits[i].UnitData.AttackRange, 0f, 2f - i * _unitsZDistance);
			//TODO: position units by Z
		}

		for (int i = 0; i < _graphics.EnemyUnits.Length; i++) {
			unitTransform = _graphics.EnemyUnits[i].transform;
			unitTransform.parent = _enemyUnitsRoot;
			unitTransform.localPosition = new Vector3(_enemyStartLine.position.x + MissionsData.Instance.UnitsXPositionStartOffset + _graphics.EnemyUnits[i].UnitData.AttackRange, 0f, 4f - i * _unitsZDistance * 2);
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

		_graphics.Unload(false);
		_logger.Clear();
		Global.Instance.Player.Heroes.Current.ResetDamageTaken();

		EventsAggregator.Network.AddListener<bool>(ENetworkEvent.FightDataCheckResponse, OnFightResultsCheckServerResponse);
		Global.Instance.Network.SendFightResults(_logger.ToJSON());
	}

	private void MapFail() {
		Debug.Log("Map fail");

		Global.Instance.Player.Heroes.Current.ResetDamageTaken();
		MissionFail();
	}
	#endregion

	#region mission results
	private void MissionComplete() {
		//TODO: show win screen, remove player resources, save progress
		Global.Instance.Network.SaveMissionSuccessResults();

		Global.Instance.Player.Resources.Fuel += -_currentMissionData.FuelWinCost + _currentMissionData.RewardFuel;
		Global.Instance.Player.Resources.Credits += -_currentMissionData.CreditsWinCost + _currentMissionData.RewardCredits;
		Global.Instance.Player.Resources.Minerals += -_currentMissionData.MineralsWinCost + _currentMissionData.RewardMinerals;

		_graphics.Unload(true);
		_currentMissionData = null;
		_currentMapIndex = 0;

		Debug.Log("Mission complete");
	}

	private void MissionFail() {
		//TODO: show lose screen, remove player resources, save progress
		Global.Instance.Network.SaveMissionFailResults();

		Global.Instance.Player.Resources.Fuel -= _currentMissionData.FuelLoseCost;
		Global.Instance.Player.Resources.Credits -= _currentMissionData.CreditsLoseCost;
		Global.Instance.Player.Resources.Minerals -= _currentMissionData.MineralsLoseCost;

		_graphics.Unload(true);
		_currentMissionData = null;
		_currentMapIndex = 0;

		Debug.Log("Mission fail");
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
	private void OnUnitAttack(BaseUnitBehaviour attacker, BaseUnitBehaviour target) {
		_logger.LogDamage(attacker, target);
		target.UnitData.ApplyDamage(attacker.UnitData.Damage);
	}

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

	private void OnFightResultsCheckServerResponse(bool checkResult) {
		EventsAggregator.Network.RemoveListener<bool>(ENetworkEvent.FightDataCheckResponse, OnFightResultsCheckServerResponse);

		if (checkResult) {
			NextMap();
		} else {
			//TODO: cripped fight results - show message and return to city without saving fight results
		}
	}
	#endregion
}
