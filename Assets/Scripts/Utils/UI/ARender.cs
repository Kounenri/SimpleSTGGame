using UnityEngine;
using UnityEngine.EventSystems;

public class ARender : MonoBehaviour, IPointerClickHandler
{
	[SerializeField]
	protected bool m_UpdateSelectRender = false;

	private int m_nRenderOrder = 0;
	private object m_pData;
	private float m_fDisableRenderDuration = 2f;
	private bool m_bInvalidate = false;

	public float disableRenderDuration
	{
		get
		{
			return m_fDisableRenderDuration;
		}
		set
		{
			m_fDisableRenderDuration = value;
		}
	}

	public bool disableRender
	{
		get
		{
			return transform.Find(@"DisableRender") != null;
		}
		set
		{
			Transform pTransform = transform.Find(@"DisableRender");

			if (value)
			{
				if (pTransform == null)
				{
					GameObject pPrefab = Resources.Load<GameObject>(@"ui/DisableRender");
					GameObject pGameObject = Instantiate(pPrefab) as GameObject;
					pGameObject.name = @"DisableRender";
					pGameObject.transform.SetParent(transform, false);
					pGameObject.GetComponent<DisableRender>().Duration = m_fDisableRenderDuration;
				}
				else
				{
					DisableRender pDisableRender = pTransform.GetComponent<DisableRender>();
					pDisableRender.Duration = m_fDisableRenderDuration;
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

	public bool invalidate
	{
		get
		{
			return m_bInvalidate;
		}
		set
		{
			m_bInvalidate = value;
		}
	}

	public int renderOrder
	{
		get
		{
			return m_nRenderOrder;
		}
		set
		{
			m_nRenderOrder = value;
		}
	}

	void Update()
	{
		if (m_bInvalidate)
		{
			m_bInvalidate = false;

			RefreshView();
		}
	}

	public object data
	{
		set
		{
			m_pData = value;
			setFontSize();

			OnSetData();
		}
		get
		{
			return m_pData;
		}
	}

	protected virtual void OnSetData()
	{
		m_bInvalidate = true;


	}

	bool _haveSetFontSize = false;
	void setFontSize()
	{
		if (_haveSetFontSize) return;
		_haveSetFontSize = true;
	}


	protected virtual void RefreshView()
	{
	}


	public virtual void OnClickRender(Vector3 pClickPosition)
	{
#if UNITY_EDITOR
		Debug.Log(@"Render Click");

		if (data != null)
		{
			Debug.Log(@"Data Type : " + data.ToString());

			//if(data is TDataVO)
			//{
			//	Debug.Log(@"Data ID : " + (data as TDataVO).ID);
			//}
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

		if (pSimpleVList != null && pSimpleVList.reuseRenders)
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
		if (!disableRender)
		{
			OnClickRender(pPointerEventData.position);

			if (m_UpdateSelectRender && transform.parent.GetComponent<SimpleVList>() != null)
			{
				transform.parent.GetComponent<SimpleVList>().refreshSelectedRender(gameObject);
			}
		}
	}
}
