using UnityEngine;

public class GameConfig
{
	/// <summary>
	/// Turn On/Off the log
	/// </summary>
	public static bool DISABLE_LOG = true;

	/// <summary>
	/// Game default fps
	/// </summary>
	public static int DEFAULT_FPS = 60;

	/// <summary>
	/// Turn On/Off background music
	/// </summary>
	public static bool ENABLE_MUSIC = true;

	/// <summary>
	/// Default weapon id when level first load
	/// </summary>
	public static int DEFAULT_WEAPON_ID = 1;

	/// <summary>
	/// Default level id
	/// </summary>
	public static int DEFAULT_LEVEL_ID = 1;

	/// <summary>
	/// Default resolution
	/// </summary>
	public static readonly Vector2 STANDARD_RESOLUTION = new Vector2(1920, 1080);
}
