using UnityEngine;

public static class GameConstants {
	public const int DEFAULT_RESOLUTION_WIDTH = 2048;
	public const int DEFAULT_RESOLUTION_HEIGHT = 1152;

	public class Scenes {
		public const string CITY = "City";
		public const string FIGHT = "Fight";
	}

	public class Paths {
		public const string ITEM_RESOURCES = "Items";
		public const string UNIT_RESOURCES = "Units";
		public const string UI_RESOURCES = "UI";
		public const string UI_WINDOWS_RESOURCES = "UI/Windows";

		public static string UI_UNIT_ICONS_RESOURCES { get { return string.Format("{0}/Icons/Units", UI_RESOURCES); } }
		public static string UI_ITEM_ICONS_RESOURCES { get { return string.Format("{0}/Icons/Items", UI_RESOURCES); } }

		public class Prefabs {
			public static string UI_UNIT { get { return string.Format("{0}/UnitUI", UI_RESOURCES); } }

			public static string UI_WIN_BATTLE_PREVIEW { get { return string.Format("{0}/Missions/WndBattlePreview", UI_WINDOWS_RESOURCES); } }
			public static string UI_WIN_BATTLE_SETUP { get { return string.Format("{0}/Missions/WndBattleSetup", UI_WINDOWS_RESOURCES); } }
			public static string UI_WIN_BATTLE_VICTORY { get { return string.Format("{0}/Missions/WndBattleVictory", UI_WINDOWS_RESOURCES); } }
			public static string UI_WIN_BATTLE_DEFEAT { get { return string.Format("{0}/Missions/WndBattleDefeat", UI_WINDOWS_RESOURCES); } }
		}
	}

	public class Tags {
		public const string UNIT_ALLY = "UnitAlly";
		public const string UNIT_ENEMY = "UnitEnemy";
	}

	public class City {
		public const int MAX_BUILDING_LEVEL = 10;
		public const int MAX_UNIT_UPGRADE_LEVEL = 10;
		public const int WAREHOUSE_FILLING_TIME = 21600;	//6 hours

		public const int FUEL_REFRESH_TIME = 6000;
		public const int FUEL_REFRESH_AMOUNT = 1;
	}
}
