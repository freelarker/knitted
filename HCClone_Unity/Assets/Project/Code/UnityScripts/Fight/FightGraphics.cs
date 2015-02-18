using System.Collections.Generic;
using UnityEngine;

public class FightGraphics {
	private Dictionary<EUnitKey, BaseUnitBehaviour> _allyUnitsGraphicsResources = new Dictionary<EUnitKey, BaseUnitBehaviour>();
	private Dictionary<EUnitKey, BaseUnitBehaviour> _enemyUnitsGraphicsResources = new Dictionary<EUnitKey, BaseUnitBehaviour>();

	private Dictionary<EItemKey, GameObject[]> _allyItemsGraphicsResources = new Dictionary<EItemKey, GameObject[]>();
	private Dictionary<EItemKey, GameObject[]> _enemyItemsGraphicsResources = new Dictionary<EItemKey, GameObject[]>();
	
	private ArrayRO<BaseUnitBehaviour> _allyUnits = null;
	public ArrayRO<BaseUnitBehaviour> AllyUnits {
		get { return _allyUnits; }
	}

	private ArrayRO<BaseUnitBehaviour> _enemyUnits = null;
	public ArrayRO<BaseUnitBehaviour> EnemyUnits {
		get { return _enemyUnits; }
	}

	public void Load(MissionMapData mapData) {
		LoadResources(mapData);
		InstantiateGraphics(mapData);
	}

	public void Unload(bool fullUnload) {
		DestroyInstances(fullUnload);
		UnloadResources(fullUnload);
	}

	#region instances management
	private void InstantiateGraphics(MissionMapData mapData) {
		InstantiateBackground(mapData);
		InstantiateAllyUnits();
		InstantiateEnemyUnits(mapData);
	}

	private void InstantiateBackground(MissionMapData mapData) {
		//TODO: instantiate background
	}

	private void InstantiateAllyUnits() {
		//check if this is first map
		if (_allyUnits != null) {
			return;
		}

		//TODO: setup inventory

		ArrayRO<BaseSoldier> playerSoldiersList = Global.Instance.CurrentMission.SelectedSoldiers;
		BaseUnitBehaviour[] unitsList = new BaseUnitBehaviour[playerSoldiersList.Length + 1];

		BaseUnitBehaviour bub = null;

		//instantiate soldiers
		for (int i = 0; i < playerSoldiersList.Length; i++) {
			if (!playerSoldiersList[i].IsDead) {
				bub = (GameObject.Instantiate(_allyUnitsGraphicsResources[playerSoldiersList[i].Data.Key].gameObject) as GameObject).GetComponent<BaseUnitBehaviour>();
				unitsList[i] = bub;

				LoadItemsResources(_allyItemsGraphicsResources, bub, playerSoldiersList[i].Inventory.GetItemInSlot(EUnitEqupmentSlot.Weapon), playerSoldiersList[i].Inventory.GetItemInSlot(EUnitEqupmentSlot.Armor));
			}
		}

		//instantiate player
		BaseHero playerHero = Global.Instance.Player.Heroes.Current;
		if (!playerHero.IsDead) {
			bub = (GameObject.Instantiate(_allyUnitsGraphicsResources[playerHero.Data.Key].gameObject) as GameObject).GetComponent<BaseUnitBehaviour>();
			unitsList[unitsList.Length - 1] = bub;

			LoadItemsResources(_allyItemsGraphicsResources, bub, playerHero.Inventory.GetItemInSlot(EUnitEqupmentSlot.Weapon), playerHero.Inventory.GetItemInSlot(EUnitEqupmentSlot.Armor));
		}

		//save
		_allyUnits = new ArrayRO<BaseUnitBehaviour>(unitsList);
	}

	private void InstantiateEnemyUnits(MissionMapData mapData) {
		//TODO: setup inventory

		BaseUnitBehaviour[] unitsList = new BaseUnitBehaviour[mapData.Units.Length];

		BaseUnitData bud = null;
		BaseUnitBehaviour bub = null;

		//instantiate soldiers
		for (int i = 0; i < mapData.Units.Length; i++) {
			bub = (GameObject.Instantiate(_enemyUnitsGraphicsResources[mapData.Units[i]].gameObject) as GameObject).GetComponent<BaseUnitBehaviour>();
			unitsList[i] = bub;

			bud = UnitsConfig.Instance.GetUnitData(mapData.Units[i]);
			LoadItemsResources(_enemyItemsGraphicsResources, bub, bud.GetBaseItemInSlot(EUnitEqupmentSlot.Weapon), bud.GetBaseItemInSlot(EUnitEqupmentSlot.Armor));
		}

		//save
		_enemyUnits = new ArrayRO<BaseUnitBehaviour>(unitsList);
	}
	#endregion

	#region resources management
	private void LoadResources(MissionMapData mapData) {
		LoadBackgroundResources(mapData);
		LoadAllyUnitsResources();
		LoadEnemyUnitsResources(mapData);
	}

	private void LoadBackgroundResources(MissionMapData mapData) {
		//TODO: load background resources
	}

