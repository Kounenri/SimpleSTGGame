using System;
using UnityEngine;

public class DelayCall : MonoBehaviour
{
	private float m_StartTime;
	private float m_Delay;
	private Action<object> m_Callback;
	private object m_Param = null;

	public static void Call(Action<object> hCallback, float fDelay, object pParam = null)
	{
		if (fDelay <= 0f)
		{
			hCallback(pParam);
		}
		else
		{
			GameObject pGameObject = new GameObject("DelayCall");
			DelayCall pDelayCall = pGameObject.AddComponent<DelayCall>();
			pDelayCall.AddCall(hCallback, fDelay, pParam);
		}
	}

	public void AddCall(Action<object> hCallback, float fDelay, object pParam = null)
	{
		m_StartTime = Time.time;
		m_Delay = fDelay;
		m_Callback = hCallback;
		m_Param = pParam;
	}

	private void OnComplete()
	{
		Destroy(gameObject);

		if (m_Callback != null)
		{
			m_Callback.Invoke(m_Param);
			m_Callback = null;
			m_Param = null;
		}
	}

	private void Update()
	{
		if (Time.time - m_StartTime >= m_Delay)
		{
			m_Delay = float.MaxValue;

			OnComplete();
		}
	}
}

