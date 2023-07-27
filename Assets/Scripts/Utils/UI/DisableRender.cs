using UnityEngine;

public class DisableRender : MonoBehaviour
{
	private float m_fDuration = 2f;
	private float m_fStartTime = 0;

	public float Duration
	{
		get
		{
			return m_fDuration;
		}
		set
		{
			m_fDuration = value;
			m_fStartTime = Time.time;
		}
	}

	void Update()
	{
		if(Time.time - m_fStartTime >= m_fDuration)
		{
			OnEnd();
		}
	}

	public void OnEnd()
	{
		Destroy(gameObject);
	}
}
