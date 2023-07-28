using Cinemachine;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerUnits))]
public class PlayerController : MonoBehaviour
{
	[Header("Player")]
	[Range(0.0f, 0.3f)]
	[SerializeField]
	private float m_RotationSmoothTime = 0.12f;
	[SerializeField]
	private float m_SpeedChangeRate = 10.0f;
	[Header("Movement Settings")]
	[SerializeField]
	private bool m_AnalogMovement;
	[Header("Player Grounded")]
	[SerializeField]
	private bool m_Grounded = true;
	[SerializeField]
	private float m_GroundedOffset = -0.14f;
	[SerializeField]
	private float m_GroundedRadius = 0.28f;
	[SerializeField]
	private LayerMask m_GroundLayers;
	[Space(10)]
	[SerializeField]
	private bool m_Jump = false;
	[SerializeField]
	private float m_JumpHeight = 1.2f;
	[SerializeField]
	private float m_Gravity = -15.0f;
	[SerializeField]
	private float m_JumpTimeout = 0.50f;
	[SerializeField]
	private float m_FallTimeout = 0.15f;
	[SerializeField]
	private AudioClip[] m_FootstepAudioClips;
	[Range(0, 1)]
	[SerializeField]
	private float m_FootstepAudioVolume = 0.5f;
	[SerializeField]
	private AudioClip m_LandingAudioClip;
	[SerializeField]
	private AudioClip m_ReloadAudioClip;
	[SerializeField]
	private AudioClip m_DieAudioClip;

	private PlayerUnits m_PlayerUnits;
	private Camera m_MainCamera;
	private Animator m_Animator;
	private CharacterController m_Controller;

	// player
	private Vector2 m_MoveAmount;
	private float m_Speed;
	private float m_AnimationBlend;
	private float m_TargetRotation = 0.0f;
	private float m_RotationVelocity;
	private float m_VerticalVelocity;
	private float m_TerminalVelocity = 53.0f;

	// timeout deltatime
	private float m_JumpTimeoutDelta;
	private float m_FallTimeoutDelta;

	// animation IDs
	private int m_AnimIDSpeed;
	private int m_AnimIDFireSingle;
	private int m_AnimIDFireBurst;
	private int m_AnimIDFireAuto;
	private int m_AnimIDJump;
	private int m_AnimIDRun;
	private int m_AnimIDGrounded;
	private int m_AnimIDFront;
	private int m_AnimIDBack;
	private int m_AnimIDLeft;
	private int m_AnimIDRight;
	private int m_AnimIDMotionSpeed;

	private bool m_HasAnimator;

	private void Awake()
	{
		m_PlayerUnits = GetComponent<PlayerUnits>();
		m_MainCamera = Camera.main;
		m_Controller = GetComponent<CharacterController>();
	}

	private void Start()
	{
		m_HasAnimator = TryGetComponent(out m_Animator);

		AssignAnimationIDs();
	}

	private void OnEnable()
	{
		var pCinemachineVirtualCamera = FindObjectOfType<CinemachineVirtualCamera>();

		if (pCinemachineVirtualCamera != null)
		{
			pCinemachineVirtualCamera.Follow = transform;
		}
	}

	private void Update()
	{
		m_HasAnimator = TryGetComponent(out m_Animator);

		if (!m_PlayerUnits.IsDead())
		{
			JumpAndGravity();
			GroundedCheck();
			Move();
		}
	}

	private void AssignAnimationIDs()
	{
		m_AnimIDSpeed = Animator.StringToHash("Speed");
		m_AnimIDFireSingle = Animator.StringToHash("FireSingle");
		m_AnimIDFireBurst = Animator.StringToHash("FireBurst");
		m_AnimIDFireAuto = Animator.StringToHash("FireAuto");
		m_AnimIDJump = Animator.StringToHash("Jump");
		m_AnimIDRun = Animator.StringToHash("Run");
		m_AnimIDGrounded = Animator.StringToHash("Grounded");
		m_AnimIDFront = Animator.StringToHash("Front");
		m_AnimIDBack = Animator.StringToHash("Back");
		m_AnimIDLeft = Animator.StringToHash("Left");
		m_AnimIDRight = Animator.StringToHash("Right");
		m_AnimIDMotionSpeed = Animator.StringToHash("MotionSpeed");
	}

	private void GroundedCheck()
	{
		// set sphere position, with offset
		Vector3 pSpherePosition = new Vector3(transform.position.x, transform.position.y - m_GroundedOffset, transform.position.z);
		m_Grounded = Physics.CheckSphere(pSpherePosition, m_GroundedRadius, m_GroundLayers, QueryTriggerInteraction.Ignore);

		// update animator if using character
		if (m_HasAnimator)
		{
			m_Animator.SetBool(m_AnimIDGrounded, m_Grounded);
		}
	}

