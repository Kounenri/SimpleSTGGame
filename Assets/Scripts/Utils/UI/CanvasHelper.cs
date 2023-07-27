using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas))]
public class CanvasHelper : MonoBehaviour
{
	public static UnityEvent OnOrientationChange = new UnityEvent();
	public static UnityEvent OnResolutionChange = new UnityEvent();
	public static bool IsLandscape { get; private set; }

	private static List<CanvasHelper> m_Helpers = new List<CanvasHelper>();

	private static bool m_ScreenChangeVarsInitialized = false;
#pragma warning disable CS0618 // Type or member is obsolete
	private static ScreenOrientation m_LastOrientation = ScreenOrientation.Unknown;
#pragma warning restore CS0618 // Type or member is obsolete
	private static Vector2 m_LastResolution = Vector2.zero;
	private static Vector2 m_LastSafeArea = Vector2.zero;

	private static Vector2 m_WantedReferenceResolution = new Vector2(1280f, 720f);
	private static Camera m_WantedCanvasCamera;

	private Canvas m_Canvas;
	private CanvasScaler m_Scaler;
	private RectTransform m_RectTransform;

	[SerializeField]
	private RectTransform m_SafeAreaTransform;

	private void Awake()
	{
		if (m_SafeAreaTransform == null)
		{
			enabled = false;

			return;
		}

		if (!m_Helpers.Contains(this)) m_Helpers.Add(this);

		m_Canvas = GetComponent<Canvas>();
		m_Scaler = GetComponent<CanvasScaler>();
		m_RectTransform = GetComponent<RectTransform>();

		UpdateReferenceResolution();
		UpdateCanvasCamera();

		if (!m_ScreenChangeVarsInitialized)
		{
			m_LastOrientation = Screen.orientation;
			m_LastResolution.x = Screen.width;
			m_LastResolution.y = Screen.height;
			m_LastSafeArea = Screen.safeArea.size;

			m_ScreenChangeVarsInitialized = true;
		}
	}

	private void Start()
	{
		ApplySafeArea();
	}

	private void Update()
	{
		if (m_Helpers[0] != this) return;

		if (Application.isMobilePlatform)
		{
			if (Screen.orientation != m_LastOrientation) OrientationChanged();

			if (Screen.safeArea.size != m_LastSafeArea) SafeAreaChanged();
		}
		else
		{
			//resolution of mobile devices should stay the same always, right?
			// so this check should only happen everywhere else
			if (Screen.width != m_LastResolution.x || Screen.height != m_LastResolution.y)
			{
				ResolutionChanged();
			}
		}
	}

	private void ApplySafeArea()
	{
		if (m_SafeAreaTransform == null) return;

		var safeArea = Screen.safeArea;

		var anchorMin = safeArea.position;
		var anchorMax = safeArea.position + safeArea.size;
		anchorMin.x /= m_Canvas.pixelRect.width;
		anchorMin.y /= m_Canvas.pixelRect.height;
		anchorMax.x /= m_Canvas.pixelRect.width;
		anchorMax.y /= m_Canvas.pixelRect.height;

		m_SafeAreaTransform.anchorMin = anchorMin;
		m_SafeAreaTransform.anchorMax = anchorMax;

		//Debug.Log(
		//    "ApplySafeArea:" +
		//    "\n Screen.orientation: " + Screen.orientation +
		//    #if UNITY_IOS
		//    "\n Device.generation: " + UnityEngine.iOS.Device.generation.ToString() +
		//    #endif
		//    "\n Screen.safeArea.position: " + Screen.safeArea.position.ToString() +
		//    "\n Screen.safeArea.size: " + Screen.safeArea.size.ToString() +
		//    "\n Screen.width / height: (" + Screen.width.ToString() + ", " + Screen.height.ToString() + ")" +
		//    "\n canvas.pixelRect.size: " + canvas.pixelRect.size.ToString() +
		//    "\n anchorMin: " + anchorMin.ToString() +
		//    "\n anchorMax: " + anchorMax.ToString());
	}

	private void UpdateCanvasCamera()
	{
		if (m_Canvas.worldCamera == null && m_WantedCanvasCamera != null)
		{
			m_Canvas.worldCamera = m_WantedCanvasCamera;
		}
	}

	private void UpdateReferenceResolution()
	{
		if (m_Scaler.referenceResolution != m_WantedReferenceResolution)
		{
			m_Scaler.referenceResolution = m_WantedReferenceResolution;
		}
	}

	private void OnDestroy()
	{
		if (m_Helpers != null && m_Helpers.Contains(this))
		{
			m_Helpers.Remove(this);
		}
	}

	private static void OrientationChanged()
	{
		//Debug.Log("Orientation changed from " + lastOrientation + " to " + Screen.orientation + " at " + Time.time);

		m_LastOrientation = Screen.orientation;
		m_LastResolution.x = Screen.width;
		m_LastResolution.y = Screen.height;

		IsLandscape = m_LastOrientation == ScreenOrientation.LandscapeLeft || m_LastOrientation == ScreenOrientation.LandscapeRight || m_LastOrientation == ScreenOrientation.LandscapeLeft;
		OnOrientationChange.Invoke();
	}

	private static void ResolutionChanged()
	{
		if (m_LastResolution.x == Screen.width && m_LastResolution.y == Screen.height) return;

		//Debug.Log("Resolution changed from " + lastResolution + " to (" + Screen.width + ", " + Screen.height + ") at " + Time.time);

		m_LastResolution.x = Screen.width;
		m_LastResolution.y = Screen.height;

		IsLandscape = Screen.width > Screen.height;
		OnResolutionChange.Invoke();
	}

	private static void SafeAreaChanged()
	{
		if (m_LastSafeArea == Screen.safeArea.size) return;

		//Debug.Log("Safe Area changed from " + lastSafeArea + " to " + Screen.safeArea.size + " at " + Time.time);

		m_LastSafeArea = Screen.safeArea.size;

		for (int i = 0; i < m_Helpers.Count; i++)
		{
			m_Helpers[i].ApplySafeArea();
		}
	}

	public static void SetAllCanvasCamera(Camera cam)
	{
		if (m_WantedCanvasCamera == cam) return;

		m_WantedCanvasCamera = cam;

		for (int i = 0; i < m_Helpers.Count; i++)
		{
			m_Helpers[i].UpdateCanvasCamera();
		}
	}

	public static void SetAllReferenceResolutions(Vector2 newReferenceResolution)
	{
		if (m_WantedReferenceResolution == newReferenceResolution) return;

		//Debug.Log("Reference resolution changed from " + wantedReferenceResolution + " to " + newReferenceResolution + " at " + Time.time);

		m_WantedReferenceResolution = newReferenceResolution;

		for (int i = 0; i < m_Helpers.Count; i++)
		{
			m_Helpers[i].UpdateReferenceResolution();
		}
	}

	public static Vector2 CanvasSize()
	{
		return m_Helpers[0].m_RectTransform.sizeDelta;
	}

	public static Vector2 SafeAreaSize()
	{
		for (int i = 0; i < m_Helpers.Count; i++)
		{
			if (m_Helpers[i].m_SafeAreaTransform != null)
			{
				return m_Helpers[i].m_SafeAreaTransform.sizeDelta;
			}
		}

		return CanvasSize();
	}

	public static Vector2 GetReferenceResolution()
	{
		return m_WantedReferenceResolution;
	}
}