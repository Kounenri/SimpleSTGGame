using UnityEngine;
using UnityEngine.InputSystem.XR;

public class PlayerUnits : BaseUnits
{
	private WeaponVO m_CurrentWeapon;
	private PlayerController m_Controller;

	private int m_TotalHP = 100;
	private int m_MoveSpeed = 4;
	private int m_LeftBulletCount = 0;
	private bool m_IsReloadingWeapon = false;
	private float m_LastFireTime = 0f;
	private float m_LastReloadTime = 0f;

	public int MoveSpeed { get { return m_MoveSpeed; } }

	public bool IsReloadingWeapon { get { return m_IsReloadingWeapon; } }

	public int LeftBulletCount { get { return m_LeftBulletCount; } }

	private void Awake()
	{
		m_Controller = GetComponent<PlayerController>();

		if (LevelManager.HasInstance)
		{
			LevelManager.GetInstance.AddEventListener(LevelManager.ON_WEAPON_CHANGE, OnWeaponChange);
		}
	}

	private void OnDestroy()
	{
		if (LevelManager.HasInstance)
		{
			LevelManager.GetInstance.RemoveEventListener(LevelManager.ON_WEAPON_CHANGE, OnWeaponChange);
		}
	}

	private void OnWeaponChange(TEvent pTEvent)
	{
		if (pTEvent != null)
		{
			m_CurrentWeapon = pTEvent.Data as WeaponVO;
		}
		else
		{
			m_CurrentWeapon = LevelManager.GetInstance.CurrentWeaponVO;

			m_LeftBulletCount = m_CurrentWeapon.Capacity;
		}
	}

	protected override void OnHPChange()
	{
		base.OnHPChange();

		LevelManager.GetInstance.PlayerHPChange(m_CurrentHP);
	}

	protected override void OnDead()
	{
		m_Controller.OnDead();

		LevelManager.GetInstance.LevelFail(LevelResultEnum.PlayerDead);
	}

	public void ResetUnit()
	{
		OnWeaponChange(null);

		m_CurrentHP = m_TotalHP;
		OnHPChange();

		m_LeftBulletCount = m_CurrentWeapon.Capacity;

		m_IsReloadingWeapon = false;
		m_LastFireTime = 0f;
		m_LastReloadTime = 0f;

		LevelManager.GetInstance.BulletCountChange(m_LeftBulletCount);
	}

	public bool FireReady()
	{
		return !m_IsReloadingWeapon && m_LeftBulletCount > 0;
	}

	public (bool, WeaponVO) FireWeapon()
	{
		if (!IsDead() && FireReady())
		{
			if (m_LastFireTime == 0f || Time.time - m_LastFireTime > m_CurrentWeapon.ShootingInterval)
			{
				m_LastFireTime = Time.time;

				m_LeftBulletCount--;

				LevelManager.GetInstance.BulletCountChange(m_LeftBulletCount);

				return (true, m_CurrentWeapon);
			}
		}

		return (false, null);
	}

	public bool CanReloadWeapon()
	{
		return !m_IsReloadingWeapon && m_LeftBulletCount < m_CurrentWeapon.Capacity;
	}

	public bool ReloadWeapon()
	{
		if (m_LastReloadTime == 0f || Time.time - m_LastReloadTime > m_CurrentWeapon.ReloadDuration)
		{
			if (!IsDead() && CanReloadWeapon())
			{
				if (!m_IsReloadingWeapon)
				{
					m_IsReloadingWeapon = true;

					Invoke(nameof(OnReloadWeapon), m_CurrentWeapon.ReloadDuration);

					m_LastReloadTime = Time.time;

					return true;
				}
			}
		}

		return false;
	}

	private void OnReloadWeapon()
	{
		m_IsReloadingWeapon = false;

		m_LeftBulletCount = m_CurrentWeapon.Capacity;

		LevelManager.GetInstance.BulletCountChange(m_LeftBulletCount);
	}
}
