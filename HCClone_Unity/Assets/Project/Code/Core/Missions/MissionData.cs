using UnityEngine;

[System.Serializable]
public class MissionData {
	[SerializeField]
	private EMissionKey _key = EMissionKey.None;
	public EMissionKey Key {
		get { return _key; }
	}

	[SerializeField]
	private MissionMapData[] _maps = new MissionMapData[0];
	public int MapsCount {
		get { return _maps.Length; }
	}

	public MissionMapData GetMap(int index) {
		return index >= 0 || index < _maps.Length ? _maps[index] : null;
	}
}
