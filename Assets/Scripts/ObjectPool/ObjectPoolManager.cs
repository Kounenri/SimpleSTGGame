using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolManager : TMonoInstanceLite<ObjectPoolManager>
{
	[SerializeField]
	private List<GameObject> m_ObjectPoolList = new();

	private Dictionary<string, ObjectPool> m_PoolDic = new();

	public static ObjectPoolManager Create()
	{
		GameObject pPrefab = Resources.Load<GameObject>("ObjectPoolManager");

		GameObject pObject = Instantiate(pPrefab);

		pObject.name = "ObjectPoolManager";

		return pObject.GetComponent<ObjectPoolManager>();
	}

	protected override void Awake()
	{
		g_Instance = this;

		for (int i = 0; i < m_ObjectPoolList.Count; i++)
		{
			GameObject pGameObject = m_ObjectPoolList[i];

			if (pGameObject != null)
			{
				GameObject pPoolGameObject = Instantiate(pGameObject, transform, false);

				pPoolGameObject.name = pGameObject.name;

				ObjectPool pObjectPool = pPoolGameObject.GetComponent<ObjectPool>();

				m_PoolDic.Add(pObjectPool.PoolObjectName, pObjectPool);
			}
		}
	}

	protected override void OnDestroy()
	{
		foreach (string strKey in m_PoolDic.Keys)
		{
			Destroy(m_PoolDic[strKey]);
		}

		m_PoolDic = null;

		base.OnDestroy();
	}

	public GameObject Get(string strName)
	{
		if (m_PoolDic == null)
		{
			Debug.LogError("try to get object " + strName + " after scene destroyed");

			return null;
		}

		if (m_PoolDic.ContainsKey(strName))
		{
			return m_PoolDic[strName].Pool.Get();
		}

		return null;
	}

	public void Release(GameObject pGameObject)
	{
		if (pGameObject == null) return;

		if (m_PoolDic.ContainsKey(pGameObject.name))
		{
			m_PoolDic[pGameObject.name].Pool.Release(pGameObject);
		}
	}
}
