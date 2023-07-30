using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : TMonoInstanceLite<LevelController>
{
	private PlayerUnits m_CurrentPlayer;
	private int m_CurrenyEnemyCount;
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

		LevelManager.GetInstance.OnLevelLoaded();
	}

	private void Update()
	{
		if (m_StartCountDown)
		{
			m_LeftTime -= Time.deltaTime;

			if (m_LeftTime <= 0f)
			{
				m_StartCountDown = false;

				LevelManager.GetInstance.LevelFail(LevelFailEnum.TimeOut);
			}
		}
	}

	private IEnumerator OnInitializePlayer()
	{
		yield return new WaitForEndOfFrame();

		Debug.Log("Begin Initialize Player.");

		GameObject pPlayerObject = ObjectPoolManager.GetInstance.Get("Player");

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
			m_EnemyDictionary.Add(pLevelVO.EnemyIDList[i], pLevelVO.EnemyCountList[i]);
		}

		while (true)
		{
			if (m_CurrenyEnemyCount < pLevelVO.EnemyOnScreen)
			{
				List<int> pIDList = new();

				foreach (int nKey in m_EnemyDictionary.Keys)
				{
					if (m_EnemyDictionary[nKey] > 0)
					{
						pIDList.Add(nKey);
					}
				}

				if (pIDList.Count <= 0) break;

				int nEnemyID = pIDList[Random.Range(0, pIDList.Count)];

				OnCreateEnemy(EnemyConfProxy.GetInstance.GetDataVO(nEnemyID));

				m_EnemyDictionary[nEnemyID] = m_EnemyDictionary[nEnemyID] - 1;
			}

			yield return new WaitForEndOfFrame();
		}
	}

	private void OnCreateEnemy(EnemyVO pEnemyVO)
	{
		GameObject pEnemyObject = ObjectPoolManager.GetInstance.Get(pEnemyVO.PrefabName);

		Vector3 pCircle = Random.insideUnitCircle * 10;
		Vector3 pPosition = pCircle.normalized * (10 + pCircle.magnitude);

		pEnemyObject.transform.position = new Vector3(pPosition.x, 0, pPosition.y);

		EnemyUnits pEnemyUnits = pEnemyObject.GetComponent<EnemyUnits>();

		pEnemyUnits.ResetUnit(pEnemyVO);
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

	public void InitializeLevel()
	{
		m_EnemyDictionary = new Dictionary<int, int>();

		m_PlayerCoroutine = StartCoroutine(OnInitializePlayer());

		m_EnemyCoroutine = StartCoroutine(OnInitializeEnemy());

		m_LeftTime = LevelManager.GetInstance.CurrentLevelVO.CountDownTime;

		m_StartCountDown = true;
	}

	public void ResetLevel()
	{
		m_EnemyDictionary.Clear();

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

	public void OnActiveEnemy()
	{
		m_CurrenyEnemyCount++;
	}

	public void OnDeactiveEnemy()
	{
		m_CurrenyEnemyCount--;

		if (m_CurrenyEnemyCount == 0)
		{
			LevelManager.GetInstance.LevelClear();
		}
	}
}
