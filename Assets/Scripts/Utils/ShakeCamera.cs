using UnityEngine;

public class ShakeCamera : MonoBehaviour
{
	// 震动幅度
	[SerializeField]
	public float m_ShakeLevel = 30f;
	// 震动时间
	[SerializeField]
	public float m_SetShakeTime = 1f;
	// 震动的FPS
	[SerializeField]
	public float m_ShakeFps = 45f;

	// 震动标志位
	private bool m_IsShakeCamera = false;
	private float m_Fps;
	private float m_ShakeTime = 0.0f;
	private float m_FrameTime = 0.0f;
	private float m_ShakeDelta = 0.005f;
	private Camera m_SelfCamera;

	private Rect changeRect;

	void Awake()
	{
		m_SelfCamera = GetComponent<Camera>();
		changeRect = new Rect(0.0f, 0.0f, 1.0f, 1.0f);
	}

	// Use this for initialization
	void Start()
	{
		m_ShakeTime = m_SetShakeTime;
		m_Fps = m_ShakeFps;
		m_FrameTime = 0.03f;
		m_ShakeDelta = 0.005f;
	}

	// Update is called once per frame
	void Update()
	{
		if (m_IsShakeCamera && Screen.safeArea.size == new Vector2(Screen.width, Screen.height))
		{
			if (m_ShakeTime > 0)
			{
				m_ShakeTime -= Time.deltaTime;
				if (m_ShakeTime <= 0)
				{
					changeRect.xMin = 0.0f;
					changeRect.yMin = 0.0f;
					m_SelfCamera.rect = changeRect;
					m_IsShakeCamera = false;
					m_ShakeTime = m_SetShakeTime;
					m_Fps = m_ShakeFps;
					m_FrameTime = 0.03f;
					m_ShakeDelta = 0.005f;
				}
				else
				{
					m_FrameTime += Time.deltaTime;

					if (m_FrameTime > 1.0 / m_Fps)
					{
						m_FrameTime = 0;
						changeRect.xMin = m_ShakeDelta * (-1.0f + m_ShakeLevel * Random.value);
						changeRect.yMin = m_ShakeDelta * (-1.0f + m_ShakeLevel * Random.value);
						m_SelfCamera.rect = changeRect;
					}
				}
			}
		}
	}

	public void shake()
	{
		m_IsShakeCamera = true;
	}
}
