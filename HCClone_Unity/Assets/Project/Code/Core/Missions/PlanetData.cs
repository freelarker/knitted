using UnityEngine;

[System.Serializable]
public class PlanetData {
	[SerializeField]
	private EPlanetKey _key = EPlanetKey.None;
	public EPlanetKey Key {
		get { return _key; }
	}

	[SerializeField]
	private MissionData[] _missions = new MissionData[0];

	public MissionData GetMission(EMissionKey missionKey) {
		for (int i = 0; i < _missions.Length; i++) {
			if (_missions[i].Key == missionKey) {
				return _missions[i];
			}
		}
		return null;
	}
}