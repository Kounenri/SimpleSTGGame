using UnityEngine;

public class GlobalCanvasDelegate : MonoBehaviour
{
	[SerializeField]
	private bool m_Moveable = true;

	private Transform m_ViewTransform;
	private GameObject m_View;
	private bool m_Invalidate = false;

	public Transform ViewTransform
	{
		get
		{
			if (m_ViewTransform == null)
			{
				m_ViewTransform = transform.Find("Panel");
			}
			return m_ViewTransform;
		}
	}

	public GameObject View
	{
		get
		{
			if (m_View == null)
			{
				m_View = ViewTransform.gameObject;
			}
			return m_View;
		}
	}

	private void Awake()
	{
		m_ViewTransform = transform.Find("Panel");
		m_View = m_ViewTransform.gameObject;
	}

	private void Start()
	{
		if (GlobalCanvas.HasInstance)
		{
			View.GetComponent<RectTransform>().SetParent(GlobalCanvas.GetInstance.transform, true);
			m_Invalidate = true;
		}
	}

	private void Update()
	{
		if (m_Moveable || m_Invalidate)
		{
			if (m_View != null)
			{
				m_View.transform.position = transform.position;
			}
			m_Invalidate = false;
		}
	}

	private void OnDisable()
	{
		if (m_View != null)
		{
			m_View.SetActive(false);
		}
	}

	private void OnEnable()
	{
		if (m_View != null)
		{
			m_Invalidate = true;
			m_View.SetActive(true);

			m_View.transform.position = transform.position;
		}
	}

	private void OnDestroy()
	{
		if (m_View != null) Destroy(m_View);

		m_View = null;
	}
}
