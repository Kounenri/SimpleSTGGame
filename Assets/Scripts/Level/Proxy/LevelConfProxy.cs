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
			EnemyOnScreen = 50,
			InitialEnemyNumber = 50,
			EnemyIDList = new List<int>() { 1, 2 },
			EnemyCountList = new List<int>() { 80, 20 },
			CountDownTime = 120
		});

		m_DataVODictionary.Add(2, new LevelVO()
		{
			ID = 2,
			Name = "Level 2",
			NextLevelID = 3,
			EnemyOnScreen = 100,
			InitialEnemyNumber = 50,
			EnemyIDList = new List<int>() { 1, 2 },
			EnemyCountList = new List<int>() { 160, 40 },
			CountDownTime = 240
		});

		m_DataVODictionary.Add(3, new LevelVO()
		{
			ID = 3,
			Name = "Level 3",
			NextLevelID = 4,
			EnemyOnScreen = 200,
			InitialEnemyNumber = 50,
			EnemyIDList = new List<int>() { 1, 2, 3 },
			EnemyCountList = new List<int>() { 200, 80, 20 },
			CountDownTime = 360
		});

		m_DataVODictionary.Add(4, new LevelVO()
		{
			ID = 4,
			Name = "Level 4",
			NextLevelID = 5,
			EnemyOnScreen = 200,
			InitialEnemyNumber = 80,
			EnemyIDList = new List<int>() { 1, 2, 3 },
			EnemyCountList = new List<int>() { 80, 200, 20 },
			CountDownTime = 360
		});

		m_DataVODictionary.Add(5, new LevelVO()
		{
			ID = 5,
			Name = "Final Level",
			NextLevelID = 0,
			EnemyOnScreen = 200,
			InitialEnemyNumber = 100,
			EnemyIDList = new List<int>() { 2, 3 },
			EnemyCountList = new List<int>() { 80, 120 },
			CountDownTime = 360
		});
	}
}

public class LevelVO : TDataVO
{
	public string Name { get; set; }

	public int NextLevelID { get; set; }

	public int EnemyOnScreen { get; set; }

	public int InitialEnemyNumber { get; set; }

	public List<int> EnemyIDList { get; set; }

	public List<int> EnemyCountList { get; set; }

	public int CountDownTime { get; set; }
}