	private void Move()
	{
		// set target speed based on move speed
		float fTargetSpeed = m_PlayerUnits.MoveSpeed;

		// a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

		// note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
		// if there is no input, set the target speed to 0
		if (m_MoveAmount == Vector2.zero) fTargetSpeed = 0.0f;

		// a reference to the players current horizontal velocity
		float fCurrentHorizontalSpeed = new Vector3(m_Controller.velocity.x, 0.0f, m_Controller.velocity.z).magnitude;
		float fSpeedOffset = 0.1f;
		float fInputMagnitude = m_AnalogMovement ? m_MoveAmount.magnitude : 1f;

		// accelerate or decelerate to target speed
		if (fCurrentHorizontalSpeed < fTargetSpeed - fSpeedOffset || fCurrentHorizontalSpeed > fTargetSpeed + fSpeedOffset)
		{
			// creates curved result rather than a linear one giving a more organic speed change
			// note T in Lerp is clamped, so we don't need to clamp our speed
			m_Speed = Mathf.Lerp(fCurrentHorizontalSpeed, fTargetSpeed * fInputMagnitude, Time.deltaTime * m_SpeedChangeRate);

			// round speed to 3 decimal places
			m_Speed = Mathf.Round(m_Speed * 1000f) / 1000f;
		}
		else
		{
			m_Speed = fTargetSpeed;
		}

		m_AnimationBlend = Mathf.Lerp(m_AnimationBlend, fTargetSpeed, Time.deltaTime * m_SpeedChangeRate);

		if (m_AnimationBlend < 0.01f) m_AnimationBlend = 0f;

		// normalise input direction
		Vector3 pInputDirection = new Vector3(m_MoveAmount.x, 0.0f, m_MoveAmount.y).normalized;

		// note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
		// if there is a move input rotate player when the player is moving
		if (m_MoveAmount != Vector2.zero)
		{
			m_TargetRotation = Mathf.Atan2(pInputDirection.x, pInputDirection.z) * Mathf.Rad2Deg + m_MainCamera.transform.eulerAngles.y;

			float fRotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, m_TargetRotation, ref m_RotationVelocity, m_RotationSmoothTime);

			// rotate to face input direction relative to camera position
			//transform.rotation = Quaternion.Euler(0.0f, fRotation, 0.0f);
		}

		Vector3 pTargetDirection = Quaternion.Euler(0.0f, m_TargetRotation, 0.0f) * Vector3.forward;

		Vector3 pMotion = pTargetDirection.normalized * (m_Speed * Time.deltaTime) + new Vector3(0.0f, m_VerticalVelocity, 0.0f) * Time.deltaTime;
		// move the player
		m_Controller.Move(pMotion);

