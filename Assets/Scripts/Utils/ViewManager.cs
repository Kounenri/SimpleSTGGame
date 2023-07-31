using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class ViewManager : TMonoEventDispatcher<ViewManager>, IDispatcher
{
	public const string ON_VIEW_CHANGE = @"ON_VIEW_CHANGE";

	private List<ViewData> m_ViewDataList;
	private List<PopUpData> m_PopUpDataList;
	private LinkedList<ViewData> m_SeriesPopUpDataList;
	private Scene m_Scene;
	private int m_TotalViewCount = -1;
	private int m_ViewCount = -1;
	private int m_UICount = -1;

	public int TotalViewCount
	{
		get
		{
			return m_TotalViewCount;
		}
	}

	public int ViewCount
	{
		get
		{
			return m_ViewCount;
		}
	}

	private bool CanPopUp
	{
		get
		{
			return m_Scene.name == LevelNameEnum.GameScene;
		}
	}

	protected override void OnInitialize()
	{
		base.OnInitialize();

		m_ViewDataList = new List<ViewData>();
		m_PopUpDataList = new List<PopUpData>();
		m_SeriesPopUpDataList = new LinkedList<ViewData>();

		SceneManager.sceneLoaded += OnLevelLoaded;

		m_IsInit = true;
	}

	private void OnLevelLoaded(Scene arg0, LoadSceneMode arg1)
	{
		m_Scene = arg0;

		if (m_IsInit)
		{
			m_ViewDataList.Clear();
		}
	}

	public void ShowBaseCanvas(GameObject pGameObject, Canvas pCanvas, CanvasType pCanvasType)
	{
		if (UICamera.HasInstance)
		{
			SetCamera(pCanvas, UICamera.GetInstance.Camera);

			if (HasInstance)
			{
				ViewData data = new ViewData(pGameObject, pCanvas, pCanvasType);
				m_ViewDataList.Add(data);

				if (IsInvoking(@"OnBaseCanvasChange"))
				{
					CancelInvoke(@"OnBaseCanvasChange");
				}

				Invoke(@"OnBaseCanvasChange", 0.1f);
			}
		}
		else
		{
			SetCamera(pCanvas, Camera.main);
		}
	}

	public void SetBaseCanvas(GameObject pGameObject, Canvas pCanvas, CanvasType pCanvasType)
	{
		if (m_IsInit)
		{
			if (m_ViewDataList.Count > 0)
			{
				int nIndex = m_ViewDataList.FindIndex(pData =>
				{
					return pData.InstanceID == pGameObject.GetInstanceID();
				});

				if (nIndex != -1)
				{
					ViewData pBlurControllerData = m_ViewDataList[nIndex];

					pBlurControllerData.Canvas = pCanvas;
					pBlurControllerData.CanvasType = pCanvasType;
					pBlurControllerData.InstanceID = pGameObject.GetInstanceID();

					if (IsInvoking(@"OnBaseCanvasChange"))
					{
						CancelInvoke(@"OnBaseCanvasChange");
					}

					Invoke(@"OnBaseCanvasChange", 0.1f);
				}
			}
		}
	}

	public void DestroyBaseCanvas(GameObject pGameObject)
	{
		if (m_IsInit)
		{
			if (m_ViewDataList.Count > 0)
			{
				int nInstanceID = pGameObject.GetInstanceID();

				int nIndex = m_ViewDataList.FindIndex(pData =>
				{
					return pData.InstanceID == nInstanceID;
				});
				ViewData nViewData = GetData(nInstanceID);
				if (nViewData != null)
				{
					m_SeriesPopUpDataList.Remove(nViewData);
				}

				if (nIndex != -1)
				{
					m_ViewDataList.RemoveAt(nIndex);

					if (IsInvoking(@"OnBaseCanvasChange"))
					{
						CancelInvoke(@"OnBaseCanvasChange");
					}

					Invoke(@"OnBaseCanvasChange", 0.1f);
				}
			}
		}
	}

	public void SetPopUp(PopUpData pPopUpData)
	{
		m_PopUpDataList.Add(pPopUpData);

		m_PopUpDataList.Sort((p1, p2) =>
		{
			return p1.Priotiry.CompareTo(p2.Priotiry);
		});
	}

	private void OnBaseCanvasChange()
	{
		if (m_IsInit)
		{
			m_TotalViewCount = 0;
			int nViewCount = 0;
			int nUICount = 0;

			for (int i = (m_ViewDataList.Count - 1); i >= 0; i--)
			{
				ViewData pViewData = m_ViewDataList[i];

				switch (pViewData.CanvasType)
				{
					case CanvasType.Static:
						break;
					case CanvasType.UI:
						m_TotalViewCount++;
						nViewCount++;
						nUICount++;
						break;
					case CanvasType.Popup:
						m_TotalViewCount++;
						nViewCount++;
						break;
					case CanvasType.Ignore:
						m_TotalViewCount++;
						break;
					default:
						Debug.LogError(pViewData.Canvas.name + @" need to be edit!");
						m_TotalViewCount++;
						nViewCount++;
						break;
				}
			}

			m_ViewCount = nViewCount;

			if (m_UICount == -1 || m_UICount != nUICount)
			{
				m_UICount = nUICount;

				bool bHasUI = m_UICount != 0;

				DispatchEvent(ON_VIEW_CHANGE, bHasUI);
			}

			if (IsInvoking(@"OnPopUp")) CancelInvoke(@"OnPopUp");

			if (m_ViewCount == 0 && m_TotalViewCount == 0) Invoke(@"OnPopUp", 0.5f);
			ShowSeriesPopUp();

		}
	}

	private void ShowSeriesPopUp()
	{
		if (m_SeriesPopUpDataList.Count > 0)
		{
			LinkedListNode<ViewData> data = m_SeriesPopUpDataList.Last;

			while (data.Previous != null)
			{
				data = data.Previous;
			}

			if (m_SeriesPopUpDataList.Count > 1)
			{
				// 第二层弹框及其以上
				PauseEvent(0.2f);
			}
		}
	}

	private void PauseEvent(float time)
	{
		if (time > 0 && EventSystem.current != null)
		{
			_current = EventSystem.current;
			_current.enabled = false;
			_pauseTime = time;
		}
	}

	private EventSystem _current;
	private float _pauseTime;
	private void Update()
	{
		if (_pauseTime > 0)
		{
			_pauseTime -= Time.deltaTime;
			if (_pauseTime <= 0 && _current != null)
			{
				_current.enabled = true;
			}
		}
	}

	private ViewData GetData(int objInst)
	{
		foreach (var item in m_SeriesPopUpDataList)
		{
			if (item.InstanceID == objInst)
			{
				return item;
			}
		}
		return null;
	}

	private void OnPopUp()
	{
		if (CanPopUp && m_PopUpDataList.Count > 0)
		{
			PopUpData pPopUpData = m_PopUpDataList[0];

			pPopUpData.Action(pPopUpData.Param);

			m_PopUpDataList.RemoveAt(0);
		}
	}

	private void SetCamera(Canvas pCanvas, Camera pCamera)
	{
		if (pCanvas.worldCamera == null || pCanvas.worldCamera.name != pCamera.name)
		{
			pCanvas.worldCamera = pCamera;
		}
	}
}

public class ViewData
{
	public int InstanceID
	{
		get; set;
	}

	public Canvas Canvas
	{
		get; set;
	}

	public CanvasType CanvasType
	{
		get; set;
	}

	public ViewData(GameObject pGameObject, Canvas pCanvas, CanvasType pCanvasType)
	{
		InstanceID = pGameObject.GetInstanceID();
		Canvas = pCanvas;
		CanvasType = pCanvasType;
	}
}

public class PopUpData
{
	public int Priotiry
	{
		get; set;
	}

	public Action<object> Action
	{
		get; set;
	}

	public object Param
	{
		get; set;
	}

	public PopUpData(Action<object> hAction, object pParam = null, int nPriotiry = 100)
	{
		Priotiry = nPriotiry;
		Action = hAction;
		Param = pParam;
	}
}
