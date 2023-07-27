using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;

[RequireComponent(typeof(Canvas))]
[RequireComponent(typeof(GraphicRaycaster))]
[RequireComponent(typeof(CanvasScaler))]
[RequireComponent(typeof(CanvasGroup))]
public class BaseCanvas : MonoBehaviour
{
	public const float SHOW_VIEW_DELAY = /*0.085f*/0;
	public const float REFRESH_VIEW_DELAY = 0.15f;

	public const float EFFECT_SHOW_TIME = /*0.6f*/0;
	public const float EFFECT_HIDE_TIME = /*0.6f*/0;

	[SerializeField]
	private CanvasType m_CanvasType = CanvasType.UI;
	[SerializeField]
	private bool m_EnableOpenSound = true;
	[SerializeField]
	private bool m_EnableCloseSound = true;
	[SerializeField]
	private AudioClip m_OpenSoundClip = null;
	[SerializeField]
	private AudioClip m_CloseSoundClip = null;
	[SerializeField]
	private bool m_DelayShow = true;
	[SerializeField]
	private bool m_DestroyWhenBackKey = true;
	protected CanvasGroup m_CanvasGroup;
	protected KeyManager m_KeyManager;
	protected GameObject m_PreView;
	protected bool m_IsBack = false;
	protected bool m_IsShow = false;
	protected bool m_IsDestroying = false;

	protected Sequence m_childAniQuence;


	public CanvasType CanvasType
	{
		get
		{
			return m_CanvasType;
		}
	}
	public bool delayShow
	{
		get
		{
			return /*m_DelayShow*/false;
		}
		set
		{
			m_DelayShow = value;
		}
	}

	public bool destroyWhenBackKey
	{
		get
		{
			return m_DestroyWhenBackKey;
		}
		set
		{
			m_DestroyWhenBackKey = value;
			if (m_KeyManager != null) m_KeyManager.RemoveView(gameObject);
		}
	}

	public GameObject preView
	{
		get
		{
			return m_PreView;
		}
	}

	public bool isBack
	{
		get
		{
			return m_IsBack;
		}
		set
		{
			m_IsBack = value;
		}
	}

	public bool isShow
	{
		get
		{
			return m_IsShow;
		}
		set
		{
			m_IsShow = value;
		}
	}

	public CanvasGroup canvasGroup
	{
		get
		{
			return m_CanvasGroup;
		}
	}

	protected virtual void Awake()
	{
		GameObject pGameObject = GameObject.FindGameObjectWithTag(@"KeyManager");

		if (pGameObject == null)
		{
			Debug.LogWarning(@"Warning - You should add a KeyManager GameObject in the " + LevelNameEnum.ActiveSceneName);
		}
		else
		{
			m_KeyManager = pGameObject.GetComponent<KeyManager>();

			if (m_DestroyWhenBackKey) m_KeyManager.AddView(gameObject);
		}

		m_CanvasGroup = GetComponent<CanvasGroup>();

		GraphicRaycaster pGraphicRaycaster = GetComponent<GraphicRaycaster>();

		if (pGraphicRaycaster != null) pGraphicRaycaster.blockingObjects = GraphicRaycaster.BlockingObjects.TwoD;
	}

	protected virtual void OnEnable()
	{
	}

	protected virtual void Start()
	{
		if (ViewManager.HasInstance) ViewManager.GetInstance.ShowBaseCanvas(gameObject, GetComponent<Canvas>(), m_CanvasType);

		if (m_DelayShow)
		{
			// m_CanvasGroup.SetActive(false); // 快速连点,这里会导致重复打开UI的问题!故注释掉.

			if (BlockCanvas.HasInstance) BlockCanvas.GetInstance.Show(EFFECT_SHOW_TIME * 1.1f);
		}
		else
		{
			if (BlockCanvas.HasInstance) BlockCanvas.GetInstance.Show(SHOW_VIEW_DELAY * 1.1f);
		}

		ShowCanvas();

		if (m_EnableOpenSound && m_DestroyWhenBackKey && SoundManager.HasInstance)
		{
			if (m_OpenSoundClip != null)
			{
				SoundManager.PlaySound(m_OpenSoundClip);
			}
			else
			{
				if (m_CanvasType == CanvasType.Popup)
				{
					SoundManager.PlaySound(SoundManager.GetInstance.m_UiOpenPopupSound);
				}
				else
				{

					SoundManager.PlaySound(SoundManager.GetInstance.m_UiOpenSound);
				}
			}
		}

		if (m_PreView != null) Invoke(@"HidePreView", SHOW_VIEW_DELAY);
	}