		// update animator if using character
		if (m_HasAnimator)
		{
			if (m_Speed > 0.1f)
			{
				m_Animator.SetBool(m_AnimIDFront, pTargetDirection.z > 0);
				m_Animator.SetBool(m_AnimIDBack, pTargetDirection.z < 0);

				m_Animator.SetBool(m_AnimIDRight, pTargetDirection.x > 0);
				m_Animator.SetBool(m_AnimIDLeft, pTargetDirection.x < 0);
			}
			else
			{
				m_Animator.SetBool(m_AnimIDFront, false);
				m_Animator.SetBool(m_AnimIDBack, false);
				m_Animator.SetBool(m_AnimIDLeft, false);
				m_Animator.SetBool(m_AnimIDRight, false);
			}

			m_Animator.SetFloat(m_AnimIDSpeed, m_AnimationBlend);
			m_Animator.SetFloat(m_AnimIDMotionSpeed, fInputMagnitude);
		}
	}

	private void JumpAndGravity()
	{
		if (m_Grounded)
		{
			// reset the fall timeout timer
			m_FallTimeoutDelta = m_FallTimeout;

			// update animator if using character
			if (m_HasAnimator)
			{
				m_Animator.SetBool(m_AnimIDJump, false);
			}

			// stop our velocity dropping infinitely when grounded
			if (m_VerticalVelocity < 0.0f)
			{
				m_VerticalVelocity = -2f;
			}

			// Jump
			if (m_Jump && m_JumpTimeoutDelta <= 0.0f)
			{
				// the square root of H * -2 * G = how much velocity needed to reach desired height
				m_VerticalVelocity = Mathf.Sqrt(m_JumpHeight * -2f * m_Gravity);

				// update animator if using character
				if (m_HasAnimator)
				{
					m_Animator.SetBool(m_AnimIDJump, true);
				}
			}

			// jump timeout
			if (m_JumpTimeoutDelta >= 0.0f)
			{
				m_JumpTimeoutDelta -= Time.deltaTime;
			}
		}
		else
		{
			// reset the jump timeout timer
			m_JumpTimeoutDelta = m_JumpTimeout;

			// fall timeout
			if (m_FallTimeoutDelta >= 0.0f)
			{
				m_FallTimeoutDelta -= Time.deltaTime;
			}

			// if we are not grounded, do not jump
			m_Jump = false;
		}

		// apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
		if (m_VerticalVelocity < m_TerminalVelocity)
		{
			m_VerticalVelocity += m_Gravity * Time.deltaTime;
		}
	}

	private static float ClampAngle(float fAngle, float fMin, float fMax)
	{
		if (fAngle < -360f) fAngle += 360f;
		if (fAngle > 360f) fAngle -= 360f;
		return Mathf.Clamp(fAngle, fMin, fMax);
	}

	private void OnFootstepSound(AnimationEvent animationEvent)
	{
		if (animationEvent.animatorClipInfo.weight > 0.5f)
		{
			if (m_FootstepAudioClips.Length > 0)
			{
				var index = UnityEngine.Random.Range(0, m_FootstepAudioClips.Length);
				SoundManager.PlaySound(m_FootstepAudioClips[index], 0, m_FootstepAudioVolume);
			}
		}
	}

	private void OnLandSound(AnimationEvent animationEvent)
	{
		if (animationEvent.animatorClipInfo.weight > 0.5f)
		{
			SoundManager.PlaySound(m_LandingAudioClip, 0, m_FootstepAudioVolume);
		}
	}

	private void OnReloadSound(AnimationEvent animationEvent)
	{
		if (animationEvent.animatorClipInfo.weight > 0.5f)
		{
			SoundManager.PlaySound(m_ReloadAudioClip, 0, m_FootstepAudioVolume);
		}
	}

	private void OnDieSound(AnimationEvent animationEvent)
	{
		if (animationEvent.animatorClipInfo.weight > 0.5f)
		{
			SoundManager.PlaySound(m_DieAudioClip, 0, m_FootstepAudioVolume);
		}
	}

	public void OnMove(InputAction.CallbackContext context)
	{
		if (!m_PlayerUnits.IsDead())
		{
			// read the value for the "Move" action each event call
			m_MoveAmount = context.ReadValue<Vector2>();
		}
	}

	public void OnJump(InputAction.CallbackContext context)
	{
		if (!m_PlayerUnits.IsDead())
		{
			m_Jump = true;
		}
	}

	public void OnFire(InputAction.CallbackContext context)
	{
		if (!m_PlayerUnits.IsDead())
		{
			m_PlayerUnits.FireWeapon();
		}
	}

	public void OnReload(InputAction.CallbackContext context)
	{
		if (!m_PlayerUnits.IsDead())
		{
			m_PlayerUnits.ReloadWeapon();

			// update animator if using character
			if (m_HasAnimator)
			{
				m_Animator.Play("Reload");
				m_Animator.SetBool(m_AnimIDFront, false);
				m_Animator.SetBool(m_AnimIDBack, false);
				m_Animator.SetBool(m_AnimIDLeft, false);
				m_Animator.SetBool(m_AnimIDRight, false);
			}
		}
	}

	public void OnDead()
	{
		if (m_PlayerUnits.IsDead())
		{
			// update animator if using character
			if (m_HasAnimator)
			{
				m_Animator.Play("Die");
				m_Animator.SetBool(m_AnimIDFront, false);
				m_Animator.SetBool(m_AnimIDBack, false);
				m_Animator.SetBool(m_AnimIDLeft, false);
				m_Animator.SetBool(m_AnimIDRight, false);
			}
		}
	}

	public void OnPointerMove(InputAction.CallbackContext context)
	{
		if (!m_PlayerUnits.IsDead())
		{
			// read the value for the "Pointer Move" action each event call
			Vector2 pPointerPosition = context.ReadValue<Vector2>();

			(bool bIsSuccess, Vector3 pPosition) = GetCursorPosition(pPointerPosition);
			if (bIsSuccess)
			{
				// Calculate the direction
				Vector3 pDirection = pPosition - transform.position;

				// Ignore the height difference.
				pDirection.y = 0;

				// Make the transform look in the direction.
				transform.forward = pDirection;
			}
		}
	}

	private (bool bIsSuccess, Vector3 pPosition) GetCursorPosition(Vector2 pPointerPosition)
	{
		Ray pRay = m_MainCamera.ScreenPointToRay(pPointerPosition);

		if (Physics.Raycast(pRay, out RaycastHit pHitInfo, 200f))
		{
			// The Raycast hit something, return with the position.
			return (bIsSuccess: true, pPosition: pHitInfo.point);
		}
		else
		{
			// The Raycast did not hit anything.
			return (bIsSuccess: false, pPosition: Vector3.zero);
		}
	}
}
