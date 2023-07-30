using UnityEngine;

public class DisableRender : MonoBehaviour
{
	private float m_Duration = 2f;
	private float m_StartTime = 0;

	public float Duration
	{
		get
		{
			return m_Duration;
		}
		set
		{
			m_Duration = value;
			m_StartTime = Time.time;
		}
	}

	private void Update()
	{
		if (Time.time - m_StartTime >= m_Duration)
		{
			OnEnd();
		}
	}

	public void OnEnd()
	{
		Destroy(gameObject);
	}
}