	protected virtual void OnDisable()
	{
	}

	protected virtual void OnDestroy()
	{
		if (m_KeyManager != null) m_KeyManager.RemoveView(gameObject);

		if (ViewManager.HasInstance) ViewManager.GetInstance.DestroyBaseCanvas(gameObject);

		if (m_EnableCloseSound && m_DestroyWhenBackKey && SoundManager.HasInstance)
		{
			if (m_CloseSoundClip != null)
			{
				SoundManager.PlaySound(m_CloseSoundClip);
			}
			else
			{
				if (m_CanvasType == CanvasType.Popup)
				{
					SoundManager.PlaySound(SoundManager.GetInstance.m_ClosePopupBtnSound);
				}
				else
				{
					SoundManager.PlaySound(SoundManager.GetInstance.m_CloseBtnSound);
				}
			}
		}

		if (m_PreView != null)
		{
			if (IsInvoking(nameof(HidePreView))) CancelInvoke(nameof(HidePreView));
			ShowPreView();
		}

		m_PreView = null;
	}

	public BaseCanvas SetPreView(GameObject pPreView, bool bIsBack = true)
	{
		m_PreView = pPreView;
		m_IsBack = bIsBack;

		return this;
	}

	public void DestroyCanvas()
	{
		if (!m_IsDestroying)
		{
			m_IsDestroying = true;

			Destroy(gameObject, EFFECT_HIDE_TIME);

			if (BlockCanvas.HasInstance) BlockCanvas.GetInstance.Show(EFFECT_HIDE_TIME * 1.1f);

			BroadcastMessage(@"OnHide", SendMessageOptions.DontRequireReceiver);
		}
	}

	public void Destroy()
	{
		Destroy(gameObject);
	}

	protected virtual void ShowCanvas()
	{
		m_IsShow = true;

		if (m_DelayShow)
		{
			m_CanvasGroup.SetTweenActive(true);
		}

		BroadcastMessage(@"OnShow", SendMessageOptions.DontRequireReceiver);
	}

	protected virtual void HidePreView()
	{
		Debug.Log("PreView===>" + m_PreView);
		if (m_PreView != null)
		{
			m_PreView.BroadcastMessage(@"OnHidePreView", SendMessageOptions.DontRequireReceiver);

			CanvasGroup pCanvasGroup = m_PreView.GetComponent<CanvasGroup>();

			if (m_PreView.name == "DrawCardView(Clone)" || m_PreView.name == "HeroDetailAlert(Clone)")
			{
				transform.GetComponent<Canvas>().sortingOrder = 20;
			}

			if (pCanvasGroup != null)
			{
				pCanvasGroup.SetTweenActive(false, true, EFFECT_HIDE_TIME / 2f);
			}
			else
			{
				m_PreView.SetActive(false);
			}
		}
	}

	protected virtual void ShowPreView()
	{
		if (m_IsBack)
		{
			m_PreView.BroadcastMessage(@"OnShowPreView", SendMessageOptions.DontRequireReceiver);

			CanvasGroup pCanvasGroup = m_PreView.GetComponent<CanvasGroup>();

			if (pCanvasGroup != null)
			{
				pCanvasGroup.SetActive(true);
			}
			else
			{
				m_PreView.SetActive(true);
			}
		}
		else
		{
			Destroy(m_PreView);
		}
	}
}

public enum CanvasType
{
	Static = -1,
	Ignore = 1,
	UI = 3,
	Popup = 4
}
