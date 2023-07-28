using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyUnits : BaseUnits
{
	[SerializeField]
	private float m_Damage = 1f;
	[SerializeField]
	private float m_AttackRange = 1f;
	[SerializeField]
	private float m_AttackInterval = 1f;

	private float m_LastAttackTime = 0f;

	private EnemyController m_EnemyController;
	private GameObject m_PlayerObject;
	private PlayerUnits m_PlayerUnits;

	public float Damage
	{
		get { return m_Damage; }
	}

	public float AttackRange
	{
		get { return m_AttackRange; }
	}

	public float AttackInterval
	{
		get { return m_AttackInterval; }
	}

	public PlayerUnits PlayerUnits
	{
		get { return m_PlayerUnits; }
	}

	protected override void Awake()
	{
		base.Awake();

		m_EnemyController = GetComponent<EnemyController>();
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

	protected override void OnDead()
	{
		m_EnemyController.OnDead();

		base.OnDead();
	}

	public Vector3 GetPlayerPosition()
	{
		return m_PlayerObject.transform.position;
	}

	public void DoAttack()
	{
		if (!IsDead())
		{
			if (Time.time - m_LastAttackTime > AttackInterval)
			{
				m_PlayerUnits.BeAttack(Damage);

				m_LastAttackTime = Time.time;
			}
		}
	}
}
