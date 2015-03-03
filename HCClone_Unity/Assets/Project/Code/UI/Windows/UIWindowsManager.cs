using UnityEngine;
using System.Collections.Generic;

public class UIWindowsManager : MonoBehaviourSingleton<UIWindowsManager> {
	private Dictionary<EUIWindowKey, string> _windowResources = new Dictionary<EUIWindowKey, string>() {
		{ EUIWindowKey.BattlePreview, GameConstants.Paths.Prefabs.UI_WIN_BATTLE_PREVIEW },
		{ EUIWindowKey.BattleSetup, GameConstants.Paths.Prefabs.UI_WIN_BATTLE_SETUP },
		{ EUIWindowKey.BattleVictory, GameConstants.Paths.Prefabs.UI_WIN_BATTLE_VICTORY },
		{ EUIWindowKey.BattleDefeat, GameConstants.Paths.Prefabs.UI_WIN_BATTLE_DEFEAT },

		
		{ EUIWindowKey.CityBuildingUpgrade, GameConstants.Paths.Prefabs.UI_WIN_CITY_BUILDING_UPGRADE },
	};

	private Dictionary<EUIWindowKey, UIWindow> _activeWindows = new Dictionary<EUIWindowKey, UIWindow>();

	public T GetWindow<T>(EUIWindowKey windowKey) where T : UIWindow {
		UIWindow window = GetWindow(windowKey);
		return window != null ? window as T : null;
	}

	public UIWindow GetWindow(EUIWindowKey windowKey) {
		if (_activeWindows.ContainsKey(windowKey) && _activeWindows[windowKey] != null) {
			return _activeWindows[windowKey];
		}

		GameObject windowResource = UIResourcesManager.Instance.GetResource<GameObject>(_windowResources[windowKey]);
		if (windowResource != null) {
			UIWindow windowInstance = (GameObject.Instantiate(windowResource) as GameObject).GetComponent<UIWindow>();
			windowInstance.transform.SetParent(Utils.UI.GetWindowsCanvas().transform, false);

			_activeWindows.Add(windowKey, windowInstance);

			return windowInstance;
		}

		return null;
	}

	public void Clear() {
		foreach (KeyValuePair<EUIWindowKey, UIWindow> kvp in _activeWindows) {
			if (kvp.Value != null) {
				GameObject.Destroy(kvp.Value);
			}

			UIResourcesManager.Instance.FreeResource(_windowResources[kvp.Key]);
		}

		UIResourcesManager.Instance.Clear();
	}

	#region unity funcs
	public void OnLevelWasLoaded(int levelNumber) {
		Clear();
	}

	public void OnApplicationQuit() {
		Clear();
	}
	#endregion
}
