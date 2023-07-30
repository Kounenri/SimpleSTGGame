using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundManager : TMonoInstanceLite<SoundManager>
{
	private const string m_GameMusic = @"GameMusic";

	public const float PITCH = 0.15f;

	#region SoundDefines
	[SerializeField]
	private AudioClip m_MainGame;
	[SerializeField]
	public AudioClip m_CloseBtnSound;
	[SerializeField]
	public AudioClip m_UiOpenSound;
	[SerializeField]
	public AudioClip m_UiOpenPopupSound;
	[SerializeField]
	public AudioClip m_ClosePopupBtnSound;
	[SerializeField]
	public AudioClip m_BeAttack_Voice;
	#endregion

	[SerializeField]
	private AudioSource m_AudioSource;
	private AudioClip m_AudioClip;
	private List<AudioSource> m_PlayingSoundList = new List<AudioSource>();
	private List<AudioSource> m_AudioSourceList = new List<AudioSource>();
	private float m_MusicVolumeScale = 1f;
	private float m_SoundVolumeScale = 1f;
	private Coroutine m_Coroutine;

	public int PlayingSoundCount
	{
		get
		{
			return m_PlayingSoundList.Count;
		}
	}

	public bool IsPlayingSound
	{
		get
		{
			return m_PlayingSoundList.Count > 0;
		}
	}

	public float MusicVolumeScale
	{
		get
		{
			return m_MusicVolumeScale;
		}
		set
		{
			m_AudioSource.volume = m_MusicVolumeScale = value;

			if (m_MusicVolumeScale == 0)
			{
				m_AudioSource.Pause();
			}
			else
			{
				if (m_AudioSource.isPlaying == false)
				{
					m_AudioSource.Play();
				}
			}
		}
	}

	public float SoundVolumeScale
	{
		set
		{
			m_SoundVolumeScale = value;
			PlayerPrefs.SetFloat(PlayerPrefNames.SOUND_VOL, value);
		}
		get
		{
			return m_SoundVolumeScale;
		}
	}

	protected override void Awake()
	{
		if (HasInstance)
		{
			Destroy(gameObject);
		}
		else
		{
			g_Instance = this;

			if (PlayerPrefs.HasKey(PlayerPrefNames.MUSIC_VOL))
			{
				m_MusicVolumeScale = PlayerPrefs.GetFloat(PlayerPrefNames.MUSIC_VOL);
			}

			if (PlayerPrefs.HasKey(PlayerPrefNames.SOUND_VOL))
			{
				m_SoundVolumeScale = PlayerPrefs.GetFloat(PlayerPrefNames.SOUND_VOL);
			}

			DontDestroyOnLoad(gameObject);

			m_AudioSource = GetComponent<AudioSource>();

			SceneManager.sceneLoaded += OnLevelLoaded;
		}
	}

	public static void PlayMusic(string strClipName, float startTime = 0f)
	{
		if (HasInstance)
		{
			GetInstance.PlayTheMusic(strClipName, startTime);
		}
	}

	protected void PlayTheMusic(string strClipName, float startTime = 0f)
	{
		if (GameConfig.ENABLE_MUSIC == false) return;

		if (strClipName == m_GameMusic)
		{
			m_AudioClip = m_MainGame;
		}

		if (m_Coroutine != null) StopCoroutine(m_Coroutine);

		m_Coroutine = StartCoroutine(CrossFadeMusic(startTime));
	}

	private IEnumerator CrossFadeMusic(float startTime)
	{
		int nFadeFrame = 20;

		if (m_AudioSource.clip != null)
		{
			int nStep = 0;
			float fVolume = m_AudioSource.volume;

			while (nStep <= nFadeFrame)
			{
				m_AudioSource.volume = Mathf.Lerp(fVolume, 0, nStep * 1f / nFadeFrame);
				yield return new WaitForSeconds(0.03f);
				nStep++;
			}

			m_AudioSource.clip = null;
		}

		if (m_AudioClip != null)
		{
			m_AudioSource.clip = m_AudioClip;
			m_AudioSource.loop = true;
			m_AudioSource.Pause();
			int nStep = 0;

			while (nStep <= nFadeFrame)
			{
				m_AudioSource.volume = Mathf.Lerp(0, MusicVolumeScale, nStep * 1f / nFadeFrame);
				yield return new WaitForSeconds(0.03f);

				nStep++;
			}

			if (MusicVolumeScale > 0f)
			{

				m_AudioSource.time = startTime;
				m_AudioSource.Play(0);
			}
		}
	}

	public static void PlaySound(AudioClip pAudioClip, float fDelay = 0.0f, float pitch = 1f)
	{
#if UNITY_EDITOR
		if (pAudioClip == null)
		{
			Debug.Log("pAudioClip == null");
		}
		else
		{
			//Debug.Log("SSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSS - : " + pAudioClip.name);
		}
#endif

		if (HasInstance && pAudioClip != null)
		{
			GetInstance.PlayTheSound(pAudioClip, fDelay, pitch);
		}
	}

	protected void PlayTheSound(AudioClip pAudioClip, float fDelay = 0.0f, float pitch = 1f)
	{
		if (pAudioClip != null)
		{
			if (SoundVolumeScale <= 0)
				return;
			if (pAudioClip == null)
				return;
			if (m_PlayingSoundList.Count > 20)
				return;

			if (pAudioClip == m_CloseBtnSound && m_PlayingSoundList.Find((p) =>
			{
				return p.clip == m_UiOpenSound || p.clip == m_CloseBtnSound;
			}) != null)
			{
				return;
			}

			StartCoroutine(playOneShotSound(pAudioClip, fDelay, pitch));
		}
	}

	private IEnumerator playOneShotSound(AudioClip pAudioClip, float fDelay = 0, float pitch = 1f)
	{
		AudioSource pAudioSource = FetAudioSource();

		if (pAudioSource == null)
			yield break;

		pAudioSource.clip = pAudioClip;
		pAudioSource.volume = SoundVolumeScale;
		pAudioSource.loop = false;
		pAudioSource.pitch = pitch;
		pAudioSource.PlayDelayed(fDelay);
		m_PlayingSoundList.Add(pAudioSource);

		yield return null;

		yield return new WaitForSeconds(pAudioClip.length + fDelay);

		RecycleAudioSource(pAudioSource);
	}

	private void RecycleAudioSource(AudioSource pAudioSource)
	{
		pAudioSource.enabled = false;
		pAudioSource.clip = null;

		if (m_PlayingSoundList.Contains(pAudioSource))
			m_PlayingSoundList.Remove(pAudioSource);

		if (m_AudioSourceList.Contains(pAudioSource) == false)
		{
			m_AudioSourceList.Add(pAudioSource);
		}
	}

	private AudioSource FetAudioSource()
	{
		if (m_AudioSourceList.Count == 0)
		{
			m_AudioSourceList.Add(gameObject.AddComponent<AudioSource>());
		}

		int nCount = m_AudioSourceList.Count;
		AudioSource pAudioSource = m_AudioSourceList[nCount - 1];
		m_AudioSourceList.RemoveAt(nCount - 1);
		pAudioSource.enabled = true;
		return pAudioSource;
	}

	private void OnLevelLoaded(Scene arg0, LoadSceneMode arg1)
	{
		if (arg0.name == LevelNameEnum.GameScene)
		{
			if (m_AudioSource.clip == null || m_AudioSource.clip.name != m_GameMusic)
				PlayMusic(m_GameMusic);
		}
	}

	void OnApplicationPause(bool bPaused)
	{
		if (bPaused)
		{
			if (m_Coroutine != null)
			{
				StopCoroutine(m_Coroutine);
			}

			if (m_AudioSource != null)
			{
				m_AudioSource.clip = null;
			}
		}
		else
		{
			m_Coroutine = StartCoroutine(CrossFadeMusic(0));
		}
	}

	public void PlayBeAttackVoice()
	{
		PlaySound(GetInstance.m_BeAttack_Voice);
	}
}
