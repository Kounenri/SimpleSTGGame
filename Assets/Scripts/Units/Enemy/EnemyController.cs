using DG.Tweening;
using System;
using UnityEngine;

[RequireComponent(typeof(EnemyUnits))]
public class EnemyController : MonoBehaviour
{
	[Range(0.0f, 0.3f)]
	[SerializeField]
	private float m_RotationSmoothTime = 0.12f;
	[SerializeField]
	private float m_SpeedChangeRate = 10.0f;
	[SerializeField]
	private bool m_Grounded = true;
	[SerializeField]
	private float m_GroundedOffset = -0.14f;
	[SerializeField]
	private float m_Gravity = -15.0f;
	[SerializeField]
	private float m_FallTimeout = 0.15f;
	[SerializeField]
	private AudioClip[] m_FootstepAudioClips;
	[Range(0, 1)]
	[SerializeField]
	private float m_FootstepAudioVolume = 0.5f;
	[SerializeField]
	private AudioClip m_AttackAudioClip;
	[SerializeField]
	private AudioClip m_DieAudioClip;

	private EnemyUnits m_EnemyUnits;
	private Animator m_Animator;
	private CharacterController m_Controller;
	private Tweener m_Tweener;

	// player
	private float m_Speed;
	private float m_AnimationBlend;
	private float m_TargetRotation = 0.0f;
	private float m_RotationVelocity;
	private float m_VerticalVelocity;
	private float m_TerminalVelocity = 53.0f;

	// animation IDs
	private int m_AnimIDSpeed;
	private int m_AnimIDWalk;
	private int m_AnimIDRun;

	// timeout deltatime
	private float m_FallTimeoutDelta;

	private bool m_HasAnimator;
	private bool m_WaitForSetPosition = true;

	private void Awake()
	{
		m_EnemyUnits = GetComponent<EnemyUnits>();
		m_Controller = GetComponent<CharacterController>();
	}

	private void Start()
	{
		m_HasAnimator = TryGetComponent(out m_Animator);

		AssignAnimationIDs();
	}

	private void OnEnable()
	{
		m_WaitForSetPosition = true;

		m_Controller.enabled = true;
		m_Controller.detectCollisions = true;

		ParticleSystem pParticleSystem = GetComponentInChildren<ParticleSystem>();

		if (pParticleSystem != null) pParticleSystem.Play(true);
	}

	private void OnDisable()
	{
		if (m_Tweener != null)
		{
			m_Tweener.Kill();
			m_Tweener = null;
		}
	}

	private void AssignAnimationIDs()
	{
		m_AnimIDSpeed = Animator.StringToHash("Speed");
		m_AnimIDWalk = Animator.StringToHash("Walk");
		m_AnimIDRun = Animator.StringToHash("Run");
	}

	private void Update()
	{
		m_HasAnimator = TryGetComponent(out m_Animator);

		if (!m_EnemyUnits.IsDead())
		{
			PlayerUnits pPlayerUnits = LevelController.GetInstance.CurrentPlayer;

			Vector3 pTargetDirection = pPlayerUnits.transform.position - transform.position;

			if (pTargetDirection.magnitude < m_EnemyUnits.AttackRange && !pPlayerUnits.IsDead())
			{
				Attack();
			}
			else if (!m_WaitForSetPosition)
			{
				Gravity();
				GroundedCheck();
				Move(pTargetDirection);
			}

			m_WaitForSetPosition = false;
		}
	}

	private void Gravity()
	{
		if (m_Grounded)
		{
			// reset the fall timeout timer
			m_FallTimeoutDelta = m_FallTimeout;

			// stop our velocity dropping infinitely when grounded
			if (m_VerticalVelocity < 0.0f)
			{
				m_VerticalVelocity = -2f;
			}
		}
		else
		{
			// fall timeout
			if (m_FallTimeoutDelta >= 0.0f)
			{
				m_FallTimeoutDelta -= Time.deltaTime;
			}
		}

		// apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
		if (m_VerticalVelocity < m_TerminalVelocity)
		{
			m_VerticalVelocity += m_Gravity * Time.deltaTime;
		}
	}

	private void GroundedCheck()
	{
		m_Grounded = transform.position.y - m_GroundedOffset <= 0f;
	}

	private void Move(Vector3 pTargetDirection)
	{
		// set target speed based on move speed
		m_Speed = m_EnemyUnits.MoveSpeed;

		if (LevelController.GetInstance.CurrentPlayer.IsDead()) m_Speed = 0.0f;

		m_AnimationBlend = Mathf.Lerp(m_AnimationBlend, m_Speed, Time.deltaTime * m_SpeedChangeRate);

		if (m_AnimationBlend < 0.01f) m_AnimationBlend = 0f;

		// normalise target direction
		pTargetDirection = pTargetDirection.normalized;

		// if there is a move target rotate player when the player is moving
		m_TargetRotation = Mathf.Atan2(pTargetDirection.x, pTargetDirection.z) * Mathf.Rad2Deg;

		float fRotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, m_TargetRotation, ref m_RotationVelocity, m_RotationSmoothTime);

		// rotate to face target direction relative to camera position
		transform.rotation = Quaternion.Euler(0.0f, fRotation, 0.0f);

		pTargetDirection = Quaternion.Euler(0.0f, m_TargetRotation, 0.0f) * Vector3.forward;

		Vector3 pMotion = pTargetDirection.normalized * (m_Speed * Time.deltaTime) + new Vector3(0.0f, m_VerticalVelocity, 0.0f) * Time.deltaTime; ;

		// move the player
		m_Controller.Move(pMotion);

		// update animator if using character
		if (m_HasAnimator)
		{
			if (m_Speed > 1.0f)
			{
				m_Animator.SetBool(m_AnimIDWalk, false);
				m_Animator.SetBool(m_AnimIDRun, true);
			}
			else if (m_Speed > 0.1f)
			{
				m_Animator.SetBool(m_AnimIDWalk, true);
				m_Animator.SetBool(m_AnimIDRun, false);
			}
			else
			{
				m_Animator.SetBool(m_AnimIDWalk, false);
				m_Animator.SetBool(m_AnimIDRun, false);
			}

			m_Animator.SetFloat(m_AnimIDSpeed, m_AnimationBlend);
		}
	}

	private void Attack()
	{
		m_EnemyUnits.DoAttack();

		// update animator if using character
		if (m_HasAnimator)
		{
			m_Animator.Play("Z_Attack");
			m_Animator.SetBool(m_AnimIDWalk, false);
			m_Animator.SetBool(m_AnimIDRun, false);
		}
	}

	public void OnDead()
	{
		m_Controller.enabled = false;
		m_Controller.detectCollisions = false;

		ParticleSystem pParticleSystem = GetComponentInChildren<ParticleSystem>();

		if (pParticleSystem != null) pParticleSystem.Stop(true);

		// fade in to ground
		m_Tweener = transform.DOMoveY(-2f, 2f).SetDelay(5f).OnComplete(() =>
		{
			ObjectPoolManager.GetInstance.Release(gameObject);
		});

		// update animator if using character
		if (m_HasAnimator)
		{
			int nRandom = UnityEngine.Random.Range(0, 2);

			if (nRandom == 0)
			{
				m_Animator.Play("Z_FallingForward");
			}
			else
			{
				m_Animator.Play("Z_FallingBack");
			}

			m_Animator.SetBool(m_AnimIDWalk, false);
			m_Animator.SetBool(m_AnimIDRun, false);
		}
	}
}
