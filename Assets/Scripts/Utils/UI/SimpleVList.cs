using System;
using System.Collections.Generic;
using UnityEngine;

public class SimpleVList : MonoBehaviour
{
	[SerializeField]
	private GameObject m_RenderPrefab;
	[SerializeField]
	private bool m_ReuseRenders = true;
	[SerializeField]
	private bool m_ClearRendersWhenAwake = true;

	protected GameObject m_SelectedRender = null;
	protected bool m_NeedRefreshView = false;
	protected List<object> m_DataProvider = null;
	protected List<GameObject> m_ReuseRenderList = new();
	protected bool m_HasDoneRefresh;

	public Action<GameObject> OnSelectRenderHandler = null;

	public virtual List<object> DataProvider
	{
		get
		{
			return m_DataProvider;
		}
		set
		{
			m_DataProvider = value;
			m_NeedRefreshView = true;
			m_HasDoneRefresh = false;
		}
	}

	public bool HasData
	{
		get
		{
			return m_DataProvider != null && m_DataProvider.Count > 0;
		}
	}

	public bool HasDoneRefresh
	{
		get
		{
			return m_HasDoneRefresh;
		}
	}

	public bool ReuseRenders
	{
		get
		{
			return m_ReuseRenders;
		}
	}

	private void Awake()
	{
		if (m_ClearRendersWhenAwake)
		{
			foreach (Transform pChildTransform in transform)
			{
				if (pChildTransform.GetComponent<ARender>() != null)
				{
					Destroy(pChildTransform.gameObject);
				}
			}
		}

		OnInit();
	}

	private void LateUpdate()
	{
		if (m_NeedRefreshView)
		{
			RefreshView();

			m_NeedRefreshView = false;
		}
	}

	protected virtual void OnInit()
	{
	}

	protected virtual GameObject GetRenderPrefab(object pDataVO)
	{
		return m_RenderPrefab;
	}

	public void RefreshSelectedRender(GameObject pRenderObject)
	{
		foreach (Transform pChildTransform in transform)
		{
			if (pChildTransform.gameObject.activeSelf == false || pChildTransform.GetComponent<ARender>() == null) continue;

			if (pRenderObject == pChildTransform.gameObject)
			{
				pChildTransform.GetComponent<ARender>().OnSelect();
				m_SelectedRender = pChildTransform.gameObject;
			}
			else
			{
				pChildTransform.GetComponent<ARender>().OnDisSelect();
			}
		}

		OnSelectRenderHandler?.Invoke(m_SelectedRender);
	}

	public Vector2 GetRenderSize()
	{
		ARender pARender = GetComponentInChildren<ARender>();

		if (pARender != null && pARender.gameObject.activeSelf)
		{
			var pRectTransform = pARender.GetComponent<RectTransform>();

			return new Vector2(pRectTransform.rect.width, pRectTransform.rect.height);
		}
		return new Vector2();
	}

	public Vector2 GetRenderSize(int nIndex)
	{
		GameObject pGameObject = GetRender(nIndex);

		if (pGameObject != null)
		{
			return pGameObject.GetComponent<RectTransform>().sizeDelta;
		}

		return new Vector2();
	}

	public Vector2 GetRenderPos(int nIndex)
	{

		GameObject obj = GetRender(nIndex);
		if (obj != null)
		{

			return obj.GetComponent<RectTransform>().offsetMin;
		}

		return new Vector2();

	}

	public GameObject GetRender(int nIndex)
	{
		if (nIndex >= transform.childCount)
			return null;

		GameObject obj = transform.GetChild(nIndex).gameObject;
		if (obj.activeSelf == false || obj.GetComponent<ARender>() == null)
		{
			return GetRender(nIndex + 1);
		}
		return obj;
	}

	public object GetRenderData(int nIndex)
	{
		if (nIndex >= transform.childCount)
		{
			return null;
		}

		return GetRender(nIndex).GetComponent<ARender>().Data;
	}

	public List<Transform> GetChildList()
	{
		List<Transform> pList = new();

		for (int i = 0; i < transform.childCount; i++)
		{
			Transform pTransform = transform.GetChild(i);

			if (pTransform.gameObject.activeSelf == true && pTransform.GetComponent<ARender>() != null)
			{
				pList.Add(pTransform);
			}
		}

		return pList;
	}

	public int GetRenderNum()
	{
		int nResult = 0;
		for (int i = 0; i < transform.childCount; i++)
		{
			if (transform.GetChild(i).gameObject.activeSelf == true && transform.GetChild(i).GetComponent<ARender>() != null)
			{
				nResult++;
			}
		}
		return nResult;
	}

	public GameObject GetSelectedRender()
	{
		return m_SelectedRender;
	}

