using System.Collections.Generic;

/// <summary>
/// Load config from xml or json
/// here is a demo
/// </summary>
public class WeaponConfProxy : TConfProxy<WeaponConfProxy, WeaponVO>
{
	public override void ParseConfig()
	{
		m_DataVODictionary = new Dictionary<int, WeaponVO>
		{
			{ 1, new WeaponVO{
				ID = 1,
				Name = "Pistol",
				Damage = 20f,
				ShootingInterval = 0.5f,
				Capacity = 20,
				BulletSpeed = 10f,
				ReloadDuration = 1f,
				IsPenetrable = false
				}
			},
			{ 2, new WeaponVO{
				ID = 2,
				Name = "Submachine Gun",
				Damage = 20f,
				ShootingInterval = 0.1f,
				Capacity = 30,
				BulletSpeed = 20f,
				ReloadDuration = 1f,
				IsPenetrable = false
				}
			},
			{ 3, new WeaponVO{
				ID = 3,
				Name = "Rifle",
				Damage = 50f,
				ShootingInterval = 0.2f,
				Capacity = 30,
				BulletSpeed = 15f,
				ReloadDuration = 1.2f,
				IsPenetrable = false
				}
			},
			{ 4, new WeaponVO{
				ID = 4,
				Name = "Sniper Rifle",
				Damage = 100f,
				ShootingInterval = 0.8f,
				Capacity = 30,
				BulletSpeed = 30f,
				ReloadDuration = 1.5f,
				IsPenetrable = true
				}
			}
		};
	}
}

public class WeaponVO : TDataVO
{
	public string Name { get; set; }

	public float Damage { get; set; }

	public float ShootingInterval { get; set; }

	public int Capacity { get; set; }

	public float BulletSpeed { get; set; }

	public float ReloadDuration { get; set; }

	public bool IsPenetrable { get; set; }
}
