using UnityEngine;
using UnityEngine.UI;
using System;

[RequireComponent(typeof(Button))]
public class UIMissionIcon : MonoBehaviour {
	[SerializeField]
	private EMissionKey _missionKey = EMissionKey.None;
	private EPlanetKey _planetKey = EPlanetKey.None;

	public void Start() {
#if MISSIONS_TEST
		if (!Global.IsInitialized) {
			Global.Instance.Initialize();
		}
#endif
		
		PlanetData pd = MissionsConfig.Instance.GetPlanet(_missionKey);
		if (pd != null) {
			_planetKey = MissionsConfig.Instance.GetPlanet(_missionKey).Key;
		}

		bool missionAvailable = false;

		if (_planetKey != EPlanetKey.None && _missionKey != EMissionKey.None) {
			if (Global.Instance.Player.StoryProgress.IsMissioAvailable(_planetKey, _missionKey)) {
				missionAvailable = true;
			}
		}

		if (missionAvailable) {
			gameObject.GetComponent<Button>().onClick.AddListener(OnIconClick);
		} else {
			gameObject.GetComponent<Button>().interactable = false;
		}
	}

	private void OnIconClick() {
		if (_missionKey != EMissionKey.None) {
			//TODO: show mission preview window
		}
	}
}
