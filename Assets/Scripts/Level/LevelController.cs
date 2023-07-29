using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelController : TMonoEventDispatcher<LevelController>, IDispatcher
{
	public const string ON_BULLET_COUNT_CHANGE = "ON_BULLET_COUNT_CHANGE";

	public const string ON_WEAPON_CHANGE = "ON_WEAPON_CHANGE";
	public const string ON_WEAPON_RELOAD = "ON_WEAPON_RELOAD";

	public const string ON_PLAYER_HP_CHANG = "ON_PLAYER_HP_CHANG";
	public const string ON_PLAYER_DEAD = "ON_PLAYER_DEAD";

	private bool m_NeedResetGame = false;
	private WeaponVO m_CurrentWeapon;

	public WeaponVO CurrentWeapon
	{
		get { return m_CurrentWeapon; }
		set { m_CurrentWeapon = value; }
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

		if (m_CurrentWeapon == null)
		{
			LoadDefaultWeapon();
		}

		if (m_NeedResetGame)
		{
			RecycleAllObjects();

			LoadDefaultWeapon();

			m_NeedResetGame = false;
		}

		GameObject pPlayerObject = ObjectPoolManager.GetInstance.Get("Player");

		pPlayerObject.transform.SetPositionAndRotation(Vector3.zero, Quaternion.Euler(0, 0, 0));

		PlayerUnits pPlayerUnits = pPlayerObject.GetComponent<PlayerUnits>();

		pPlayerUnits.ResetUnit();

		for (int i = 0; i < 50; i++)
		{
			GameObject pEnemyObject = ObjectPoolManager.GetInstance.Get("Zombie1");

			Vector3 pCircle = Random.insideUnitCircle * 10;
			Vector3 pPosition = pCircle.normalized * (10 + pCircle.magnitude);

			pEnemyObject.transform.position = new Vector3(pPosition.x, 0, pPosition.y);

			EnemyUnits pEnemyUnits = pEnemyObject.GetComponent<EnemyUnits>();

			pEnemyUnits.ResetUnit();
		}

		yield return null;
	}

	private void LoadDefaultWeapon()
	{
		ChangeWeapon(WeaponConfProxy.GetInstance.GetDataVO(1));
	}

	private void RecycleAllObjects()
	{
		GameObject[] pPlayerObjects = GameObject.FindGameObjectsWithTag("Player");

		for (int i = 0; i < pPlayerObjects.Length; i++)
		{
			ObjectPoolManager.GetInstance.Release(pPlayerObjects[i]);
		}

		GameObject[] pEnemyObjects = GameObject.FindGameObjectsWithTag("Enemy");

		for (int i = 0; i < pEnemyObjects.Length; i++)
		{
			ObjectPoolManager.GetInstance.Release(pEnemyObjects[i]);
		}

		GameObject[] pBulletObjects = GameObject.FindGameObjectsWithTag("Bullet");

		for (int i = 0; i < pBulletObjects.Length; i++)
		{
			ObjectPoolManager.GetInstance.Release(pBulletObjects[i]);
		}
	}

	public void ChangeWeapon(WeaponVO pWeapon)
	{
		m_CurrentWeapon = pWeapon;

		DispatchEvent(ON_WEAPON_CHANGE, m_CurrentWeapon);
		DispatchEvent(ON_BULLET_COUNT_CHANGE, m_CurrentWeapon.Capacity);
	}

	public void BulletCountChange(int nCount)
	{
		DispatchEvent(ON_BULLET_COUNT_CHANGE, nCount);
	}

	public void PlayerHPChanged(float fPlayerHP)
	{
		if (fPlayerHP < 0f) fPlayerHP = 0f;

		DispatchEvent(ON_PLAYER_HP_CHANG, fPlayerHP);
	}

	public void PlayerDead()
	{
		DispatchEvent(ON_PLAYER_DEAD);
	}

	public void DoneReloadWeapon()
	{
		DispatchEvent(ON_WEAPON_RELOAD, m_CurrentWeapon);
	}
}
