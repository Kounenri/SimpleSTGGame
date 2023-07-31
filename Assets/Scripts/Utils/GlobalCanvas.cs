using UnityEngine;

public class GlobalCanvas : MonoBehaviour
{
	private static GlobalCanvas g_Instance;

	public static GlobalCanvas GetInstance
	{
		get
		{
			return g_Instance;
		}
	}

	public static bool HasInstance
	{
		get
		{
			return g_Instance != null;
		}
	}

	[SerializeField]
	private Camera m_TargetCamera = null;
	private Camera m_Camera;
	private Transform m_CameraTransform;

	public Camera Camera
	{
		get
		{
			return m_Camera;
		}
	}

	public Transform CameraTransform
	{
		get
		{
			return m_CameraTransform;
		}
	}

	void Awake()
	{
		g_Instance = this;

		m_Camera = GetComponentInChildren<Camera>();
		m_CameraTransform = m_Camera.transform;

		if (m_TargetCamera != null && m_TargetCamera.orthographic)
		{
			m_Camera.orthographic = true;
			m_Camera.farClipPlane = 10000f;
		}
	}

	void OnDestroy()
	{
		g_Instance = null;
	}

	void Update()
	{
		if (m_TargetCamera != null)
		{
			m_CameraTransform.position = m_TargetCamera.transform.position;
			m_CameraTransform.rotation = m_TargetCamera.transform.rotation;
			if (m_Camera.orthographic)
			{
				GetComponent<Camera>().orthographicSize = m_TargetCamera.orthographicSize;
			}
			else
			{
				GetComponent<Camera>().fieldOfView = m_TargetCamera.fieldOfView;
			}
		}
	}
}
