using UnityEngine;

/// <summary>
/// Keeps all items data
/// Configured through prefab
/// </summary>
public class ItemsData : MonoBehaviourResourceSingleton<ItemsData> {	//TODO: create editor
#pragma warning disable 0414
	private static string _path = "Config/ItemsData";
#pragma warning restore 0414
	

	[SerializeField]
	private BaseItem[] _data = new BaseItem[0];

	public BaseItem GetItem(EItemKey itemKey) {
		for (int i = 0; i < _data.Length; i++) {
			if (_data[i].Key == itemKey) {
				return _data[i];
			}
		}

		return null;
	}
}
