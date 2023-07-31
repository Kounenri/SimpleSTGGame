using System.Collections.Generic;

/// <summary>
/// Load config from xml or json
/// here is a demo
/// </summary>
public class WeaponConfProxy : TConfProxy<WeaponConfProxy, WeaponVO>
{
	public override void ParseConfig()
	{
		m_DataVODictionary = new Dictionary<int, WeaponVO>();

		m_DataVODictionary.Add(1, new WeaponVO
		{
			ID = 1,
			Name = "Pistol",
			Damage = 20,
			ShootingInterval = 0.5f,
			Capacity = 18,
			BulletSpeed = 10,
			ReloadDuration = 2f,
			IsPenetrable = false
		});

		m_DataVODictionary.Add(2, new WeaponVO
		{
			ID = 2,
			Name = "Submachine Gun",
			Damage = 20,
			ShootingInterval = 0.1f,
			Capacity = 30,
			BulletSpeed = 20,
			ReloadDuration = 1.5f,
			IsPenetrable = false
		});

		m_DataVODictionary.Add(3, new WeaponVO
		{
			ID = 3,
			Name = "Rifle",
			Damage = 50,
			ShootingInterval = 0.2f,
			Capacity = 30,
			BulletSpeed = 15,
			ReloadDuration = 2f,
			IsPenetrable = false
		});

		m_DataVODictionary.Add(4, new WeaponVO
		{
			ID = 4,
			Name = "Sniper Rifle",
			Damage = 100,
			ShootingInterval = 0.8f,
			Capacity = 10,
			BulletSpeed = 30,
			ReloadDuration = 2.5f,
			IsPenetrable = true
		});
	}
}

public class WeaponVO : TDataVO
{
	/// <summary>
	/// Weapon Name
	/// </summary>
	public string Name { get; set; }

	/// <summary>
	/// Weapon Damage
	/// </summary>
	public int Damage { get; set; }

	/// <summary>
	/// Shoot Between Interval
	/// </summary>
	public float ShootingInterval { get; set; }

	/// <summary>
	/// Magazine Capacity
	/// </summary>
	public int Capacity { get; set; }

	/// <summary>
	/// Bullet Speed
	/// </summary>
	public int BulletSpeed { get; set; }

	/// <summary>
	/// Reload Duration
	/// </summary>
	public float ReloadDuration { get; set; }

	/// <summary>
	/// Destroy When Hit Enemy
	/// </summary>
	public bool IsPenetrable { get; set; }

	/// <summary>
	/// Resources path for loading image
	/// </summary>
	public string Icon
	{
		get
		{
			return "Icon/Weapon/" + ID;
		}
	}
}