	protected virtual void RefreshView()
	{
		m_SelectedRender = null;

		foreach (Transform pChildTransform in transform)
		{
			ARender pARender = pChildTransform.GetComponent<ARender>();
			if (pARender != null)
			{
				if (m_ReuseRenders)
				{
					RecycleRender(pChildTransform.gameObject);
				}
				else
				{
					Destroy(pChildTransform.gameObject);
				}
			}
		}

		if (m_DataProvider == null) return;

		for (int i = 0; i < m_DataProvider.Count; i++)
		{
			GameObject pRenderObject = CreateRender(m_DataProvider[i], i);
			pRenderObject.GetComponent<ARender>().RenderOrder = i;
			pRenderObject.transform.SetSiblingIndex(transform.childCount);
		}

		AdjustSibling();

		m_HasDoneRefresh = true;
	}

	protected GameObject GetReuseRender()
	{
		int nCount = m_ReuseRenderList.Count;

		if (nCount <= 0) return null;

		GameObject pRenderObject = m_ReuseRenderList[nCount - 1];

		if (pRenderObject.GetComponent<ARender>() == null)
		{
			m_ReuseRenderList.Remove(pRenderObject);
			return null;
		}

		m_ReuseRenderList.RemoveAt(nCount - 1);

		pRenderObject.SetActive(true);

		return pRenderObject;
	}

	public void RecycleRender(GameObject pRender)
	{
		ARender pARender = pRender.GetComponent<ARender>();

		if (pARender != null && pARender.enabled)
		{
			pRender.SetActive(false);

			if (m_ReuseRenderList.IndexOf(pRender) == -1)
			{
				m_ReuseRenderList.Add(pRender);
			}
		}
	}

	protected virtual GameObject CreateRender(object pData, int nIndex = 0, int nSiblingIndex = -1)
	{
		GameObject pRenderObject = GetReuseRender();
		if (pRenderObject == null)
		{
			pRenderObject = Instantiate(GetRenderPrefab(pData));
			pRenderObject.GetComponent<RectTransform>().SetParent(gameObject.GetComponent<RectTransform>(), false);
			if (nSiblingIndex >= 0) pRenderObject.transform.SetSiblingIndex(nIndex);
		}

		if (pRenderObject.TryGetComponent<ARender>(out var pARender))
		{
			pARender.RenderOrder = nIndex;
			pARender.Data = pData;
		}

		return pRenderObject;
	}

	private void AdjustSibling()
	{
		foreach (Transform pChildTransform in transform)
		{
			if (pChildTransform.TryGetComponent<KeepRenderSibling>(out var pKeepRenderSibling))
			{
				if (pKeepRenderSibling.KeepInFirst)
				{
					pKeepRenderSibling.GetComponent<RectTransform>().SetAsFirstSibling();
				}
				else
				{
					pKeepRenderSibling.GetComponent<RectTransform>().SetAsLastSibling();
				}
			}
		}
	}

	public void AddData(object pData, bool bInsertToLast = true)
	{
		List<object> pList = new() { pData };

		if (bInsertToLast)
		{
			InsertLast(pList);
		}
		else
		{
			InsertFirst(pList);
		}
	}

	public void Insert(List<object> pList, int nIndex)
	{
		m_DataProvider ??= new List<object>();

		m_DataProvider.InsertRange(nIndex, pList);

		for (int i = 0; i < pList.Count; i++)
		{
			GameObject pRender = CreateRender(pList[i]);

			pRender.GetComponent<RectTransform>().SetSiblingIndex(nIndex + i);
		}

		AdjustSibling();
	}

	public void InsertLast(List<object> pList)
	{
		m_DataProvider ??= new List<object>();

		m_DataProvider.InsertRange(m_DataProvider.Count, pList);

		for (int i = 0; i < pList.Count; i++)
		{
			GameObject pRender = CreateRender(pList[i]);

			pRender.GetComponent<RectTransform>().SetSiblingIndex(pRender.transform.parent.childCount - 1);
		}

		AdjustSibling();
	}

	public void InsertFirst(List<object> pList)
	{
		m_DataProvider ??= new List<object>();

		m_DataProvider.InsertRange(0, pList);

		for (int i = 0; i < pList.Count; i++)
		{
			GameObject pRender = CreateRender(pList[i]);

			pRender.GetComponent<RectTransform>().SetSiblingIndex(i);
		}

		AdjustSibling();
	}

	public void RemoveData(object pData)
	{
		List<Transform> pRenders = GetChildList();

		Transform pTransform = pRenders.Find((p) => { return p.GetComponent<ARender>().Data == pData; });

		if (pTransform != null)
		{
			Destroy(pTransform.gameObject);
		}

		int nIndex = m_DataProvider.FindIndex((p) =>
		{
			return pData == p;
		});

		if (nIndex >= 0) m_DataProvider.RemoveAt(nIndex);
	}
}
