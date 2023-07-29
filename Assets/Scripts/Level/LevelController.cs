using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelController : TMonoEventDispatcher<LevelController>, IDispatcher
{
	public const string ON_WEAPON_CHANGED = "ON_WEAPON_CHANGED";
	public const string ON_WEAPON_RELOADED = "ON_WEAPON_RELOADED";

	public const string ON_PLAYER_HP_CHANGED = "ON_PLAYER_HP_CHANGED";
	public const string ON_PLAYER_DEAD = "ON_PLAYER_DEAD";

	private bool m_NeedResetGame = false;
	private WeaponVO m_CurrentWeapon;
	private int m_LeftBulletCount = 0;
	private bool m_IsReloadingWeapon = false;

	public WeaponVO CurrentWeapon
	{
		get { return m_CurrentWeapon; }
		set { m_CurrentWeapon = value; }
	}

	public int LeftBulletCount
	{
		get { return m_LeftBulletCount; }
	}

	protected override void Awake()
	{
		base.Awake();

		SceneManager.sceneLoaded += OnLoadCallback;
	}

	private void OnLoadCallback(Scene pScene, LoadSceneMode pLoadSceneMode)
	{
		if (pScene.name == LevelNameEnum.GameScene)
		{
			StartCoroutine(InitializeLevel());
		}
	}

	private IEnumerator InitializeLevel()
	{
		yield return new WaitForEndOfFrame();

		Debug.Log("Begin Initialize Level.");

		m_IsReloadingWeapon = false;

		if (m_NeedResetGame)
		{
			m_CurrentWeapon = WeaponConfProxy.GetInstance.GetDataVO(1);

			m_LeftBulletCount = m_CurrentWeapon.Capacity;

			m_NeedResetGame = false;
		}
		else
		{

		}

		yield return null;
	}

	public void ChangeWeapon(WeaponVO pWeapon)
	{
		m_CurrentWeapon = pWeapon;

		DispatchEvent(ON_WEAPON_CHANGED, m_CurrentWeapon);
	}

	public void PlayerHPChanged(float fPlayerHP)
	{
		if (fPlayerHP < 0f) fPlayerHP = 0f;

		DispatchEvent(ON_PLAYER_HP_CHANGED, fPlayerHP);
	}

	public void PlayerDead()
	{
		DispatchEvent(ON_PLAYER_DEAD);
	}

	public bool FireReady()
	{
		return !m_IsReloadingWeapon && m_LeftBulletCount > 0;
	}


	public bool CanReloadWeapon()
	{
		return !m_IsReloadingWeapon && m_LeftBulletCount < m_CurrentWeapon.Capacity;
	}

	public void ReloadWeapon()
	{
		if (!m_IsReloadingWeapon)
		{
			m_IsReloadingWeapon = true;

			Invoke(nameof(OnReloadWeapon), m_CurrentWeapon.ReloadDuration);
		}
	}

	public void OnReloadWeapon()
	{
		m_LeftBulletCount = m_CurrentWeapon.Capacity;

		m_IsReloadingWeapon = false;

		DispatchEvent(ON_WEAPON_RELOADED, m_LeftBulletCount);
	}
}
