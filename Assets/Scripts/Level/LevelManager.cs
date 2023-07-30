using UnityEngine;

public class LevelManager : TEventDispatcher<LevelManager>, IDispatcher
{
	public const string ON_LEVEL_CHANGE = "ON_LEVEL_CHANGE";

	public const string ON_BULLET_COUNT_CHANGE = "ON_BULLET_COUNT_CHANGE";

	public const string ON_WEAPON_CHANGE = "ON_WEAPON_CHANGE";

	public const string ON_PLAYER_HP_CHANG = "ON_PLAYER_HP_CHANG";

	private LevelVO m_CurrentLevelVO;
	private WeaponVO m_CurrentWeaponVO;
	private int m_PlayerHP = 0;

	public LevelVO CurrentLevelVO { get { return m_CurrentLevelVO; } }

	public WeaponVO CurrentWeaponVO { get { return m_CurrentWeaponVO; } }

	public int PlayerHP
	{
		get { return m_PlayerHP; }
	}

	protected override void OnInitialize()
	{
		base.OnInitialize();

		m_CurrentWeaponVO = WeaponConfProxy.GetInstance.GetDataVO(GameConfig.DEFAULT_WEAPON_ID);

		m_CurrentLevelVO = LevelConfProxy.GetInstance.GetDataVO(GameConfig.DEFAULT_LEVEL_ID);

		m_IsInit = true;
	}

	public void OnLevelLoaded()
	{
		LevelController.GetInstance.InitializeLevel();
	}

	public void ChangeLevel(LevelVO pLevelVO)
	{
		m_CurrentLevelVO = pLevelVO;

		DispatchEvent(ON_LEVEL_CHANGE, m_CurrentLevelVO);
	}

	public void ResetLevel()
	{
		ChangeWeapon(WeaponConfProxy.GetInstance.GetDataVO(GameConfig.DEFAULT_WEAPON_ID));

		ChangeLevel(LevelConfProxy.GetInstance.GetDataVO(GameConfig.DEFAULT_LEVEL_ID));

		LevelController.GetInstance.ResetLevel();
	}

	public void ChangeWeapon(WeaponVO pWeapon)
	{
		m_CurrentWeaponVO = pWeapon;

		DispatchEvent(ON_WEAPON_CHANGE, m_CurrentWeaponVO);
		DispatchEvent(ON_BULLET_COUNT_CHANGE, m_CurrentWeaponVO.Capacity);
	}

	public void BulletCountChange(int nCount)
	{
		DispatchEvent(ON_BULLET_COUNT_CHANGE, nCount);
	}

	public void PlayerHPChanged(int nPlayerHP)
	{
		m_PlayerHP = nPlayerHP;

		DispatchEvent(ON_PLAYER_HP_CHANG, m_PlayerHP);
	}

	public void LevelClear()
	{
		Debug.Log("LevelClear");

		if (m_CurrentLevelVO.NextLevelID != 0)
		{

		}
		else
		{

		}
	}

	public void LevelFail(LevelFailEnum pReason)
	{
		Debug.Log("LevelFail " + pReason.ToString());
	}
}

public enum LevelFailEnum
{
	PlayerDead,
	TimeOut
}