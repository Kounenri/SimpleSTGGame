using System.Collections.Generic;

/// <summary>
/// Load config from xml or json
/// here is a demo
/// </summary>
/// </summary>
public class EnemyConfProxy : TConfProxy<EnemyConfProxy, EnemyVO>
{
	public override void ParseConfig()
	{
		m_DataVODictionary = new Dictionary<int, EnemyVO>();

		m_DataVODictionary.Add(1, new EnemyVO()
		{
			ID = 1,
			Name = "Enemy 1",
			Description = "Enemy 1 Description",
			ResourceID = 1,
			PrefabName = "Zombie1",
			TotalHP = 20,
			MoveSpeed = 2,
			Damage = 2,
			AttackRange = 2f,
			AttackInterval = 1f,
			ExperiencePoints = 1
		});

		m_DataVODictionary.Add(2, new EnemyVO()
		{
			ID = 2,
			Name = "Enemy 2",
			Description = "Enemy 2 Description",
			ResourceID = 2,
			PrefabName = "Zombie2",
			TotalHP = 50,
			MoveSpeed = 3,
			Damage = 5,
			AttackRange = 2.5f,
			AttackInterval = 0.8f,
			ExperiencePoints = 2
		});

		m_DataVODictionary.Add(3, new EnemyVO()
		{
			ID = 3,
			Name = "Enemy 3",
			Description = "Enemy 3 Description",
			ResourceID = 3,
			PrefabName = "Zombie3",
			TotalHP = 100,
			MoveSpeed = 4,
			Damage = 10,
			AttackRange = 3f,
			AttackInterval = 0.5f,
			ExperiencePoints = 5
		});
	}
}

public class EnemyVO : TDataVO
{
	public string Name { get; set; }

	public string Description { get; set; }

	public int ResourceID { get; set; }

	public string PrefabName { get; set; }

	public int TotalHP { get; set; }

	public int MoveSpeed { get; set; }

	public int Damage { get; set; }

	public float AttackRange { get; set; }

	public float AttackInterval { get; set; }

	public int ExperiencePoints { get; set; }
}