	private void LoadAllyUnitsResources() {
		//check if this is first map
		if (_allyUnitsGraphicsResources.Count > 0) {
			return;
		}

		ArrayRO<BaseSoldier> playerSoldiersList = Global.Instance.CurrentMission.SelectedSoldiers;

		BaseUnitData bud = null;
		BaseUnitBehaviour bub = null;

		//load unique soldiers
		for (int i = 0; i < playerSoldiersList.Length; i++) {
			if (_allyUnitsGraphicsResources.ContainsKey(playerSoldiersList[i].Data.Key)) {
				continue;
			}

			bud = UnitsConfig.Instance.GetSoldierData(playerSoldiersList[i].Data.Key);
			if (bud != null && !bud.PrefabName.Equals(string.Empty)) {
				bub = LoadUnitResource<BaseUnitBehaviour>(string.Format("{0}/{1}", GameConstants.Paths.UNITS_PERFABS, bud.PrefabName));
				_allyUnitsGraphicsResources.Add(playerSoldiersList[i].Data.Key, bub);
			} else {
				Debug.LogError("Can't load unit graphics: " + playerSoldiersList[i].Data.Key);
			}
		}

		//load hero
		BaseHero playerHero = Global.Instance.Player.Heroes.Current;
		if (playerHero == null || playerHero.Data.PrefabName.Equals(string.Empty)) {
			Debug.LogError("Can't load unit graphics for player hero");
		}

		bub = LoadUnitResource<BaseUnitBehaviour>(string.Format("{0}/{1}", GameConstants.Paths.UNITS_PERFABS, playerHero.Data.PrefabName));
		_allyUnitsGraphicsResources.Add(playerHero.Data.Key, bub);
	}

	private void LoadEnemyUnitsResources(MissionMapData mapData) {
		BaseUnitData bud = null;
		BaseUnitBehaviour bub = null;

		//load unique units
		for (int i = 0; i < mapData.Units.Length; i++) {
			if (_enemyUnitsGraphicsResources.ContainsKey(mapData.Units[i])) {
				continue;
			}

			bud = UnitsConfig.Instance.GetUnitData(mapData.Units[i]);
			if (bud != null && !bud.PrefabName.Equals(string.Empty)) {
				bub = LoadUnitResource<BaseUnitBehaviour>(string.Format("{0}/{1}", GameConstants.Paths.UNITS_PERFABS, bud.PrefabName));
				_enemyUnitsGraphicsResources.Add(mapData.Units[i], bub);
			} else {
				Debug.LogError("Can't load unit graphics: " + mapData.Units[i]);
			}
		}
	}

	private void LoadItemsResources(Dictionary<EItemKey, GameObject[]> resourcesDic, BaseUnitBehaviour bub, EItemKey weaponKey, EItemKey armorKey) {
		if (bub.ModelView != null) {
			GameObject[] weaponResources = null;
			GameObject[] armorResources = null;

			if (weaponKey == EItemKey.None) {
				Debug.LogError(string.Format("No weapon set for {0} unit", bub.UnitData.Data.Key));
			} else if (!resourcesDic.ContainsKey(weaponKey)) {
				weaponResources = new GameObject[1];
				weaponResources[0] = Resources.Load(string.Format("{0}/{1}", GameConstants.Paths.ITEMS_PERFABS, weaponKey)) as GameObject;
				resourcesDic.Add(weaponKey, weaponResources);
			} else {
				weaponResources = resourcesDic[weaponKey];
			}


			if (armorKey == EItemKey.None) {
				Debug.LogError(string.Format("No armor set for {0} unit", bub.UnitData.Data.Key));
			} else if (!resourcesDic.ContainsKey(armorKey)) {
				armorResources = new GameObject[2];
				armorResources[0] = Resources.Load(string.Format("{0}/{1}_head", GameConstants.Paths.ITEMS_PERFABS, armorKey)) as GameObject;
				armorResources[1] = Resources.Load(string.Format("{0}/{1}_body", GameConstants.Paths.ITEMS_PERFABS, armorKey)) as GameObject;
				resourcesDic.Add(armorKey, armorResources);
			} else {
				armorResources = resourcesDic[armorKey];
			}

			bub.ModelView.Setup(weaponResources[0], armorResources[0], armorResources[1]);
		}
	}

	private void DestroyInstances(bool fullUnload) {
		//destroy ally units
		if (fullUnload && _allyUnits != null) {
			for (int i = 0; i < _allyUnits.Length; i++) {
				if (_allyUnits[i] != null) {
					GameObject.Destroy(_allyUnits[i].gameObject);
				}
			}
			_allyUnits = null;
		}

		//destroy enemy units
		if (_enemyUnits != null) {
			for (int i = 0; i < _enemyUnits.Length; i++) {
				if (_enemyUnits[i] != null) {
					GameObject.Destroy(_enemyUnits[i].gameObject);
				}
			}
			_enemyUnits = null;
		}

		//TODO: destroy background
	}

	private void UnloadResources(bool fullUnload) {
		if (fullUnload) {
			_allyUnitsGraphicsResources.Clear();
			_allyItemsGraphicsResources.Clear();
		}
		_enemyUnitsGraphicsResources.Clear();
		_enemyItemsGraphicsResources.Clear();
		//TODO: free background resources

		Resources.UnloadUnusedAssets();
	}

	private T LoadUnitResource<T>(string path) where T : MonoBehaviour {
		GameObject go = Resources.Load(path) as GameObject;
		return go.GetComponent<T>();
	}
	#endregion
}
