using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class UICamera : MonoBehaviour
{
	private static UICamera g_Instance;

	public static bool HasInstance
	{
		get
		{
			return g_Instance != null;
		}
	}

	public static UICamera GetInstance
	{
		get
		{
			return g_Instance;
		}
		set
		{
			g_Instance = value;
		}
	}

	private Camera m_UICamera;

	public new Camera camera
	{
		get
		{
			return m_UICamera;
		}
	}

	public bool IsHitUI
	{
		get
		{
			if (EventSystem.current != null)
			{
				Vector3 pMousePosition = Input.mousePosition;
				if (pMousePosition.x < 0 || pMousePosition.x >= Screen.width || pMousePosition.y < 0 || pMousePosition.y >= Screen.height)
					return false;
				PointerEventData pPointerEventData = new PointerEventData(EventSystem.current);
				pPointerEventData.position = pMousePosition;
				List<RaycastResult> pRaycastResultList = new List<RaycastResult>();
				EventSystem.current.RaycastAll(pPointerEventData, pRaycastResultList);
				for (int i = 0; i < pRaycastResultList.Count; i++)
				{
					RaycastResult pResult = pRaycastResultList[i];

					if (pResult.gameObject.layer == LayerMask.NameToLayer(@"UI") || pResult.gameObject.layer == LayerMask.NameToLayer(@"BlockUI"))
					{
						return true;
					}
				}
			}

			return false;
		}
	}

	public bool IsHitGlobalCanvas
	{
		get
		{
			if (EventSystem.current == null)
			{
				return false;
			}
			Vector3 pMousePosition = Input.mousePosition;
			if (pMousePosition.x < 0 || pMousePosition.x >= Screen.width || pMousePosition.y < 0 || pMousePosition.y >= Screen.height)
				return false;
			PointerEventData pPointerEventData = new PointerEventData(EventSystem.current);
			pPointerEventData.position = pMousePosition;
			List<RaycastResult> pRaycastResultList = new List<RaycastResult>();
			EventSystem.current.RaycastAll(pPointerEventData, pRaycastResultList);
			for (int i = 0; i < pRaycastResultList.Count; i++)
			{
				RaycastResult pResult = pRaycastResultList[i];

				if (pResult.gameObject.layer == LayerMask.NameToLayer(@"GlobalCanvas") || pResult.gameObject.layer == LayerMask.NameToLayer(@"BlockUI"))
				{
					return true;
				}
			}
			return false;
		}
	}

	public bool IsHitUIOrGlobalCanvas
	{
		get
		{
			if (EventSystem.current != null)
			{
				Vector3 pMousePosition = Input.mousePosition;
				if (pMousePosition.x < 0 || pMousePosition.x >= Screen.width || pMousePosition.y < 0 || pMousePosition.y >= Screen.height)
					return false;

				PointerEventData pPointerEventData = new PointerEventData(EventSystem.current);
				pPointerEventData.position = pMousePosition;
				List<RaycastResult> pRaycastResultList = new List<RaycastResult>();
				EventSystem.current.RaycastAll(pPointerEventData, pRaycastResultList);
				for (int i = 0; i < pRaycastResultList.Count; i++)
				{
					RaycastResult pResult = pRaycastResultList[i];

					if (pResult.gameObject.layer == LayerMask.NameToLayer(@"UI") || pResult.gameObject.layer == LayerMask.NameToLayer(@"GlobalCanvas") || pResult.gameObject.layer == LayerMask.NameToLayer(@"BlockUI"))
					{
						//Debug.Log("pResult name===>" + pResult.gameObject.name);
						return true;
					}
				}
			}

			return false;
		}
	}

	private void Awake()
	{
		if (!HasInstance)
		{
			g_Instance = this;

			m_UICamera = GetComponent<Camera>();

			if (Application.isPlaying)
			{
				//m_UICamera.pixelRect = Screen.safeArea;
				m_UICamera.pixelRect = new Rect(Screen.safeArea.x, 0, Screen.safeArea.width, Screen.height);
				//m_UICamera.pixelRect = new Rect(132.0f,63.0f,2172.0f,1062.0f);// Test for iPhone X
			}
		}
		else
		{
#if UNITY_EDITOR
			if (!UnityEditor.EditorApplication.isPlaying)
			{
				DestroyImmediate(gameObject);
			}
			else
			{
				Destroy(gameObject);
			}
#else
			Destroy(gameObject);
#endif
		}
	}

	private void OnDestroy()
	{
		if (HasInstance && g_Instance.GetInstanceID() == GetInstanceID())
		{
			g_Instance = null;
		}
	}

#if UNITY_EDITOR
	private static Vector3[] g_FourCornerArrary = new Vector3[4];

	private Color m_GizmosColor = Color.cyan;

	private void OnDrawGizmos()
	{
		foreach (MaskableGraphic pMaskableGraphic in FindObjectsOfType<MaskableGraphic>())
		{
			if (pMaskableGraphic.raycastTarget)
			{
				RectTransform pRectTransform = pMaskableGraphic.transform as RectTransform;

				pRectTransform.GetWorldCorners(g_FourCornerArrary);

				Gizmos.color = m_GizmosColor;

				for (int i = 0; i < 4; i++)
				{
					Gizmos.DrawLine(g_FourCornerArrary[i], g_FourCornerArrary[(i + 1) % 4]);
				}
			}
		}
	}

	private void Update()
	{
		if (!UnityEditor.EditorApplication.isPlaying)
		{
			BaseCanvas[] pBaseCanvaes = FindObjectsOfType<BaseCanvas>();

			for (int i = 0; i < pBaseCanvaes.Length; i++)
			{
				Canvas pCanvas = pBaseCanvaes[i].GetComponent<Canvas>();

				if (pCanvas.worldCamera == null) pCanvas.worldCamera = GetComponent<Camera>();

				CanvasScaler pCanvasScaler = pBaseCanvaes[i].GetComponent<CanvasScaler>();

				if (pCanvasScaler != null)
				{
					pCanvasScaler.referenceResolution = GameConfig.STANDARD_RESOLUTION;
					pCanvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
				}
			}

			Selectable[] pSelectables = FindObjectsOfType<Selectable>();

			for (int i = 0; i < pSelectables.Length; i++)
			{
				pSelectables[i].navigation = new Navigation() { mode = Navigation.Mode.None };
			}
		}
	}
#endif
}
