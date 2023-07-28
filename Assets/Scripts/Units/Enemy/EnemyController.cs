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

	private EnemyUnits m_EnemyUnits;
	private Animator m_Animator;
	private CharacterController m_Controller;

	// player
	private float m_Speed;
	private float m_AnimationBlend;
	private float m_TargetRotation = 0.0f;
	private float m_RotationVelocity;

	// animation IDs
	private int m_AnimIDSpeed;
	private int m_AnimIDWalk;
	private int m_AnimIDRun;

	private bool m_HasAnimator;

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
			Vector3 pMoveAmount = m_EnemyUnits.GetPlayerPosition() - transform.position;

			if (pMoveAmount.magnitude < m_EnemyUnits.AttackRange && !m_EnemyUnits.PlayerUnits.IsDead())
			{
				Attack();
			}
			else
			{
				Move(pMoveAmount);
			}
		}
	}

	private void Move(Vector3 pMoveAmount)
	{
		// set target speed based on move speed
		float fTargetSpeed = m_EnemyUnits.MoveSpeed;

		if (m_EnemyUnits.PlayerUnits.IsDead()) fTargetSpeed = 0.0f;

		// a reference to the players current horizontal velocity
		float fCurrentHorizontalSpeed = new Vector3(m_Controller.velocity.x, 0.0f, m_Controller.velocity.z).magnitude;
		float fSpeedOffset = 0.1f;

		// accelerate or decelerate to target speed
		if (fCurrentHorizontalSpeed < fTargetSpeed - fSpeedOffset || fCurrentHorizontalSpeed > fTargetSpeed + fSpeedOffset)
		{
			// creates curved result rather than a linear one giving a more organic speed change
			// note T in Lerp is clamped, so we don't need to clamp our speed
			m_Speed = Mathf.Lerp(fCurrentHorizontalSpeed, fTargetSpeed, Time.deltaTime * m_SpeedChangeRate);

			// round speed to 3 decimal places
			m_Speed = Mathf.Round(m_Speed * 1000f) / 1000f;
		}
		else
		{
			m_Speed = fTargetSpeed;
		}

		m_AnimationBlend = Mathf.Lerp(m_AnimationBlend, fTargetSpeed, Time.deltaTime * m_SpeedChangeRate);

		if (m_AnimationBlend < 0.01f) m_AnimationBlend = 0f;

		Vector2 pMoveAmount2 = new Vector2(pMoveAmount.x, pMoveAmount.z);

		// normalise target direction
		Vector3 pTargetDirection = new Vector3(pMoveAmount2.x, 0.0f, pMoveAmount2.y).normalized;

		// note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
		// if there is a move target rotate player when the player is moving
		if (pMoveAmount2 != Vector2.zero)
		{
			m_TargetRotation = Mathf.Atan2(pTargetDirection.x, pTargetDirection.z) * Mathf.Rad2Deg;

			float fRotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, m_TargetRotation, ref m_RotationVelocity, m_RotationSmoothTime);

			// rotate to face target direction relative to camera position
			transform.rotation = Quaternion.Euler(0.0f, fRotation, 0.0f);
		}

		pTargetDirection = Quaternion.Euler(0.0f, m_TargetRotation, 0.0f) * Vector3.forward;

		Vector3 pMotion = pTargetDirection.normalized * (m_Speed * Time.deltaTime);
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
		if (m_EnemyUnits.IsDead())
		{
			// update animator if using character
			if (m_HasAnimator)
			{
				int nRandom = UnityEngine.Random.Range(0, 1);

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
}
