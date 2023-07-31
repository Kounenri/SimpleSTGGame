using UnityEngine;

public class LevelManager : TEventDispatcher<LevelManager>, IDispatcher
{
	public const string ON_LEVEL_CHANGE = "ON_LEVEL_CHANGE";

	public const string ON_BULLET_COUNT_CHANGE = "ON_BULLET_COUNT_CHANGE";

	public const string ON_WEAPON_CHANGE = "ON_WEAPON_CHANGE";

	public const string ON_PLAYER_HP_CHANG = "ON_PLAYER_HP_CHANG";

	public const string ON_LEFT_ENEMY_COUNT_CHANGE = "ON_LEFT_ENEMY_COUNT_CHANGE";

	private LevelVO m_CurrentLevelVO;
	private WeaponVO m_CurrentWeaponVO;
	private bool m_IsRunning;

	public LevelVO CurrentLevelVO { get { return m_CurrentLevelVO; } }

	public WeaponVO CurrentWeaponVO { get { return m_CurrentWeaponVO; } }

	public bool IsRunning { get { return m_IsRunning; } }

	protected override void OnInitialize()
	{
		base.OnInitialize();

		m_CurrentWeaponVO = WeaponConfProxy.GetInstance.GetDataVO(GameConfig.DEFAULT_WEAPON_ID);

		m_CurrentLevelVO = LevelConfProxy.GetInstance.GetDataVO(GameConfig.DEFAULT_LEVEL_ID);

		m_IsInit = true;
	}

	public void OnLevelLoaded()
	{
		ChangeLevel(m_CurrentLevelVO);

		LevelController.GetInstance.InitializeLevel();

		m_IsRunning = true;
	}

	public void ChangeLevel(LevelVO pLevelVO)
	{
		m_CurrentLevelVO = pLevelVO;

		DispatchEvent(ON_LEVEL_CHANGE, m_CurrentLevelVO);
	}

	public void ResetLevel()
	{
		if (!m_IsRunning)
		{
			ChangeWeapon(WeaponConfProxy.GetInstance.GetDataVO(GameConfig.DEFAULT_WEAPON_ID));

			ChangeLevel(LevelConfProxy.GetInstance.GetDataVO(GameConfig.DEFAULT_LEVEL_ID));

			LevelController.GetInstance.ResetLevel();

			m_IsRunning = true;
		}
	}

	public void NextLevel()
	{
		if (!m_IsRunning)
		{
			ChangeLevel(LevelConfProxy.GetInstance.GetDataVO(m_CurrentLevelVO.NextLevelID));

			LevelController.GetInstance.ResetLevel();

			m_IsRunning = true;
		}
	}

	public void ChangeWeapon(WeaponVO pWeapon)
	{
		m_CurrentWeaponVO = pWeapon;

		DispatchEvent(ON_WEAPON_CHANGE, m_CurrentWeaponVO);
	}

	public void BulletCountChange(int nCount)
	{
		DispatchEvent(ON_BULLET_COUNT_CHANGE, nCount);
	}

	public void PlayerHPChange(int nPlayerHP)
	{
		DispatchEvent(ON_PLAYER_HP_CHANG, nPlayerHP);
	}

	public void LeftEnemyCountChange(int nEnemyCount)
	{
		DispatchEvent(ON_LEFT_ENEMY_COUNT_CHANGE, nEnemyCount);
	}

	public void LevelClear()
	{
		Debug.Log("LevelClear");

		m_IsRunning = false;

		if (m_CurrentLevelVO.NextLevelID != 0)
		{
			ResultView.Create(LevelResultEnum.LevelClear);
		}
		else
		{
			ResultView.Create(LevelResultEnum.AllLevelClear);
		}
	}

	public void LevelFail(LevelResultEnum pResult)
	{
		Debug.Log("LevelFail " + pResult.ToString());

		m_IsRunning = false;

		ResultView.Create(pResult);
	}
}

public enum LevelResultEnum
{
	LevelClear,
	AllLevelClear,
	PlayerDead,
	TimeOut
}