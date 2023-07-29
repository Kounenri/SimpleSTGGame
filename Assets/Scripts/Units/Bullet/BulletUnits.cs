using UnityEngine;

public class BulletUnits : BaseUnits
{
	private float m_LiftTime;
	private Vector3 m_Direction;
	private bool m_IsPenetrable;

	private void Update()
	{
		if (!IsDead())
		{
			float fDeltaTime = Time.deltaTime;

			transform.position += m_Direction.normalized * (m_MoveSpeed * fDeltaTime);

			m_LiftTime += fDeltaTime;

			if (m_LiftTime > 10f)
			{
				OnDead();
			}
		}
	}

	private void OnTriggerEnter(Collider pCollider)
	{
		if (pCollider.transform.tag == "Enemy")
		{
			EnemyUnits pEnemyUnits = pCollider.GetComponent<EnemyUnits>();

			pEnemyUnits.BeAttack(m_TotalHP);

			if (!m_IsPenetrable)
			{
				m_CurrentHP = 0f;

				OnDead();
			}
		}
	}

	protected override void OnDead()
	{
		ObjectPoolManager.GetInstance.Release(gameObject);

		base.OnDead();
	}

	public void Fire(Vector3 pDirection, float fDamage, float fSpeed, bool bIsPenetrable)
	{
		m_Direction = pDirection;
		m_CurrentHP = m_TotalHP = fDamage;
		m_MoveSpeed = fSpeed;
		m_IsPenetrable = bIsPenetrable;
		m_LiftTime = 0f;

		transform.up = pDirection;
	}
}
