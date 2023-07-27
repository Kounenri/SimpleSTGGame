using System;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ScrollRecycler
{
	[RequireComponent(typeof(ScrollRect))]
	public abstract class ScrollRecycleBase : MonoBehaviour
	{
		public RecycleItemRender PrefItem; //克隆体
		public GameObject emptyPanel;
		[SerializeField] protected bool usePrefabSize;

		[SerializeField] protected bool usePrefabMargin;  // 使用预设上的 Offset,当usePrefabSize = true 时有效.
		[SerializeField] protected int itemWidth = 50; //单元格宽
		[SerializeField] protected int itemHeight = 50; //单元格高

		[Header("自动适配")][SerializeField] protected bool AdapterOnInit = false;
		[SerializeField] protected bool AdapterWidthOrHeight;//表示只显示一行或一列 自适应宽或高

		[Header("显示行列数")][SerializeField] protected int rowCount = 2;
		[SerializeField] protected int columnCount = 2;
		[Header("横纵间距")][SerializeField] protected int offsetX = 10;
		[SerializeField] protected int offsetY = 10;
		[SerializeField] protected RectOffset padding;


		private ScrollRect scrollRect;
		protected RectTransform itemParent;

		// 计算数据
		protected int createCount; //创建的ITEM数量
		protected int rectWidth; //列表宽度
		protected int rectHeight; //列表高度
		protected int listCount;
		private int showCount;
		private int lastStartIndex;
		private int startIndex;
		private int endIndex;

		private Dictionary<int, Transform> dicItems = new Dictionary<int, Transform>(); //item对应的序号
		private Vector3 curItemParentPos = Vector3.zero;

		private RecycleGetItem onRecycleGetItem;
		private bool isInited;

		private List<int> _newIndexList = new List<int>();
		private List<int> _changeIndexList = new List<int>();

		protected bool _hasAdapter;


		#region 仅兼容性功能

		private bool _needRefreshView;
		private List<IRecycleData> _list;

		public List<IRecycleData> dataProvider
		{
			get
			{
				return _list;
			}
			set
			{
				_list = value;
				if (emptyPanel != null)
				{
					if (_list == null || _list.Count == 0)
						emptyPanel.SetActive(true);
					else
						emptyPanel.SetActive(false);
				}
				RefreshAdapter();
				_needRefreshView = true;
			}
		}

		public delegate void TutoDelagate();
		public TutoDelagate OnRefreshRender;

		void LateUpdate()
		{
			if (_needRefreshView && _list != null)
			{
				RefreshList(_list.Count, (index) => _list[index]);
				_needRefreshView = false;
			}
		}

		#endregion

		protected virtual void RefreshAdapter()
		{

		}

		protected enum ShowType
		{
			None,
			MoveSize,//受锚点位置影响
			FillMount//需要在Mask添加Image并设置图骗 ‘Ui_BG_Mask’
		}

		[Header("是否播放刷新特效")][SerializeField] protected ShowType isPlayListShow = ShowType.None;
		[Header("刷新数据延迟播放时间")][SerializeField] protected float ShowTime = 0f;
		[Header("特效播放速度")][SerializeField] protected float ShowSpeed = 0.2f;
		private ScrollRect m_scroll;
		private RectTransform m_viewport;
		private CanvasGroup m_canvas;
		protected virtual void PlayShowTween()
		{
			if (isPlayListShow != ShowType.None)
			{

				m_scroll = GetComponent<ScrollRect>();
				if (m_scroll != null)
				{

					if (m_scroll.viewport != null)
					{
						m_viewport = m_scroll.viewport;

						m_canvas = m_scroll.viewport.GetComponent<CanvasGroup>() ? m_scroll.viewport.gameObject.GetComponent<CanvasGroup>() : m_scroll.viewport.gameObject.AddComponent<CanvasGroup>();
						m_canvas.alpha = 0;
						m_canvas.blocksRaycasts = false;

					}
					else
					{
						if (m_scroll.GetComponent<Mask>() != null)
						{
							m_viewport = m_scroll.GetComponent<RectTransform>();
							m_canvas = m_scroll.GetComponent<CanvasGroup>() ? m_scroll.gameObject.GetComponent<CanvasGroup>() : m_scroll.gameObject.AddComponent<CanvasGroup>();
							m_canvas.alpha = 0;
							m_canvas.blocksRaycasts = false;

						}
					}

					Invoke(nameof(DoPlay), ShowTime);
				}
				else
				{
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
						m_canvas.DOFade(1, 0.2f).SetEase(Ease.Linear).OnComplete(() => { m_canvas.blocksRaycasts = true; });
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
			isPlayListShow = ShowType.None;
		}


		/// <summary>
		/// 初始化/刷新列表数据,并回滚到顶部
		/// </summary>
		public void InitList(int count, RecycleGetItem recycleGetAction, Action<bool, RectTransform, Vector2> onResized = null)
		{
			var isDoInit = false;
			if (!isInited)
			{
				isDoInit = true;
				InitData();
			}

			listCount = count;
			onRecycleGetItem = recycleGetAction;
			itemParent.transform.localPosition = Vector2.zero;

			SetVertHeightOrHorWid(listCount);

			var size = new Vector2(rectWidth, rectHeight);
			itemParent.sizeDelta = size;

			// 重新计算列表anchorPOS的回调,必须放在 sizeDelta 之后 以及 ResetStarIndex之前.
			onResized?.Invoke(isDoInit, itemParent, size);

			ResetStarIndex();

			onResized?.Invoke(isDoInit, itemParent, size);
			showCount = Mathf.Min(listCount, createCount);
			dicItems.Clear();

			for (int i = startIndex; i <= endIndex; i++)
			{
				Transform item = GetItem(i - startIndex);
				SetItem(item, i);
			}

			ShowListCount(itemParent, showCount);
			_needRefreshView = false;
		}


		/// <summary>
		/// 仅刷新列表数据,不回滚
		/// </summary>
		/// <param name="count">列表的元素的最大个数</param>
		/// <param name="updateItem">单个元素的赋值</param>
		public void RefreshList(int count, RecycleGetItem updateItem, Action<bool, RectTransform, Vector2> onResized = null)
		{
			var isDoInit = false;
			if (!isInited)
			{
				isDoInit = true;
				InitData();
			}

			onRecycleGetItem = updateItem;
			SetVertHeightOrHorWid(count);

			listCount = count;

			var size = new Vector2(rectWidth, rectHeight);
			itemParent.sizeDelta = size;
			onResized?.Invoke(isDoInit, itemParent, size);

			showCount = Mathf.Min(count, createCount); //显示item的数量
			dicItems.Clear();
			if (count == 0)
			{
				ShowListCount(itemParent, showCount);
				return;
			}

			ResetStarIndex();

			if (OnRefreshRender != null) OnRefreshRender();
			lastStartIndex = startIndex;
			if (endIndex < startIndex)
			{
				Debug.LogError("列表有问题！");
				return;
			}

			for (int i = startIndex; i <= endIndex; i++)
			{
				Transform item = GetItem(i - startIndex);
				SetItem(item, i);
			}

			ShowListCount(itemParent, showCount);
			_needRefreshView = false;
		}

		private void ResetStarIndex()
		{
			if (listCount <= createCount)
			{
				startIndex = 0;
				endIndex = listCount - 1;
			}
			else
			{
				startIndex = GetStartIndex(itemParent.localPosition);
				if (startIndex + createCount >= listCount)
				{
					startIndex = listCount - createCount;
					endIndex = listCount - 1;
				}
				else
				{
					endIndex = startIndex + createCount - 1;
				}
			}
		}


		private void OnValueChange(Vector2 pos)
		{
			curItemParentPos = itemParent.localPosition;
			if (listCount <= createCount)
				return;
			// 当前起始索引
			startIndex = GetStartIndex(itemParent.localPosition);
			if (startIndex + createCount >= listCount)
			{
				startIndex = listCount - createCount;
				endIndex = listCount - 1;
			}
			else
			{
				endIndex = startIndex + createCount - 1;
			}

			// 不需要刷新
			if (startIndex == lastStartIndex)
				return;
			lastStartIndex = startIndex;
			_newIndexList.Clear();
			_changeIndexList.Clear();
			for (int i = startIndex; i <= endIndex; i++)
			{
				_newIndexList.Add(i);
			}

			// ReSharper disable once GenericEnumeratorNotDisposed
			var e = dicItems.GetEnumerator();
			while (e.MoveNext())
			{
				int index = e.Current.Key;
				if (index >= startIndex && index <= endIndex)
				{
					if (_newIndexList.Contains(index))
						_newIndexList.Remove(index);
				}
				else
				{
					_changeIndexList.Add(e.Current.Key);
				}
			}
			bool updated = false;
			// 刷新新界面和索引
			for (int i = 0; i < _newIndexList.Count && i < _changeIndexList.Count; i++)
			{
				int oldIndex = _changeIndexList[i];
				int newIndex = _newIndexList[i];
				if (newIndex >= 0 && newIndex < listCount)
				{
					var item = dicItems[oldIndex];
					dicItems.Remove(oldIndex);
					SetItem(item, newIndex);
					updated = true;
					//TutoCheck.GetInstance.DispatchEvent(TutoCheck.Refresh_ScrollRecy);

				}

			}
			if (updated && OnRefreshRender != null) OnRefreshRender();

		}

		public void ResetPrefab(RecycleItemRender pref)
		{
			if (pref != null)
			{
				PrefItem = pref;
			}

			if (scrollRect != null && scrollRect.content != null)
			{
				for (int i = scrollRect.content.childCount - 1; i >= 0; i--)
				{
					DestroyImmediate(scrollRect.content.GetChild(i));
				}
			}
			dicItems?.Clear();
		}

		private Transform GetItem(int index)
		{
			Transform item;
			if (index < itemParent.childCount)
				item = itemParent.GetChild(index);
			else
				item = (Instantiate(PrefItem.gameObject, itemParent)).transform;
			//            item.name = PrefItem.name + index;
			//            item.SetParent(itemParent);
			item.localScale = Vector3.one;
			if (!usePrefabSize)
			{
				var rect = item.GetComponent<RectTransform>();
				if (rect) rect.sizeDelta = new Vector2(itemWidth, itemHeight);
			}

			return item;
		}


		public System.Action<IRecycleData> OnSelectData;

		private int selectIndex = -1;

		public int SelectIndex
		{
			set
			{
				selectIndex = value;
				SelectItem();
			}
			get { return selectIndex; }
		}

		private GameObject selectGameobject;

		public GameObject SelectGameobject
		{
			set { selectGameobject = value; }
			get { return selectGameobject; }
		}

		public IRecycleData SelectData
		{
			get
			{
				if (selectIndex >= 0 && selectIndex < _list.Count)
				{
					return _list[selectIndex];
				}
				else
				{
					return null;
				}
			}
			set
			{
				if (_list != null)
				{
					IRecycleData data = _list.Find(x => x == value);
					if (data != null)
					{
						selectIndex = _list.FindIndex(x => x == data);
					}
				}
			}
		}

		private void SelectItem()
		{
			if (itemParent == null)
			{
				itemParent = GetComponent<ScrollRect>().content;
			}
			for (int i = 0; i < itemParent.childCount; i++)
			{
				RecycleItemRender render = itemParent.GetChild(i).GetComponent<RecycleItemRender>();
				if (render != null)
				{
					if (render.index != selectIndex)
					{
						render.OnDisSelect();
					}
					else
					{
						render.OnSelect();
						if (OnSelectData != null)
						{
							OnSelectData(_list[selectIndex]);
						}
					}
				}
			}
		}

		/// <summary>
		/// 刷新item对应数据信息
		/// </summary>
		/// <param name="item"></param>
		/// <param name="index"></param>
		private void SetItem(Transform item, int index)
		{
			// 不用 _list 初始化时直接放行
			if (_list == null || _list.Count > index)
			{
				dicItems[index] = item;
				item.localPosition = GetPos(index);
				//                item.name = PrefItem.name + index;
				item.GetComponent<RecycleItemRender>().Refresh(onRecycleGetItem(index), index, index == selectIndex);
			}
		}

		/// <summary>
		/// 显示子物体的数量
		/// </summary>
		/// <param name="trans"></param>
		/// <param name="num"></param>
		private void ShowListCount(Transform trans, int num)
		{
			if (trans.childCount < num)
				return;
			for (int i = 0; i < num; i++)
			{
				trans.GetChild(i).gameObject.SetActive(true);
			}

			for (int i = num; i < trans.childCount; i++)
			{
				trans.GetChild(i).gameObject.SetActive(false);
			}
		}

		/// <summary>
		/// 设置 item长宽，行列数
		/// </summary>
		/// <param name="width">item宽</param>
		/// <param name="height">item长</param>
		/// <param name="column"></param>
		/// <param name="row">一个列表最多能显示多少行（元素）</param>
		/// <param name="isUseItemSize"></param>
		public void SetMargin(int width, int height, int column, int row, bool isUseItemSize = false)
		{
			usePrefabSize = isUseItemSize;
			itemWidth = width;
			itemHeight = height;
			columnCount = column;
			rowCount = row;
			InitData();
		}

		/// <summary>
		/// 设置列表的 item长宽，列数和行,初始化事件等
		/// 该设置会在下次 initList 或 refreshList 生效
		/// </summary>
		private void InitData()
		{
			if (PrefItem == null)
			{
				Debug.LogError("Prefab item is null");
				return;
			}

			createCount = columnCount * rowCount;
			if (createCount <= 0)
			{
				Debug.LogError("横纵不能为0！");
				return;
			}

			scrollRect = transform.GetComponent<ScrollRect>();
			if (scrollRect)
			{
				scrollRect.onValueChanged.AddListener(OnValueChange);
			}

			itemParent = scrollRect.content;
			itemParent.pivot = new Vector2(0, 1);

			RectTransform rec = PrefItem.GetComponent<RectTransform>();
			var oldMin = rec.anchorMin;
			var oldMax = rec.anchorMax;
			rec.anchorMin = new Vector2(0, 1);
			rec.anchorMax = new Vector2(0, 1);
			rec.pivot = new Vector2(0, 1);
			if (usePrefabSize)
			{
				itemWidth = (int)rec.sizeDelta.x;
				itemHeight = (int)rec.sizeDelta.y;

				if (usePrefabMargin)
				{
					// 未使用 预设尺寸时可还原原本的margin
					rec.anchorMin = oldMin;
					rec.anchorMax = oldMax;
				}
			}
			isInited = true;

			PlayShowTween();
		}

		/// <summary>
		/// 设置元素之间的间距 spacing
		/// 该设置会在下次 initList 或 refreshList 生效
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		public void SetOffset(int x, int y)
		{
			offsetX = x;
			offsetY = y;
			SetVertWidOrHorHeightOnOffset();
		}

		#region 横向或纵向复写方法

		protected abstract void SetVertWidOrHorHeightOnOffset();

		protected abstract void SetVertHeightOrHorWid(int count);

		/// <summary>
		/// 获取当前点的位置
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		protected abstract Vector2 GetPos(int index);

		/// <summary>
		/// 获取当前点的index索引
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		protected abstract int GetStartIndex(Vector2 pos);

		#endregion

		#region 兼容性功能

		private GameObject m_SelectedRender;

		public void RefreshSelectedRender(GameObject render)
		{
			foreach (Transform childTransform in scrollRect.content.transform)
			{
				if (childTransform.gameObject.activeSelf == false || childTransform.GetComponent<RecycleItemRender>() == null)
					continue;

				if (render == childTransform.gameObject)
				{
					SelectIndex = childTransform.GetComponent<RecycleItemRender>().index;
					break;
				}
			}
		}

		public GameObject GetSelectedRender()
		{
			return m_SelectedRender;
		}

		public GameObject GetRender(int dex)
		{
			if (dex >= transform.childCount)
				return null;

			GameObject obj = scrollRect.content.transform.GetChild(dex).gameObject;
			if (obj.activeSelf == false || obj.GetComponent<RecycleItemRender>() == null)
			{
				return GetRender(dex + 1);
			}
			return obj;
		}

		#endregion
	}

	public delegate IRecycleData RecycleGetItem(int index);
}