using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SimpleVList : MonoBehaviour
{
	public GameObject emptyPanel;
	public GameObject renderPrefab;
	public bool reuseRenders = true;
	protected bool _needRefreshView = false;
	protected List<System.Object> _dataProvider = null;
	protected bool _hasDoneRefresh;

	public virtual List<System.Object> dataProvider
	{
		get
		{
			return _dataProvider;
		}
		set
		{
			_dataProvider = value;
			_needRefreshView = true;
			_hasDoneRefresh = false;
			if (emptyPanel != null)
			{
				if (_dataProvider == null || _dataProvider.Count == 0)
					emptyPanel.SetActive(true);
				else
					emptyPanel.SetActive(false);
			}
			PlayShowTween();

		}
	}

	public bool hasData
	{
		get
		{
			return _dataProvider != null && _dataProvider.Count > 0;
		}
	}

	public bool hasDoneRefresh
	{
		get
		{
			return _hasDoneRefresh;
		}
	}

	public bool clearRendersWhenAwake = true;
	void Awake()
	{
		if (clearRendersWhenAwake)
		{
			foreach (Transform childTransform in this.transform)
			{
				if (childTransform.GetComponent<ARender>() != null)
					Destroy(childTransform.gameObject);
			}
		}

		onInit();
	}


	protected virtual void onInit()
	{

	}

	/*
	private bool _isDown = false;
	private bool _isDrag = false;
	private Vector3 _startPos = new Vector3();
	void Update()
	{
		if(Input.GetMouseButtonDown(0))
		{

			_isDrag = false;
			Ray ray = UICamera.Instance.camera.ScreenPointToRay (Input.mousePosition);
			RaycastHit2D hit = Physics2D.GetRayIntersection (ray);
			if ( hit.collider != null) {
				if (hit.collider.GetComponent<ARender>()!=null)
				{
					_startPos = Input.mousePosition;
					_isDown = true;
				}
			}
		}

		if(_isDown)
		{
			if(Vector3.Distance(Input.mousePosition,_startPos)>15f)
			{
				_isDrag = true;
			}
		}

		if(Input.GetMouseButtonUp(0))
		{
			Ray ray = UICamera.Instance.camera.ScreenPointToRay (Input.mousePosition);
			RaycastHit2D hit = Physics2D.GetRayIntersection (ray);
			if ( hit.collider != null) {
				if (hit.collider.GetComponent<ARender>()!=null && _isDrag == false)
				{
					SendMessageUpwards("onClickRender",hit.collider.gameObject,SendMessageOptions.DontRequireReceiver);
					hit.collider.GetComponent<ARender>().onClick();
					refreshSelectedRender(hit.collider.gameObject);
					DebugUtil.Log("click render");
				}
			}
			_isDrag = false;
		}
	}
    */

	protected enum ShowType
	{
		None,
		MoveSize,
		FillMount
	}

	[SerializeField] protected ShowType isPlayListShow = ShowType.None;
	public bool isLoopShowList = false;
	[SerializeField] protected float ShowTime = 0f;
	[SerializeField] protected float ShowSpeed = 0.2f;
	private ScrollRect m_scroll;
	private RectTransform m_viewport;
	private CanvasGroup m_canvas;
	protected virtual void PlayShowTween()
	{
		if (isPlayListShow != ShowType.None || isLoopShowList)
		{
			m_scroll = transform.GetComponentInParent<ScrollRect>();
			if (m_scroll == null && transform.parent != null && transform.parent.parent != null)
				m_scroll = transform.parent.parent.GetComponent<ScrollRect>();

			if (m_scroll != null && m_scroll.viewport != null)
			{
				if (m_scroll.viewport.GetComponent<Mask>() != null)
				{
					m_viewport = m_scroll.viewport;
					m_canvas = m_scroll.viewport.GetComponent<CanvasGroup>() ? m_scroll.viewport.gameObject.GetComponent<CanvasGroup>() : m_scroll.viewport.gameObject.AddComponent<CanvasGroup>();
					m_canvas.alpha = 0;
					m_canvas.blocksRaycasts = false;

					Invoke("DoPlay", ShowTime);
				}
				else
				{
					if (m_scroll.GetComponent<Mask>() != null)
					{
						m_viewport = m_scroll.GetComponent<RectTransform>();
						m_canvas = m_scroll.GetComponent<CanvasGroup>() ? m_scroll.gameObject.GetComponent<CanvasGroup>() : m_scroll.gameObject.AddComponent<CanvasGroup>();
						m_canvas.alpha = 0;
						m_canvas.blocksRaycasts = false;

						Invoke("DoPlay", ShowTime);
					}
				}


			}
			else
			{
				if (!isLoopShowList)
					isPlayListShow = ShowType.None;
			}


		}
	}

	private void DoPlay()
	{
		switch (isPlayListShow)
		{
			case ShowType.None:
				break;
			case ShowType.MoveSize:
				{
					bool IsVisibility = false;
					m_canvas.DOFade(1, ShowSpeed).SetEase(Ease.Linear).OnComplete(() => { m_canvas.blocksRaycasts = true; });
					if (m_scroll.vertical)
					{
						if (m_scroll.verticalScrollbarVisibility == ScrollRect.ScrollbarVisibility.AutoHideAndExpandViewport)
						{
							m_scroll.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHide;
							IsVisibility = true;
						}
						float baseY = -m_scroll.GetComponent<RectTransform>().rect.height;
						Vector2 thisVer = m_viewport.sizeDelta;
						m_viewport.sizeDelta = new Vector2(thisVer.x, baseY);
						m_viewport.DOSizeDelta(thisVer, ShowSpeed).SetEase(Ease.Linear).OnComplete(() =>
						{

							if (IsVisibility)
							{
								m_scroll.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHideAndExpandViewport;
							}
						});


					}
					if (m_scroll.horizontal)
					{
						if (m_scroll.horizontalScrollbarVisibility == ScrollRect.ScrollbarVisibility.AutoHideAndExpandViewport)
						{
							m_scroll.horizontalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHide;
							IsVisibility = true;
						}
						float baseX = -m_scroll.GetComponent<RectTransform>().rect.width;
						Vector2 thisVer = m_viewport.sizeDelta;
						m_viewport.sizeDelta = new Vector2(baseX, thisVer.y);
						m_viewport.DOSizeDelta(thisVer, ShowSpeed).SetEase(Ease.Linear).OnComplete(() =>
						{
							if (IsVisibility)
							{
								m_scroll.horizontalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHideAndExpandViewport;
							}
						});
					}

				}
				break;
			case ShowType.FillMount:
				{
					m_canvas.DOFade(1, ShowSpeed).SetEase(Ease.Linear).OnComplete(() => { m_canvas.blocksRaycasts = true; });
					if (m_scroll.vertical)
					{
						Image tmpImage = m_viewport.transform.GetComponent<Image>() ? m_viewport.gameObject.GetComponent<Image>() : m_viewport.gameObject.AddComponent<Image>();
						tmpImage.type = Image.Type.Filled;
						tmpImage.fillMethod = Image.FillMethod.Vertical;
						tmpImage.fillOrigin = (int)Image.OriginVertical.Top;
						tmpImage.fillAmount = 0;
						tmpImage.DOFillAmount(1, ShowSpeed);
					}
					else if (m_scroll.horizontal)
					{
						Image tmpImage = m_viewport.transform.GetComponent<Image>() ? m_viewport.gameObject.GetComponent<Image>() : m_viewport.gameObject.AddComponent<Image>();
						tmpImage.type = Image.Type.Filled;
						tmpImage.fillMethod = Image.FillMethod.Horizontal;
						tmpImage.fillOrigin = (int)Image.OriginHorizontal.Left;
						tmpImage.fillAmount = 0;
						tmpImage.DOFillAmount(1, ShowSpeed);
					}

				}
				break;
			default:
				break;
		}

		if (!isLoopShowList)
			isPlayListShow = ShowType.None;

	}

	protected virtual GameObject getRenderPrefab(System.Object vo)
	{
		return renderPrefab;
	}

	protected GameObject _selectedRender = null;

	public void refreshSelectedRender(GameObject render)
	{
		_selectedRender = null;
		foreach (Transform childTransform in this.transform)
		{
			if (childTransform.gameObject.activeSelf == false || childTransform.GetComponent<ARender>() == null)
				continue;

			if (render == childTransform.gameObject)
			{
				childTransform.GetComponent<ARender>().OnSelect();
				_selectedRender = childTransform.gameObject;
			}
			else
			{
				childTransform.GetComponent<ARender>().OnDisSelect();
			}
		}

		if (onSelectRenderHandler != null)
		{
			onSelectRenderHandler(_selectedRender);
		}
	}

	public Action<GameObject> onSelectRenderHandler = null;

	public Vector2 getRenderSize()
	{
		ARender r = this.GetComponentInChildren<ARender>();
		if (r != null && r.gameObject.activeSelf)
		{
			return new Vector2(r.GetComponent<RectTransform>().rect.width, r.GetComponent<RectTransform>().rect.height);
		}
		return new Vector2();
	}

	public Vector2 getRenderSize(int dex)
	{
		GameObject obj = getRender(dex);
		if (obj != null)
		{

			return obj.GetComponent<RectTransform>().sizeDelta;
		}

		return new Vector2();
	}

	public Vector2 getRenderPos(int dex)
	{

		GameObject obj = getRender(dex);
		if (obj != null)
		{

			return obj.GetComponent<RectTransform>().offsetMin;
		}

		return new Vector2();

	}

	public GameObject getRender(int dex)
	{
		if (dex >= transform.childCount)
			return null;

		GameObject obj = transform.GetChild(dex).gameObject;
		if (obj.activeSelf == false || obj.GetComponent<ARender>() == null)
		{
			return getRender(dex + 1);
		}
		return obj;
	}

	public object getRenderDate(int dex)
	{
		GameObject render = getRender(dex);
		if (render != null)
		{
			ARender arend = render.GetComponent<ARender>();
			return arend.data;
		}
		else
		{
			return null;
		}
	}

	public List<Transform> GetChildList()
	{
		List<Transform> pList = new List<Transform>();

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

	/// <summary>
	/// 避免用上面那个list 多次 GetComponent
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <returns></returns>
	public List<T> GetChildList<T>() where T : ARender
	{
		List<T> pList = new List<T>();

		for (int i = 0; i < transform.childCount; i++)
		{
			Transform pTransform = transform.GetChild(i);
			var tmp = pTransform.GetComponent<T>();
			if (pTransform.gameObject.activeSelf && tmp != null)
			{
				pList.Add(tmp);
			}
		}
		return pList;
	}

	public int getRenderNum()
	{
		int res = 0;
		for (int i = 0; i < transform.childCount; i++)
		{
			if (transform.GetChild(i).gameObject.activeSelf == true && transform.GetChild(i).GetComponent<ARender>() != null)
			{
				res++;
			}
		}
		return res;
	}

	public GameObject getSelectedRender()
	{
		return _selectedRender;
	}

	void LateUpdate()
	{
		if (_needRefreshView)
		{
			refreshView();
			_needRefreshView = false;
		}
	}

	protected virtual void refreshView()
	{

		_selectedRender = null;

		foreach (Transform childTransform in this.transform)
		{
			ARender temp = childTransform.GetComponent<ARender>();
			if (temp != null)
			{
				if (reuseRenders)
				{

					RecycleRender(childTransform.gameObject);
				}
				else
				{
					Destroy(childTransform.gameObject);
				}
			}
		}

		if (_dataProvider == null)
		{
			return;
		}
		//int startSiblingDex = transform.childCount;
		for (int i = 0; i < _dataProvider.Count; i++)
		{
			GameObject obj = createRender(_dataProvider[i], i);
			obj.GetComponent<ARender>().renderOrder = i;
			obj.transform.SetSiblingIndex(transform.childCount);
		}

		AdjustSibling();

		_hasDoneRefresh = true;
	}

	private List<GameObject> _reuseRenderList = new List<GameObject>();
	/// <summary>
	/// 是否顺序渲染 解决数据刷新时闪屏问题。
	/// </summary>
	public bool isOrderRender;

	protected GameObject getReuseRender()
	{
		int count = _reuseRenderList.Count;

		if (count <= 0)
			return null;

		if (isOrderRender)
		{
			GameObject tmp = _reuseRenderList.First();
			if (tmp.GetComponent<ARender>() == null)
			{
				_reuseRenderList.RemoveAt(0);
				return getReuseRender();
			}
			_reuseRenderList.RemoveAt(0);
			tmp.SetActive(true);
			return tmp;
		}

		GameObject obj = _reuseRenderList[count - 1];
		if (obj.GetComponent<ARender>() == null)
		{
			_reuseRenderList.Remove(obj);
			return null;
		}
		_reuseRenderList.RemoveAt(count - 1);
		obj.SetActive(true);
		return obj;
	}

	public void RecycleRender(GameObject pRender)
	{
		ARender pARender = pRender.GetComponent<ARender>();

		if (pARender != null && pARender.enabled)
		{
			pRender.SetActive(false);

			if (_reuseRenderList.IndexOf(pRender) == -1)
				_reuseRenderList.Add(pRender);

			ARender[] aRenders = transform.GetComponentsInChildren<ARender>();

			for (int i = 0; i < aRenders.Length; i++)
			{
				aRenders[i].renderOrder = i;
			}
		}
	}

	protected virtual GameObject createRender(object pData, int nIndex = 0, int siblingIndex = -1)
	{

		GameObject obj = getReuseRender();
		if (obj == null)
		{
			obj = Instantiate(getRenderPrefab(pData)) as GameObject;
			obj.GetComponent<RectTransform>().SetParent(gameObject.GetComponent<RectTransform>(), false);
			if (siblingIndex >= 0) obj.transform.SetSiblingIndex(nIndex);
		}

		ARender render = obj.GetComponent<ARender>();

		if (render != null)
		{
			render.renderOrder = nIndex;
			render.data = pData;
		}

		return obj;
	}

	void AdjustSibling()
	{
		int len = transform.childCount;
		for (int i = 0; i < len; i++)
		{
			KeepRenderSibling r = transform.GetChild(i).GetComponent<KeepRenderSibling>();
			if (r != null)
			{
				if (r.KeepInFirst)
				{
					r.GetComponent<RectTransform>().SetAsFirstSibling();
				}
				else
				{
					r.GetComponent<RectTransform>().SetAsLastSibling();
				}
			}
		}
	}

	public void addData(System.Object data, bool insertToLast = true)
	{
		List<System.Object> arr = new List<object>() { data };
		if (insertToLast)
		{
			insertLast(arr);
		}
		else
		{
			insertFirst(arr);
		}
	}

	public void insert(List<System.Object> arr, int dex)
	{
		if (_dataProvider == null)
			_dataProvider = new List<object>();



		_dataProvider.InsertRange(dex, arr);

		int len = arr.Count;
		for (int i = 0; i < len; i++)
		{
			GameObject render = createRender(arr[i]);

			render.GetComponent<RectTransform>().SetSiblingIndex(dex + i);
		}

		AdjustSibling();
	}




	public void insertLast(List<System.Object> arr)
	{

		if (_dataProvider == null)
			_dataProvider = new List<object>();

		_dataProvider.InsertRange(_dataProvider.Count, arr);

		int len = arr.Count;
		for (int i = 0; i < len; i++)
		{
			GameObject render = createRender(arr[i]);

			render.GetComponent<RectTransform>().SetSiblingIndex(render.transform.parent.childCount - 1);
		}

		AdjustSibling();
	}

	public void insertFirst(List<System.Object> arr)
	{
		if (_dataProvider == null)
			_dataProvider = new List<object>();
		_dataProvider.InsertRange(0, arr);

		int len = arr.Count;
		for (int i = 0; i < len; i++)
		{
			GameObject render = createRender(arr[i]);

			render.GetComponent<RectTransform>().SetSiblingIndex(i);
		}

		AdjustSibling();
	}


	public void removeData(object data)
	{

		if (data == null)
		{
			Debug.LogError("不能删除data为null的render");
		}

		List<Transform> renders = GetChildList();
		Transform r = renders.Find((p) => { return p.GetComponent<ARender>().data == data; });

		if (r != null)
		{
			Destroy(r.gameObject);
		}

		int dex = _dataProvider.FindIndex((p) =>
		{


			if (data == p) return true;

			return false;
		});

		if (dex >= 0) _dataProvider.RemoveAt(dex);

	}


	void OnDestroy()
	{
		_reuseRenderList = new List<GameObject>();
	}
}
