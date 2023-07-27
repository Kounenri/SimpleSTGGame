using UnityEngine;

public class TAudioSource : MonoBehaviour
{
	[SerializeField]
	private AudioClip m_AudioClip;

	[SerializeField]
	private bool m_PlayOnStart = false;
	[SerializeField]
	private bool m_PlayOnEnable = false;
	[SerializeField]
	private bool m_PlayOnDestroy = false;

	[SerializeField]
	public float m_Delay = 0;

	public AudioClip AudioClip
	{
		get
		{
			return m_AudioClip;
		}
		set
		{
			m_AudioClip = value;
		}
	}

	public bool PlayOnStart
	{
		get
		{
			return m_PlayOnStart;
		}
		set
		{
			m_PlayOnStart = value;
		}
	}

	public bool PlayOnEnable
	{
		get
		{
			return m_PlayOnEnable;
		}
		set
		{
			m_PlayOnEnable = value;
		}
	}

	public bool PlayOnDestroy
	{
		get
		{
			return m_PlayOnDestroy;
		}
		set
		{
			m_PlayOnDestroy = value;
		}
	}

	void Start()
	{
		if (m_Delay < 0)
		{
			m_Delay = 0;
		}

		if (m_PlayOnStart)
		{
			Play();
		}
	}

	void OnEnable()
	{
		if (m_Delay < 0)
		{
			m_Delay = 0;
		}

		if (m_PlayOnEnable)
		{
			Play();
		}
	}

	void OnDestroy()
	{
		if (m_PlayOnDestroy)
		{
			Play();
		}
	}

	public void Play()
	{
		if (m_AudioClip != null)
		{
			SoundManager.PlaySound(m_AudioClip, 0);
		}
	}
}
