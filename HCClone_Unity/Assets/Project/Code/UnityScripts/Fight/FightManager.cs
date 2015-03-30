using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightManager : MonoBehaviour {
	private static FightManager _sceneInstance = null;
	public static FightManager SceneInstance {
		get { return _sceneInstance; }
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

	[SerializeField]
	private Transform[] _allySpawnPoints;
	[SerializeField]
	private Transform[] _enemySpawnPoints;

	[SerializeField]
	private UIFight _ui;
	public UIFight UI {
		get { return _ui; }
	}

	private FightGraphics _graphics = new FightGraphics();
	private FightLogger _logger = new FightLogger();

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

	private bool _isPaused = false;
	public bool IsPaused {
		get { return _isPaused; }
	}

	public void Awake() {
		_sceneInstance = this;

		HCCGridController.Instance.Initialize((int)(-HCCGridView.Instance.ZeroPoint.x / HCCGridView.Instance.TileSize) * 2, (int)(-HCCGridView.Instance.ZeroPoint.z / HCCGridView.Instance.TileSize) * 2);

		EventsAggregator.Fight.AddListener<BaseUnitBehaviour, BaseUnitBehaviour>(EFightEvent.PerformAttack, OnUnitAttack);
		EventsAggregator.Fight.AddListener<BaseUnit>(EFightEvent.AllyDeath, OnAllyDeath);
		EventsAggregator.Fight.AddListener<BaseUnit>(EFightEvent.EnemyDeath, OnEnemyDeath);
	}

	public void Start() {
		FightCamera.AdaptMain();
		FightCamera.AdaptCanvas(GameConstants.DEFAULT_RESOLUTION_WIDTH, _ui.CanvasBG);
		Utils.UI.AdaptCanvasResolution(GameConstants.DEFAULT_RESOLUTION_WIDTH, GameConstants.DEFAULT_RESOLUTION_HEIGHT, _ui.CanvasUI);

		if (Global.Instance.CurrentMission.PlanetKey == EPlanetKey.None || Global.Instance.CurrentMission.MissionKey == EMissionKey.None) {
			//TODO: broadcast message
			Debug.LogError("Wrong mission info");
			return;
			//yield break;	//???
		}

		_currentMissionData = MissionsConfig.Instance.GetPlanet(Global.Instance.CurrentMission.PlanetKey).GetMission(Global.Instance.CurrentMission.MissionKey);

		//yield return null;	//???

		StartFightPreparations();
	}

	public void OnDestroy() {
		if (!Global.IsInitialized) {
			return;
		}

		EventsAggregator.Fight.RemoveListener<BaseUnitBehaviour, BaseUnitBehaviour>(EFightEvent.PerformAttack, OnUnitAttack);
		EventsAggregator.Fight.RemoveListener<BaseUnit>(EFightEvent.AllyDeath, OnAllyDeath);
		EventsAggregator.Fight.RemoveListener<BaseUnit>(EFightEvent.EnemyDeath, OnEnemyDeath);

		EventsAggregator.Network.RemoveListener<bool>(ENetworkEvent.FightDataCheckResponse, OnFightResultsCheckServerResponse);

		_logger.Clear();

		Global.Instance.Player.Heroes.Current.ResetDamageTaken();

		Clear();
	}

	//TODO: stop all skills

	#region map start
	public void StartFightPreparations() {
		Global.Instance.Player.Heroes.Current.ResetDamageTaken();
		LoadMap();
	}

	public void LoadMap() {
		MissionMapData mapData = _currentMissionData.GetMap(_currentMapIndex);
		_graphics.Load(mapData);
		InitializeUnits(mapData);
	}

	private void InitializeUnits(MissionMapData mapData) {
		_alliesCount = 1;	//hero
		for (int i = 0; i < Global.Instance.CurrentMission.SelectedSoldiers.Length; i++) {
			if (!Global.Instance.CurrentMission.SelectedSoldiers[i].IsDead) {
				_alliesCount++;
			}
		}
		_enemiesCount = _graphics.EnemyUnits.Length;

		InitializeUnitsData(mapData);
		InitializeUnitsPositions(_graphics.AllyUnits, _allySpawnPoints, _allyUnitsRoot);
		InitializeUnitsPositions(_graphics.EnemyUnits, _enemySpawnPoints, _enemyUnitsRoot);

		StartCoroutine(RunUnits());
	}

	private void InitializeUnitsData(MissionMapData mapData) {
		_graphics.AllyUnits[_graphics.AllyUnits.Length - 1].Setup(Global.Instance.Player.Heroes.Current, GameConstants.Tags.UNIT_ALLY, _graphics.UnitUIResource);
		for(int i = 0; i < Global.Instance.CurrentMission.SelectedSoldiers.Length; i++) {
			if (!Global.Instance.CurrentMission.SelectedSoldiers[i].IsDead) {
				_graphics.AllyUnits[i].Setup(Global.Instance.CurrentMission.SelectedSoldiers[i], GameConstants.Tags.UNIT_ALLY, _graphics.UnitUIResource);	//TODO: get BaseSoldier from city
			}
		}

		BaseUnitData bud = null;
		for (int i = 0; i < mapData.Units.Length; i++) {
			bud = UnitsConfig.Instance.GetUnitData(mapData.Units[i]);
			if (bud is BaseHeroData) {
				_graphics.EnemyUnits[i].Setup(new BaseHero(bud as BaseHeroData, 0), GameConstants.Tags.UNIT_ENEMY, _graphics.UnitUIResource);	//TODO: setup enemy hero inventory
			} else {
				_graphics.EnemyUnits[i].Setup(new BaseSoldier(bud as BaseSoldierData), GameConstants.Tags.UNIT_ENEMY, _graphics.UnitUIResource);	//TODO: setup enemy soldier upgrades
			}
		}
	}

	private void InitializeUnitsPositions(ArrayRO<BaseUnitBehaviour> units, Transform[] spawnPoints, Transform unitsRoot) {
		Transform[] order = new Transform[spawnPoints.Length];

		List<BaseUnitBehaviour> sotrtedSoldiersList = new List<BaseUnitBehaviour>();
		for (int i = 0; i < units.Length; i++) {
			if (units[i] != null) {
				units[i].transform.parent = unitsRoot;
				if (UnitsConfig.Instance.IsHero(units[i].UnitData.Data.Key)) {
					//position hero
					EItemKey rightHandWeapon = units[i].UnitData.Inventory.GetItemInSlot(EUnitEqupmentSlot.Weapon_RHand);
					EItemKey leftHandWeapon = units[i].UnitData.Inventory.GetItemInSlot(EUnitEqupmentSlot.Weapon_LHand);

					bool isMelee = true;
					if (rightHandWeapon != EItemKey.None) {
						isMelee = !ItemsConfig.Instance.IsWeaponRanged(rightHandWeapon);
					} else if (leftHandWeapon != EItemKey.None) {
						isMelee = !ItemsConfig.Instance.IsWeaponRanged(leftHandWeapon);
					}

					int positionIndex = isMelee ? 0 : 3;
					if (order[positionIndex] != null) {
						Debug.LogError("Two or more heroes of the same attack type found! position error!");
						return;
					}
					order[positionIndex] = units[i].transform;
				} else {
					//sort soldiers
					int insertIndex = sotrtedSoldiersList.Count;
					for (int j = 0; j < sotrtedSoldiersList.Count; j++) {
						if (sotrtedSoldiersList[j].UnitData.AttackRange > units[i].UnitData.AttackRange) {
							insertIndex = j;
							break;
						} else if (sotrtedSoldiersList[j].UnitData.AttackRange == units[i].UnitData.AttackRange) {
							if (sotrtedSoldiersList[j].UnitData.Health > units[i].UnitData.Health) {	//TODO: compare level
								insertIndex = j;
								break;
							}
						}
					}
					sotrtedSoldiersList.Insert(insertIndex, units[i]);
				}
			}
		}

		for (int i = 0, j = 0; i < order.Length; i++) {
			if (order[i] == null && j < sotrtedSoldiersList.Count) {
				order[i] = sotrtedSoldiersList[j].transform;
				j++;
			}

			if (order[i] != null) {
				order[i].position = spawnPoints[i].position;
			}
		}
	}

	private IEnumerator RunUnits() {
		yield return null;

		//WARNING! temp
		for (int i = 0; i < _graphics.AllyUnits.Length; i++) {
			if (_graphics.AllyUnits[i] != null) {
				_graphics.AllyUnits[i].Run();
			}
		}

		for (int i = 0; i < _graphics.EnemyUnits.Length; i++) {
			_graphics.EnemyUnits[i].Run();
		}
	}
	#endregion

	#region maps switch
	public void Withdraw() {
		Global.Instance.Network.SaveMissionFailResults();

		_logger.Clear();

		Global.Instance.Player.Resources.Fuel -= _currentMissionData.FuelLoseCost;
		Global.Instance.Player.Resources.Credits -= _currentMissionData.CreditsLoseCost;
		Global.Instance.Player.Resources.Minerals -= _currentMissionData.MineralsLoseCost;

		Global.Instance.CurrentMission.Clear();

		Clear();

		//TODO: load correct planet
		Application.LoadLevel("Planet1");
	}

	public void NextMap() {
		_currentMapIndex++;

		if (_currentMapIndex < _currentMissionData.MapsCount) {
			LoadMap();
		} else {
			MissionComplete();
		}
	}

	private void MapComlete() {
		EventsAggregator.Fight.Broadcast(EFightEvent.MapComplete);

		_graphics.Unload(false);
		
		EventsAggregator.Network.AddListener<bool>(ENetworkEvent.FightDataCheckResponse, OnFightResultsCheckServerResponse);
		Global.Instance.Network.SendFightResults(_logger.ToJSON());

		_logger.Clear();
	}

	private void MapFail() {
		EventsAggregator.Fight.Broadcast(EFightEvent.MapFail);

		Debug.Log("Map fail");

		MissionFail();
	}
	#endregion

	#region mission results
	private void MissionComplete() {
		//TODO: show win screen, remove player resources, save progress
		Global.Instance.Network.SaveMissionSuccessResults();
		Global.Instance.Player.StoryProgress.SaveProgress(Global.Instance.CurrentMission.PlanetKey, Global.Instance.CurrentMission.MissionKey);

		Global.Instance.Player.Resources.Fuel += -_currentMissionData.FuelWinCost + _currentMissionData.RewardFuel;
		Global.Instance.Player.Resources.Credits += -_currentMissionData.CreditsWinCost + _currentMissionData.RewardCredits;
		Global.Instance.Player.Resources.Minerals += -_currentMissionData.MineralsWinCost + _currentMissionData.RewardMinerals;

		Global.Instance.Player.Heroes.Current.AddExperience(_currentMissionData.RewardExperienceWin);

		//TODO: get items from server
		MissionData md = MissionsConfig.Instance.GetPlanet(Global.Instance.CurrentMission.PlanetKey).GetMission(Global.Instance.CurrentMission.MissionKey);
		for (int i = 0; i < md.RewardItems.Length; i++) {
			if (Random.Range(0, 101) < md.RewardItems[i].DropChance) {
				Global.Instance.Player.Inventory.AddItem(ItemsConfig.Instance.GetItem(md.RewardItems[i].ItemKey));
			}
		}

		Global.Instance.Player.StoryProgress.RegisterAttemptUsage(Global.Instance.CurrentMission.PlanetKey, Global.Instance.CurrentMission.MissionKey);

		UIWindowsManager.Instance.GetWindow<UIWindowBattleVictory>(EUIWindowKey.BattleVictory).Show(Global.Instance.CurrentMission.PlanetKey, Global.Instance.CurrentMission.MissionKey);

		Global.Instance.CurrentMission.Clear();
	}

	private void MissionFail() {
		//TODO: show lose screen, remove player resources, save progress
		Global.Instance.Network.SaveMissionFailResults();

		_logger.Clear();

		Global.Instance.Player.Resources.Fuel -= _currentMissionData.FuelLoseCost;
		Global.Instance.Player.Resources.Credits -= _currentMissionData.CreditsLoseCost;
		Global.Instance.Player.Resources.Minerals -= _currentMissionData.MineralsLoseCost;

		Global.Instance.Player.Heroes.Current.AddExperience(_currentMissionData.RewardExperienceLose);

		UIWindowsManager.Instance.GetWindow<UIWindowBattleDefeat>(EUIWindowKey.BattleDefeat).Show(Global.Instance.CurrentMission.PlanetKey, Global.Instance.CurrentMission.MissionKey);

		Global.Instance.CurrentMission.Clear();
	}

	public void Clear() {
		_graphics.Unload(true);
		_currentMissionData = null;
		_currentMapIndex = 0;
	}
	#endregion

	#region pause
	public void Pause() {
		_isPaused = true;
		EventsAggregator.Fight.Broadcast(EFightEvent.Pause);
		Time.timeScale = 0;
	}

	public void Resume() {
		_isPaused = false;
		EventsAggregator.Fight.Broadcast(EFightEvent.Resume);
		Time.timeScale = 1;
	}

	public void TogglePause() {
		if (!_isPaused) {
			Pause();
		} else {
			Resume();
		}
	}
	#endregion

	#region listeners
	private void OnUnitAttack(BaseUnitBehaviour attacker, BaseUnitBehaviour target) {
		_logger.LogDamage(attacker, target);
		attacker.UnitData.Attack(target.UnitData);
	}

	private void OnAllyDeath(BaseUnit unit) {
		_alliesCount--;
		if (_alliesCount <= 0) {
			MapFail();
		}
	}

	private void OnEnemyDeath(BaseUnit unit) {
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
