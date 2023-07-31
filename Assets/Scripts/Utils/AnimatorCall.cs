using System;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Animator))]
public class AnimatorCall : MonoBehaviour
{
	[SerializeField]
	private bool m_DisableOnStart = false;
	[SerializeField]
	private AnimatorEvent m_Trigger = new();
	[Serializable]
	public class AnimatorEvent : UnityEvent { }

	private Animator m_Animator;

	public AnimatorEvent Trigger
	{
		get
		{
			return m_Trigger;
		}
		set
		{
			m_Trigger = value;
		}
	}

	private void Awake()
	{
		m_Animator = GetComponent<Animator>();
	}

	private void Start()
	{
		if (m_DisableOnStart)
		{
			m_Animator.enabled = false;
		}
	}

	public void AniPlay()
	{
		if (m_Animator != null)
		{
			m_Animator.enabled = true;
			m_Animator.Play(0);
			m_Animator.speed = 1f;
		}
	}

	public void AniReset()
	{
		if (m_Animator != null)
		{
			m_Animator.Play(0);
			m_Animator.speed = 0f;
		}
	}

	public void AniKill()
	{
		Destroy(gameObject);
	}

	public void AniKillRoot()
	{
		Destroy(transform.root.gameObject);
	}

	public void AniKillParent()
	{
		Destroy(transform.parent.gameObject);
	}

	public void AniDisable()
	{
		gameObject.SetActive(false);
	}

	public void AniTrigger()
	{
		m_Trigger.Invoke();
	}
}
