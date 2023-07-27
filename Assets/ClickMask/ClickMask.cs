using UnityEngine;

public class ClickMask : MonoBehaviour
{
	private static ClickMask g_Instance;

	public static ClickMask GetInstance
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
			return g_Instance != null ? true : false;
		}
	}

	public Transform targetCamTransfrom;

	[SerializeField]
	private GameObject m_BlockAllUIPrefab = null;

	private Camera m_TargetCamera;
	private Camera m_SelfCamera;
	private Transform m_SelfCameraCamTransfrom;
	private GameObject m_BlockObject;

	private void Awake()
	{
		if (g_Instance != null)
		{
			Destroy(g_Instance);
		}
		else
		{
			g_Instance = this;

			m_SelfCameraCamTransfrom = transform.Find(@"Camera");
			m_SelfCamera = m_SelfCameraCamTransfrom.GetComponent<Camera>();
		}
	}

	public bool IsHitMask
	{
		get
		{
			if (m_SelfCamera != null)
			{
				Vector3 mousePos = Input.mousePosition;
				if (mousePos.x >= 0 && mousePos.x < Screen.width && mousePos.y >= 0 && mousePos.y < Screen.height)
				{
					Ray ray = m_SelfCamera.ScreenPointToRay(mousePos);
					RaycastHit2D[] hits = Physics2D.GetRayIntersectionAll(ray);
					int len = hits.Length;
					for (int i = 0; i < len; i++)
					{
						RaycastHit2D hit = hits[i];
						if (hit.collider != null)
						{
							string layerName = LayerMask.LayerToName(hit.collider.gameObject.layer);
							if (layerName == "ClickMask")
							{
								return true;
							}
						}
					}
				}
			}
			return false;
		}
	}

	private void Update()
	{
		if (m_BlockObject == null)
		{
			m_BlockObject = Instantiate(m_BlockAllUIPrefab) as GameObject;
		}

		targetCamTransfrom = Camera.main.transform;

		if (m_BlockObject != null && m_BlockObject.activeSelf == false)
		{
			m_BlockObject.SetActive(true);
		}

		m_TargetCamera = targetCamTransfrom.GetComponent<Camera>();
		Vector3 pos = targetCamTransfrom.position;
		pos.z = -1;
		m_SelfCameraCamTransfrom.position = pos;
		m_SelfCamera.orthographicSize = m_TargetCamera.orthographicSize;
	}

	void OnDestroy()
	{
		g_Instance = null;

		if (m_BlockObject != null)
		{
			Destroy(m_BlockObject);
		}
	}
}
