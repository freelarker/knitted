#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public class EditorMenu : MonoBehaviour {
	[MenuItem ("HCClone/Config/Items config")]
	public static void InstantiateItemsConfig() {
		Selection.activeObject = InstantiateSingletonPrefab <ItemsData>();
	}

	[MenuItem("HCClone/Config/Units config")]
	public static void InstantiateUnitsConfig() {
		Selection.activeObject = InstantiateSingletonPrefab<UnitsData>();
	}

	[MenuItem("HCClone/Config/Missions config")]
	public static void InstantiateMissionsConfig() {
		Selection.activeObject = InstantiateSingletonPrefab<MissionsData>();
	}

	private static GameObject InstantiateSingletonPrefab<T>() where T : MonoBehaviour {
		GameObject goData = null;
		T data = (T)FindObjectOfType(typeof(T));
		if (data == null) {
			string path = typeof(T).GetField("_path", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic).GetValue(null).ToString();
			goData = UnityEditor.PrefabUtility.InstantiatePrefab(Resources.Load(path) as GameObject) as GameObject;
		} else {
			goData = data.gameObject;
		}

		return goData;
	}
}
#endif