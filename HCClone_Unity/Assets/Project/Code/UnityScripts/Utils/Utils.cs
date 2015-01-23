using System;

public class Utils {
	private static DateTime _unixEpochStart = new DateTime(1970, 1, 1);

	public static int UnixTimestamp {
		get { return (int)(DateTime.UtcNow - _unixEpochStart).TotalSeconds; }
	}
}
