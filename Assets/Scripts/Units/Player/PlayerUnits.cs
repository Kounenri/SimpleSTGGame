using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUnits : BaseUnits
{
	private PlayerController m_PlayerController;

	private float m_LastFireTime = 0f;
	private float m_LastReloadTime = 0f;

	protected override void Awake()
	{
		base.Awake();

		m_PlayerController = GetComponent<PlayerController>();
	}

	private void OnEnable()
	{
		if (LevelController.HasInstance)
		{
		}
	}

	protected override void OnHPChange()
	{
		base.OnHPChange();

		LevelController.GetInstance.PlayerHPChanged(m_CurrentHP);
	}

	protected override void OnDead()
	{
		LevelController.GetInstance.PlayerDead();

		base.OnDead();
	}

	public void FireWeapon()
	{
		if (LevelController.GetInstance.FireReady())
		{
			WeaponVO pWeapon = LevelController.GetInstance.CurrentWeapon;

		}
		else if (LevelController.GetInstance.LeftBulletCount <= 0)
		{
			ReloadWeapon();
		}
	}

	public bool ReloadWeapon()
	{
		if (LevelController.GetInstance.CanReloadWeapon())
		{
			LevelController.GetInstance.ReloadWeapon();

			return true;
		}

		return false;
	}
}
