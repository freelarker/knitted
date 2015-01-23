using System.Collections.Generic;

/// <summary>
/// Information about completed levels, missions, etc
/// </summary>
public class PlayerStoryProgress {
	private Dictionary<EPlanetKey, List<EMissionKey>> _progress = new Dictionary<EPlanetKey, List<EMissionKey>>();

	public void SaveProgress(EPlanetKey planetKey, EMissionKey missionKey) {
		if (!IsMissionCompleted(planetKey, missionKey)) {
			if (!_progress.ContainsKey(planetKey)) {
				_progress.Add(planetKey, new List<EMissionKey>());
			}
			_progress[planetKey].Add(missionKey);
		}
	}

	public bool IsMissionCompleted(EPlanetKey planetKey, EMissionKey missionKey) {
		if (!_progress.ContainsKey(planetKey)) {
			return false;
		}
		return _progress[planetKey].IndexOf(missionKey) != -1;
	}

	public bool IsPlanetCompleted(EPlanetKey planetKey) {
		if (!_progress.ContainsKey(planetKey)) {
			return false;
		}

		PlanetData pd = MissionsData.Instance.GetPlanet(planetKey);
		for (int i = 0; i < pd.Missions.Length; i++) {
			if (!IsMissionCompleted(planetKey, pd.Missions[i].Key)) {
				return false;
			}
		}

		return true;
	}
}
