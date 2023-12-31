using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LevelController : TMonoInstanceLite<LevelController>
{
	private PlayerUnits m_CurrentPlayer;
	private int m_CurrenyEnemyCount;
	private int m_OnScreenEnemyCount;
	private Dictionary<int, int> m_EnemyDictionary;
	private Coroutine m_PlayerCoroutine;
	private Coroutine m_EnemyCoroutine;
	private bool m_StartCountDown;
	private float m_LeftTime;

	public PlayerUnits CurrentPlayer { get { return m_CurrentPlayer; } }

	public int CurrentEnemyCount { get { return m_CurrenyEnemyCount; } }

	public float LeftTime { get { return m_LeftTime; } }

	protected override void Awake()
	{
		g_Instance = this;
	}

	protected override void Start()
	{
		base.Start();

		ObjectPoolManager.Create();

		//LevelManager.GetInstance.OnLevelLoaded();
		HelpView.Create();
	}

	private void Update()
	{
		if (m_StartCountDown)
		{
			// Do countdown here
			m_LeftTime -= Time.deltaTime;

			// When time gose to 0 then fail
			if (m_LeftTime <= 0f)
			{
				m_StartCountDown = false;

				LevelManager.GetInstance.LevelFail(LevelResultEnum.TimeOut);
			}
		}
	}

	private IEnumerator OnInitializePlayer()
	{
		yield return new WaitForEndOfFrame();

		Debug.Log("Begin Initialize Player.");

		GameObject pPlayerObject = ObjectPoolManager.GetInstance.Get("Player");

		// Reset play position and rotation
		pPlayerObject.transform.SetPositionAndRotation(Vector3.zero, Quaternion.Euler(0, 0, 0));

		m_CurrentPlayer = pPlayerObject.GetComponent<PlayerUnits>();

		m_CurrentPlayer.ResetUnit();

		yield return null;
	}

	private IEnumerator OnInitializeEnemy()
	{
		yield return new WaitForEndOfFrame();

		Debug.Log("Begin OnInitialize Enemy.");

		LevelVO pLevelVO = LevelManager.GetInstance.CurrentLevelVO;

		for (int i = 0; i < pLevelVO.EnemyIDList.Count; i++)
		{
			// Calculation total enemies number
			m_CurrenyEnemyCount += pLevelVO.EnemyCountList[i];

			// Save enemies id and count for later use
			m_EnemyDictionary.Add(pLevelVO.EnemyIDList[i], pLevelVO.EnemyCountList[i]);
		}

		LevelManager.GetInstance.LeftEnemyCountChange(m_CurrenyEnemyCount);

		while (true)
		{
			if (m_OnScreenEnemyCount < pLevelVO.EnemyOnScreen)
			{
				List<int> pIDList = new();

				foreach (int nKey in m_EnemyDictionary.Keys)
				{
					// Add enemies id which still remain
					if (m_EnemyDictionary[nKey] > 0)
					{
						pIDList.Add(nKey);
					}
				}

				// If no remain enemies then break
				if (pIDList.Count <= 0) break;

				// Random select a type of enemies
				int nEnemyID = pIDList[Random.Range(0, pIDList.Count)];

				OnCreateEnemy(EnemyConfProxy.GetInstance.GetDataVO(nEnemyID));

				// Calculat enemies remain count
				m_EnemyDictionary[nEnemyID] = m_EnemyDictionary[nEnemyID] - 1;
			}

			yield return new WaitForEndOfFrame();
		}
	}

	private void OnCreateEnemy(EnemyVO pEnemyVO)
	{
		GameObject pEnemyObject = ObjectPoolManager.GetInstance.Get(pEnemyVO.PrefabName);

		// Refresh enemies near the player
		Vector3 pPosition = GetRandomPosition();

		// If the coordinates are out of the map, re-randomize
		while (Mathf.Abs(pPosition.x) > 100 || Mathf.Abs(pPosition.z) > 100)
		{
			pPosition = GetRandomPosition();
		}

		pEnemyObject.transform.position = pPosition;

		EnemyUnits pEnemyUnits = pEnemyObject.GetComponent<EnemyUnits>();

		pEnemyUnits.ResetUnit(pEnemyVO);

		m_OnScreenEnemyCount++;
	}

	private Vector3 GetRandomPosition()
	{
		// Create an immediate coordinate inside the circle
		Vector2 pCircle = Random.insideUnitCircle * 20;
		// Into a hollow circle
		Vector2 pPosition = pCircle.normalized * (10 + pCircle.magnitude);

		return new Vector3(pPosition.x, 0, pPosition.y) + m_CurrentPlayer.transform.position;
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

	private void RequestChangeWeapon(int nIndex)
	{
		Debug.Log("RequestChangeWeapon " + nIndex);

		if (m_CurrentPlayer.IsReloadingWeapon) return;

		if (LevelManager.GetInstance.CurrentLevelVO.UnlockedWeapon.IndexOf(nIndex) != -1)
		{
			LevelManager.GetInstance.ChangeWeapon(WeaponConfProxy.GetInstance.GetDataVO(nIndex));
		}
	}

	public void InitializeLevel()
	{
		m_EnemyDictionary = new Dictionary<int, int>();

		m_CurrenyEnemyCount = m_OnScreenEnemyCount = 0;

		m_PlayerCoroutine = StartCoroutine(OnInitializePlayer());

		m_EnemyCoroutine = StartCoroutine(OnInitializeEnemy());

		m_LeftTime = LevelManager.GetInstance.CurrentLevelVO.CountDownTime;

		m_StartCountDown = true;
	}

	public void ResetLevel()
	{
		m_EnemyDictionary.Clear();

		m_CurrenyEnemyCount = m_OnScreenEnemyCount = 0;

		if (m_PlayerCoroutine != null)
		{
			StopCoroutine(m_PlayerCoroutine);
			m_PlayerCoroutine = null;
		}

		if (m_EnemyCoroutine != null)
		{
			StopCoroutine(m_EnemyCoroutine);
			m_EnemyCoroutine = null;
		}

		RecycleAllObjects();

		m_PlayerCoroutine = StartCoroutine(OnInitializePlayer());

		m_EnemyCoroutine = StartCoroutine(OnInitializeEnemy());

		m_LeftTime = LevelManager.GetInstance.CurrentLevelVO.CountDownTime;

		m_StartCountDown = true;
	}

	public void OnDeactiveEnemy()
	{
		// Calculate the number of remain enemies
		m_OnScreenEnemyCount--;
		m_CurrenyEnemyCount--;

		LevelManager.GetInstance.LeftEnemyCountChange(m_CurrenyEnemyCount);

		if (m_CurrenyEnemyCount == 0)
		{
			m_StartCountDown = false;

			LevelManager.GetInstance.LevelClear();
		}
	}

	public void OnWeapon1(InputAction.CallbackContext context)
	{
		if (!context.started) return;

		RequestChangeWeapon(1);
	}

	public void OnWeapon2(InputAction.CallbackContext context)
	{
		if (!context.started) return;

		RequestChangeWeapon(2);
	}

	public void OnWeapon3(InputAction.CallbackContext context)
	{
		if (!context.started) return;

		RequestChangeWeapon(3);
	}

	public void OnWeapon4(InputAction.CallbackContext context)
	{
		if (!context.started) return;

		RequestChangeWeapon(4);
	}
}
