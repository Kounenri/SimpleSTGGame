using System.Collections.Generic;

/// <summary>
/// Load config from xml or json
/// here is a demo
/// </summary>
/// </summary>
public class LevelConfProxy : TConfProxy<LevelConfProxy, LevelVO>
{
	public override void ParseConfig()
	{
		m_DataVODictionary = new Dictionary<int, LevelVO>();

		m_DataVODictionary.Add(1, new LevelVO()
		{
			ID = 1,
			Name = "Level 1",
			NextLevelID = 2,
			EnemyOnScreen = 30,
			EnemyIDList = new List<int>() { 1 },
			EnemyCountList = new List<int>() { 50 },
			UnlockedWeapon = new List<int> { 1 },
			CountDownTime = 100,
		});

		m_DataVODictionary.Add(2, new LevelVO()
		{
			ID = 2,
			Name = "Level 2",
			NextLevelID = 3,
			EnemyOnScreen = 40,
			EnemyIDList = new List<int>() { 1, 2 },
			EnemyCountList = new List<int>() { 80, 20 },
			UnlockedWeapon = new List<int> { 1 },
			CountDownTime = 120,
		});

		m_DataVODictionary.Add(3, new LevelVO()
		{
			ID = 4,
			Name = "Level 3",
			NextLevelID = 5,
			EnemyOnScreen = 50,
			EnemyIDList = new List<int>() { 1, 2 },
			EnemyCountList = new List<int>() { 160, 40 },
			UnlockedWeapon = new List<int> { 1, 2 },
			CountDownTime = 240
		});

		m_DataVODictionary.Add(4, new LevelVO()
		{
			ID = 4,
			Name = "Level 4",
			NextLevelID = 5,
			EnemyOnScreen = 60,
			EnemyIDList = new List<int>() { 1, 2, 3 },
			EnemyCountList = new List<int>() { 200, 80, 20 },
			UnlockedWeapon = new List<int> { 1, 2, 3 },
			CountDownTime = 360
		});

		m_DataVODictionary.Add(5, new LevelVO()
		{
			ID = 5,
			Name = "Level 5",
			NextLevelID = 6,
			EnemyOnScreen = 70,
			EnemyIDList = new List<int>() { 1, 2, 3 },
			EnemyCountList = new List<int>() { 80, 200, 20 },
			UnlockedWeapon = new List<int> { 1, 2, 3, 4 },
			CountDownTime = 360
		});

		m_DataVODictionary.Add(6, new LevelVO()
		{
			ID = 6,
			Name = "Final Level",
			NextLevelID = 0,
			EnemyOnScreen = 50,
			EnemyIDList = new List<int>() { 2, 3 },
			EnemyCountList = new List<int>() { 80, 120 },
			UnlockedWeapon = new List<int> { 1, 2, 3, 4 },
			CountDownTime = 360
		});
	}
}

public class LevelVO : TDataVO
{
	/// <summary>
	/// Level Name
	/// </summary>
	public string Name { get; set; }

	/// <summary>
	/// Next Level ID
	/// </summary>
	public int NextLevelID { get; set; }

	/// <summary>
	/// Number Of Enemies At Time
	/// </summary>
	public int EnemyOnScreen { get; set; }

	/// <summary>
	/// Enemy IDs That Appear
	/// </summary>
	public List<int> EnemyIDList { get; set; }

	/// <summary>
	/// Enemy Count That Appear
	/// </summary>
	public List<int> EnemyCountList { get; set; }

	/// <summary>
	/// Player Can Used Weapon
	/// </summary>
	public List<int> UnlockedWeapon { get; set; }

	/// <summary>
	/// Countdown To Failure
	/// </summary>
	public int CountDownTime { get; set; }
}
