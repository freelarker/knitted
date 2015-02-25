public static class GameConstants {
	public class Paths {
		public const string ITEM_RESOURCES = "Items";
		public const string UNIT_RESOURCES = "Units";
		public const string UI_RESOURCES = "UI";
		public const string UI_WINDOWS_RESOURCES = "UI/Windows";

		public class Prefabs {
			public static string UI_UNIT { get { return string.Format("{0}/UnitUI", UI_RESOURCES); } }

			public static string UI_WIN_BATTLE_PREVIEW { get { return string.Format("{0}/Missions/BattlePreview", UI_WINDOWS_RESOURCES); } }
			public static string UI_WIN_BATTLE_SETUP { get { return string.Format("{0}/Missions/BattleSetup", UI_WINDOWS_RESOURCES); } }
			public static string UI_WIN_BATTLE_VICTORY { get { return string.Format("{0}/Missions/BattleVictory", UI_WINDOWS_RESOURCES); } }
			public static string UI_WIN_BATTLE_DEFEAT { get { return string.Format("{0}/Missions/BattleDefeat", UI_WINDOWS_RESOURCES); } }
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
