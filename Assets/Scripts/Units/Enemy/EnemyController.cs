using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

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
	private GameObject m_PlayerObject;
	private PlayerUnits m_PlayerUnits;
	private Vector3 m_PlayerPosition;

	// player
	private float m_Speed;
	private float m_AnimationBlend;
	private float m_TargetRotation = 0.0f;
	private float m_RotationVelocity;

	// animation IDs
	private int m_AnimIDSpeed;
	private int m_AnimIDWalk;
	private int m_AnimIDRun;
	private int m_AnimIDAttack;
	private int m_AnimIDDieForwad;
	private int m_AnimIDDieBackward;

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
		m_AnimIDAttack = Animator.StringToHash("Attack");
		m_AnimIDDieForwad = Animator.StringToHash("DieForward");
		m_AnimIDDieBackward = Animator.StringToHash("DieBackward");
	}

	private void OnEnable()
	{
		m_PlayerObject = GameObject.FindGameObjectWithTag("Player");

		if (m_PlayerObject != null)
		{
			m_PlayerUnits = m_PlayerObject.GetComponent<PlayerUnits>();
		}
		else
		{
			gameObject.SetActive(false);

			Debug.Log("Can't find player object!");
		}
	}

	private void Update()
	{
		m_HasAnimator = TryGetComponent(out m_Animator);

		GetPlayerPosition();

		Move();
	}

	private void GetPlayerPosition()
	{
		m_PlayerPosition = m_PlayerObject.transform.position;
	}

	private void Move()
	{
		// set target speed based on move speed
		float fTargetSpeed = m_EnemyUnits.MoveSpeed;

		if (m_PlayerUnits.IsDead()) fTargetSpeed = 0.0f;

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

		Vector3 pVector = m_PlayerPosition - transform.position;

		if (pVector.magnitude < m_EnemyUnits.AttackRange && !m_PlayerUnits.IsDead())
		{
			m_Speed = 0f;

			DoAttack();
		}

		Vector2 m_MoveAmount = new Vector2(pVector.x, pVector.z);

		// normalise target direction
		Vector3 pTargetDirection = new Vector3(m_MoveAmount.x, 0.0f, m_MoveAmount.y).normalized;

		// note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
		// if there is a move target rotate player when the player is moving
		if (m_MoveAmount != Vector2.zero)
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
			if (pVector.magnitude < m_EnemyUnits.AttackRange && !m_PlayerUnits.IsDead())
			{
				m_Animator.SetBool(m_AnimIDWalk, false);
				m_Animator.SetBool(m_AnimIDRun, false);
				m_Animator.SetBool(m_AnimIDAttack, true);
			}
			else
			{
				if (m_Speed > 1.0f)
				{
					m_Animator.SetBool(m_AnimIDWalk, false);
					m_Animator.SetBool(m_AnimIDRun, true);
					m_Animator.SetBool(m_AnimIDAttack, false);
				}
				else if (m_Speed > 0.1f)
				{
					m_Animator.SetBool(m_AnimIDWalk, true);
					m_Animator.SetBool(m_AnimIDRun, false);
					m_Animator.SetBool(m_AnimIDAttack, false);
				}
				else
				{
					m_Animator.SetBool(m_AnimIDWalk, false);
					m_Animator.SetBool(m_AnimIDRun, false);
					m_Animator.SetBool(m_AnimIDAttack, false);
				}
			}

			m_Animator.SetFloat(m_AnimIDSpeed, m_AnimationBlend);
		}
	}

	private float m_LastAttackTime = 0f;

	private void DoAttack()
	{
		if (!m_PlayerUnits.IsDead())
		{
			float fDeltaTime = Time.deltaTime;

			if (m_LastAttackTime > m_EnemyUnits.AttackInterval)
			{
				m_PlayerUnits.BeAttack(m_EnemyUnits.Damage);

				m_LastAttackTime = 0;
			}

			m_LastAttackTime += fDeltaTime;
		}
	}
}
