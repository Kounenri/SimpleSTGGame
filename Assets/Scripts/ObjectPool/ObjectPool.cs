using UnityEngine;
using UnityEngine.Pool;

public class ObjectPool : MonoBehaviour
{
	[SerializeField]
	private GameObject m_PoolObject;
	[SerializeField]
	private bool m_CollectionChecks;
	[SerializeField]
	private int m_DefaultPoolSize = 10;
	[SerializeField]
	private int m_MaxPoolSize = 100;

	private IObjectPool<GameObject> m_Pool;

	public string PoolObjectName { get { return m_PoolObject.name; } }
	public IObjectPool<GameObject> Pool { get { return m_Pool; } }

	private void Start()
	{
		m_Pool = new ObjectPool<GameObject>(CreatePooledItem, OnTakeFromPool, OnReturnedToPool, OnDestroyPoolObject, m_CollectionChecks, m_DefaultPoolSize, m_MaxPoolSize);
	}

	private void OnDestroy()
	{
		if (m_Pool != null)
		{
			m_Pool.Clear();
		}
	}

	protected GameObject CreatePooledItem()
	{
		GameObject pGameObject = Instantiate(m_PoolObject);

		pGameObject.name = m_PoolObject.name;

		return pGameObject;
	}

	// Called when an item is taken from the pool using Get
	protected virtual void OnTakeFromPool(GameObject pGameObject)
	{
		pGameObject.SetActive(true);
	}

	// Called when an item is returned to the pool using Release
	protected virtual void OnReturnedToPool(GameObject pGameObject)
	{
		pGameObject.SetActive(false);
	}

	// If the pool capacity is reached then any items returned will be destroyed.
	// We can control what the destroy behavior does, here we destroy the GameObject.
	protected virtual void OnDestroyPoolObject(GameObject pGameObject)
	{
		Destroy(pGameObject);
	}
}
