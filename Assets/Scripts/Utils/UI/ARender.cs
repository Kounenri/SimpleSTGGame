using UnityEngine;
using UnityEngine.EventSystems;

public class ARender : MonoBehaviour, IPointerClickHandler
{
	[SerializeField]
	protected bool m_UpdateSelectRender = false;

	private int m_RenderOrder = 0;
	private object m_Data;
	private float m_DisableRenderDuration = 2f;
	private bool m_Invalidate = false;

	public float DisableRenderDuration
	{
		get
		{
			return m_DisableRenderDuration;
		}
		set
		{
			m_DisableRenderDuration = value;
		}
	}

	public bool DisableRender
	{
		get
		{
			return transform.Find("DisableRender") != null;
		}
		set
		{
			Transform pTransform = transform.Find("DisableRender");

			if (value)
			{
				if (pTransform == null)
				{
					GameObject pPrefab = Resources.Load<GameObject>("UI/DisableRender");
					GameObject pGameObject = Instantiate(pPrefab);
					pGameObject.name = @"DisableRender";
					pGameObject.transform.SetParent(transform, false);
					pGameObject.GetComponent<DisableRender>().Duration = m_DisableRenderDuration;
				}
				else
				{
					DisableRender pDisableRender = pTransform.GetComponent<DisableRender>();
					pDisableRender.Duration = m_DisableRenderDuration;
				}
			}
			else
			{
				if (pTransform != null)
				{
					DisableRender pDisableRender = pTransform.GetComponent<DisableRender>();
					Destroy(pDisableRender.gameObject);
				}
			}
		}
	}

	public bool Invalidate
	{
		get
		{
			return m_Invalidate;
		}
		set
		{
			m_Invalidate = value;
		}
	}

	public int RenderOrder
	{
		get
		{
			return m_RenderOrder;
		}
		set
		{
			m_RenderOrder = value;
		}
	}

	private void Update()
	{
		if (m_Invalidate)
		{
			m_Invalidate = false;

			RefreshView();
		}
	}

	public object Data
	{
		set
		{
			m_Data = value;

			OnSetData();
		}
		get
		{
			return m_Data;
		}
	}

	protected virtual void OnSetData()
	{
		m_Invalidate = true;
	}

	protected virtual void RefreshView()
	{
	}


	public virtual void OnClickRender(Vector3 pClickPosition)
	{
#if UNITY_EDITOR
		Debug.Log("Render Click");

		if (Data != null)
		{
			Debug.Log("Data Type : " + Data.ToString());

			if (Data is TDataVO)
			{
				Debug.Log("Data ID : " + (Data as TDataVO).ID);
			}
		}
#endif
	}

	public virtual void OnSelect()
	{
	}

	public virtual void OnDisSelect()
	{
	}

	protected void Recycle()
	{
		SimpleVList pSimpleVList = transform.GetComponentInParent<SimpleVList>();

		if (pSimpleVList != null && pSimpleVList.ReuseRenders)
		{
			pSimpleVList.RecycleRender(gameObject);
		}
		else
		{
			Destroy(gameObject);
		}
	}

	void IPointerClickHandler.OnPointerClick(PointerEventData pPointerEventData)
	{
		if (!DisableRender)
		{
			OnClickRender(pPointerEventData.position);

			if (m_UpdateSelectRender && transform.parent.GetComponent<SimpleVList>() != null)
			{
				transform.parent.GetComponent<SimpleVList>().RefreshSelectedRender(gameObject);
			}
		}
	}
}
