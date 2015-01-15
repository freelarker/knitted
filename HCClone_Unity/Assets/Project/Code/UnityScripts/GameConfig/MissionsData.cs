using UnityEngine;

public class MissionsData : MonoBehaviourResourceSingleton<MissionsData> {
#pragma warning disable 0414
	private static string _path = "Config/MissionsData";
#pragma warning restore 0414

	[SerializeField]
	private float _unitsXPositionStartOffset = 0f;	//start units offset from battlefield
	public float UnitsXPositionStartOffset {
		get { return _unitsXPositionStartOffset; }
	}

	//private Vector2 _battlefieldSize = new Vector2();	//can be taken frrom Pathfinder class

	[SerializeField]
	private PlanetData[] _planets = new PlanetData[0];

	public PlanetData GetPlanet(EPlanetKey planetKey) {
		for (int i = 0; i < _planets.Length; i++) {
			if (_planets[i].Key == planetKey) {
				return _planets[i];
			}
		}
		return null;
	}
}
