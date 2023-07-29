using UnityEngine;

public class PlayerUnits : BaseUnits
{
	private WeaponVO m_CurrentWeapon;

	private int m_LeftBulletCount = 0;
	private bool m_IsReloadingWeapon = false;
	private float m_LastFireTime = 0f;
	private float m_LastReloadTime = 0f;

	public bool IsReloadingWeapon { get { return m_IsReloadingWeapon; } }

	public int LeftBulletCount { get { return m_LeftBulletCount; } }

	private void Awake()
	{
		if (LevelController.HasInstance)
		{
			LevelController.GetInstance.AddEventListener(LevelController.ON_WEAPON_CHANGE, OnWeaponChange);
		}
	}

	private void OnDestroy()
	{
		if (LevelController.HasInstance)
		{
			LevelController.GetInstance.RemoveEventListener(LevelController.ON_WEAPON_CHANGE, OnWeaponChange);
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
			m_CurrentWeapon = LevelController.GetInstance.CurrentWeapon;

			m_LeftBulletCount = m_CurrentWeapon.Capacity;
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

	public override void ResetUnit()
	{
		base.ResetUnit();

		OnWeaponChange(null);

		m_LeftBulletCount = m_CurrentWeapon.Capacity;

		m_IsReloadingWeapon = false;
		m_LastFireTime = 0f;
		m_LastReloadTime = 0f;

		LevelController.GetInstance.BulletCountChange(m_LeftBulletCount);
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

				LevelController.GetInstance.BulletCountChange(m_LeftBulletCount);

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

		LevelController.GetInstance.DoneReloadWeapon();
		LevelController.GetInstance.BulletCountChange(m_LeftBulletCount);
	}
}
