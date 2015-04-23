﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightManager : MonoBehaviour {
	private enum EFightPreparationStep {
		None = 0,
		Begin,
		MapGraphicsLoad,
		MapGraphicsLoaded,
		InitializeUnits,
		InitializeUnitsGraphics,
		UnitsGraphicsInitialized,
		UnitsInitialized,
		End
	}

	#region static
	private static FightManager _sceneInstance = null;
	public static FightManager SceneInstance {
		get { return _sceneInstance; }
	}

	private static EFightMode _fightMode = EFightMode.Campaign;
	private static MissionData _missionData = null;
	public static void Setup(EFightMode fightMode, MissionData missionData) {
		_fightMode = fightMode;
		_missionData = missionData;
	}
	#endregion

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

	private EFightStatus _status = EFightStatus.None;
	public EFightStatus Status {
		get { return _status; }
	}

	private FightGraphics _graphics = new FightGraphics();
	private FightLogger _logger = new FightLogger();

	private int _currentMapIndex = 0;
	public bool IsLastMap {
		get { return _missionData.MapsCount <= _currentMapIndex + 1; }
	}

	public BaseUnitBehaviour AllyHero {
		get { return _graphics.AllyUnits[_graphics.AllyUnits.Length - 1]; }
	}
	public ArrayRO<BaseUnitBehaviour> AllyUnits {
		get { return _graphics.AllyUnits; }
	}
	public ArrayRO<BaseUnitBehaviour> EnemyUnits {
		get { return _graphics.EnemyUnits; }
	}

	private int _alliesCount = 0;
	private int _enemiesCount = 0;

	private int _rtfUnitsAmount = 0;

	private EFightPreparationStep _fightPreparationStep = EFightPreparationStep.None;

	public void Awake() {
		_sceneInstance = this;

		HCCGridController.Instance.Initialize((int)(-HCCGridView.Instance.ZeroPoint.x / HCCGridView.Instance.TileSize) * 2, (int)(-HCCGridView.Instance.ZeroPoint.z / HCCGridView.Instance.TileSize) * 2);

		EventsAggregator.Units.AddListener<BaseUnitBehaviour>(EUnitEvent.ReadyToFight, OnUnitReadyToFight);
		EventsAggregator.Fight.AddListener<BaseUnitBehaviour, BaseUnitBehaviour>(EFightEvent.PerformAttack, OnUnitAttack);
		EventsAggregator.Fight.AddListener<BaseUnit>(EFightEvent.AllyDeath, OnAllyDeath);
		EventsAggregator.Fight.AddListener<BaseUnit>(EFightEvent.EnemyDeath, OnEnemyDeath);
	}

	public void Start() {
		FightCamera.AdaptMain();
		FightCamera.AdaptCanvas(GameConstants.DEFAULT_RESOLUTION_WIDTH, _ui.CanvasBG);
		Utils.UI.AdaptCanvasResolution(GameConstants.DEFAULT_RESOLUTION_WIDTH, GameConstants.DEFAULT_RESOLUTION_HEIGHT, _ui.CanvasUI);

		if (_missionData == null) {
			//TODO: broadcast message
			Debug.LogError("Wrong mission info");
			return;
			//yield break;	//???
		}

		//yield return null;	//???

		StartFightPreparations();
	}

	public void OnDestroy() {
		if (!Global.IsInitialized) {
			return;
		}

		EventsAggregator.Units.RemoveListener<BaseUnitBehaviour>(EUnitEvent.ReadyToFight, OnUnitReadyToFight);

		EventsAggregator.Fight.RemoveListener<BaseUnitBehaviour, BaseUnitBehaviour>(EFightEvent.PerformAttack, OnUnitAttack);
		EventsAggregator.Fight.RemoveListener<BaseUnit>(EFightEvent.AllyDeath, OnAllyDeath);
		EventsAggregator.Fight.RemoveListener<BaseUnit>(EFightEvent.EnemyDeath, OnEnemyDeath);

		EventsAggregator.Network.RemoveListener<bool>(ENetworkEvent.FightDataCheckResponse, OnFightResultsCheckServerResponse);

		_logger.Clear();

		Global.Instance.Player.Heroes.Current.ResetDamageTaken();
		Global.Instance.Player.Heroes.Current.ResetAggro();

		Clear();
	}

	#region map start
	public void StartFightPreparations() {
		_fightPreparationStep = EFightPreparationStep.Begin;
		_status = EFightStatus.Preparation;
		Global.Instance.Player.Heroes.Current.ResetDamageTaken();
		Global.Instance.Player.Heroes.Current.ResetAggro();
		StartCoroutine(LoadMap());
	}

	public IEnumerator LoadMap() {
		LoadingScreen.Instance.SetProgress(0.1f);

		_graphics.Unload(false);
		_logger.Clear();

		MissionMapData mapData = _missionData.GetMap(_currentMapIndex);

		_fightPreparationStep = EFightPreparationStep.MapGraphicsLoad;
		_graphics.Load(mapData);
		yield return null;
		_fightPreparationStep = EFightPreparationStep.MapGraphicsLoaded;

		StartCoroutine(InitializeUnits(mapData));
		while (_fightPreparationStep != EFightPreparationStep.UnitsInitialized) {
			yield return null;
		}
		_fightPreparationStep = EFightPreparationStep.End;

		StartCoroutine(PlayFightDialog());
		_ui.HideFader();
	}

	private IEnumerator InitializeUnits(MissionMapData mapData) {
		_fightPreparationStep = EFightPreparationStep.InitializeUnits;

		_rtfUnitsAmount = 0;

		_alliesCount = 1;	//hero
		for (int i = 0; i < Global.Instance.CurrentMission.SelectedSoldiers.Length; i++) {
			if (Global.Instance.CurrentMission.SelectedSoldiers[i] != null && !Global.Instance.CurrentMission.SelectedSoldiers[i].IsDead) {
				_alliesCount++;
			}
		}
		_enemiesCount = _graphics.EnemyUnits.Length;

		StartCoroutine(InitializeUnitsData(mapData));
		while (_fightPreparationStep != EFightPreparationStep.UnitsGraphicsInitialized) {
			yield return null;
		}
		InitializeUnitsPositions(_graphics.AllyUnits, _allySpawnPoints, _allyUnitsRoot);
		InitializeUnitsPositions(_graphics.EnemyUnits, _enemySpawnPoints, _enemyUnitsRoot);

		_fightPreparationStep = EFightPreparationStep.UnitsInitialized;
	}

	private IEnumerator InitializeUnitsData(MissionMapData mapData) {
		_fightPreparationStep = EFightPreparationStep.InitializeUnitsGraphics;

		float unitInitializationStep = (0.9f - 0.25f) / (_alliesCount + _enemiesCount);
		float currentLoadPercentage = 0.25f;

		_graphics.AllyUnits[_graphics.AllyUnits.Length - 1].Setup(Global.Instance.Player.Heroes.Current, GameConstants.Tags.UNIT_ALLY, _graphics.UnitUIResource);
		for(int i = 0; i < Global.Instance.CurrentMission.SelectedSoldiers.Length; i++) {
			if (!Global.Instance.CurrentMission.SelectedSoldiers[i].IsDead) {
				_graphics.AllyUnits[i].Setup(Global.Instance.CurrentMission.SelectedSoldiers[i], GameConstants.Tags.UNIT_ALLY, _graphics.UnitUIResource);

				currentLoadPercentage += unitInitializationStep;
				LoadingScreen.Instance.SetProgress(currentLoadPercentage);

				yield return null;
			}
		}

		BaseUnitData bud = null;
		for (int i = 0; i < mapData.Units.Length; i++) {
			bud = UnitsConfig.Instance.GetUnitData(mapData.Units[i]);
			if (bud is BaseHeroData) {
				_graphics.EnemyUnits[i].Setup(new BaseHero(bud as BaseHeroData, 0), GameConstants.Tags.UNIT_ENEMY, _graphics.UnitUIResource);	//TODO: setup enemy hero inventory
			} else {
				_graphics.EnemyUnits[i].Setup(new BaseSoldier(bud as BaseSoldierData, 1), GameConstants.Tags.UNIT_ENEMY, _graphics.UnitUIResource);	//TODO: setup enemy soldier upgrades
			}

			currentLoadPercentage += unitInitializationStep;
			LoadingScreen.Instance.SetProgress(currentLoadPercentage);

			yield return null;
		}

		_fightPreparationStep = EFightPreparationStep.UnitsGraphicsInitialized;
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
						if (units[i].UnitData.AttackRange > sotrtedSoldiersList[j].UnitData.AttackRange) {
							insertIndex = j;
							break;
						} else if (sotrtedSoldiersList[j].UnitData.AttackRange == units[i].UnitData.AttackRange) {
							if (units[i].UnitData.Health > sotrtedSoldiersList[j].UnitData.Health) {	//TODO: compare level
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

		_status = EFightStatus.InProgress;

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

	#region dialogues
	private IEnumerator PlayFightDialog() {
		LoadingScreen.Instance.SetProgress(1f);

		while (_rtfUnitsAmount < _alliesCount + _enemiesCount) {
			yield return null;
		}

		LoadingScreen.Instance.Hide();
		UnitDialogs.Instance.Play(_missionData.Key, _currentMapIndex, OnFightDialogPlayed);
	}

	private void OnFightDialogPlayed() {
		StartCoroutine(RunUnits());
	}
	#endregion

	#region maps switch
	public void PrepareMapSwitch() {
		StartCoroutine(MapSwitchPreparationRoutine());
	}

	private IEnumerator MapSwitchPreparationRoutine() {
		for (int i = 0; i < _graphics.AllyUnits.Length; i++) {
			if (_graphics.AllyUnits[i] != null) {
				if (!_graphics.AllyUnits[i].UnitData.IsDead) {
					_graphics.AllyUnits[i].GoToMapEnd();
				}
			}
		}

		_ui.ShowFader(2f);
		yield return new WaitForSeconds(2f);

		for (int i = 0; i < _graphics.AllyUnits.Length; i++) {
			if (_graphics.AllyUnits[i] != null) {
				if (!_graphics.AllyUnits[i].UnitData.IsDead) {
					_graphics.AllyUnits[i].StopAllActions();
				}
			}
		}

		NextMap();
	}

	public void Withdraw() {
		_status = EFightStatus.Finished;

		Global.Instance.Network.SaveMissionFailResults();

		_logger.Clear();

		Global.Instance.Player.Resources.Fuel -= _missionData.FuelLoseCost;
		Global.Instance.Player.Resources.Credits -= _missionData.CreditsLoseCost;
		Global.Instance.Player.Resources.Minerals -= _missionData.MineralsLoseCost;

		Global.Instance.CurrentMission.Clear();

		Clear();

		_status = EFightStatus.None;

		//TODO: load correct planet
		Application.LoadLevel("Planet1");
	}

	public void NextMap() {
		_currentMapIndex++;

		if (_currentMapIndex < _missionData.MapsCount) {
			StartCoroutine(LoadMap());
		} else {
			MissionComplete();
		}
	}

	private void MapComlete() {
		_status = EFightStatus.Finished;

		EventsAggregator.Fight.Broadcast(EFightEvent.MapComplete);

		EventsAggregator.Network.AddListener<bool>(ENetworkEvent.FightDataCheckResponse, OnFightResultsCheckServerResponse);
		Global.Instance.Network.SendFightResults(_logger.ToJSON());
	}

	private void MapFail() {
		_status = EFightStatus.Finished;

		EventsAggregator.Fight.Broadcast(EFightEvent.MapFail);

		Debug.Log("Map fail");

		MissionFail();
	}
	#endregion

	#region mission results
	private void MissionComplete() {
		switch (_fightMode) {
			case EFightMode.Campaign:
				//TODO: show win screen, remove player resources, save progress
				Global.Instance.Network.SaveMissionSuccessResults();
				Global.Instance.Player.StoryProgress.SaveProgress(Global.Instance.CurrentMission.PlanetKey, Global.Instance.CurrentMission.MissionKey);

				_logger.Clear();

				Global.Instance.Player.Resources.Fuel += -_missionData.FuelWinCost + _missionData.RewardFuel;
				Global.Instance.Player.Resources.Credits += -_missionData.CreditsWinCost + _missionData.RewardCredits;
				Global.Instance.Player.Resources.Minerals += -_missionData.MineralsWinCost + _missionData.RewardMinerals;

				Global.Instance.Player.Heroes.Current.AddExperience(_missionData.RewardExperienceWin);

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
				break;
			case EFightMode.PvP:
				_logger.Clear();
				Global.Instance.CurrentMission.Clear();
				Application.LoadLevel("MainMenu");
				break;
		}

	}

	private void MissionFail() {
		switch (_fightMode) {
			case EFightMode.Campaign:
				//TODO: show lose screen, remove player resources, save progress
				Global.Instance.Network.SaveMissionFailResults();

				_logger.Clear();

				Global.Instance.Player.Resources.Fuel -= _missionData.FuelLoseCost;
				Global.Instance.Player.Resources.Credits -= _missionData.CreditsLoseCost;
				Global.Instance.Player.Resources.Minerals -= _missionData.MineralsLoseCost;

				Global.Instance.Player.Heroes.Current.AddExperience(_missionData.RewardExperienceLose);

				UIWindowsManager.Instance.GetWindow<UIWindowBattleDefeat>(EUIWindowKey.BattleDefeat).Show(Global.Instance.CurrentMission.PlanetKey, Global.Instance.CurrentMission.MissionKey);

				Global.Instance.CurrentMission.Clear();
				break;
			case EFightMode.PvP:
				_logger.Clear();
				Global.Instance.CurrentMission.Clear();
				Application.LoadLevel("MainMenu");
				break;
		}
	}

	public void Clear() {
		_graphics.Unload(true);
		_currentMapIndex = 0;

		_missionData = null;
	}
	#endregion

	#region pause
	public void Pause() {
		_status = EFightStatus.Paused;

		EventsAggregator.Fight.Broadcast(EFightEvent.Pause);
		Time.timeScale = 0;
	}

	public void Resume() {
		_status = EFightStatus.InProgress;

		EventsAggregator.Fight.Broadcast(EFightEvent.Resume);
		Time.timeScale = 1;
	}

	public void TogglePause() {
		if (_status == EFightStatus.InProgress) {
			Pause();
		} else if (_status == EFightStatus.Paused) {
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

		if (!checkResult) {
			//TODO: cripped fight results - show message and return to city without saving fight results
			return;
		}

		if (IsLastMap) {
			MissionComplete();
		}
	}

	private void OnUnitReadyToFight(BaseUnitBehaviour bub) {
		_rtfUnitsAmount++;
	}
	#endregion
}
