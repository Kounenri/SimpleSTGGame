using UnityEngine;

public class BulletUnits : BaseUnits
{
	/// <summary>
	/// Moving Direction
	/// </summary>
	private Vector3 m_Direction;
	/// <summary>
	/// Bullet Damage
	/// </summary>
	private int m_Damage;
	/// <summary>
	/// Moving Speed
	/// </summary>
	private int m_Speed;
	/// <summary>
	/// Can Penetrable Enemy
	/// </summary>
	private bool m_IsPenetrable;
	/// <summary>
	/// Self Destruct Timer
	/// </summary>
	private float m_LiftTime;

	private void OnEnable()
	{
		ParticleSystem pParticleSystem = GetComponentInChildren<ParticleSystem>();

		if (pParticleSystem != null) pParticleSystem.Play(true);
	}

	private void OnDisable()
	{
		ParticleSystem pParticleSystem = GetComponentInChildren<ParticleSystem>();

		if (pParticleSystem != null) pParticleSystem.Stop(true);
	}

	private void Update()
	{
		if (!IsDead())
		{
			float fDeltaTime = Time.deltaTime;

			transform.position += m_Direction.normalized * (m_Speed * fDeltaTime);

			// set a timer for bullet destroy it self
			m_LiftTime += fDeltaTime;

			if (m_LiftTime > 10f) OnDead();
		}
	}

	private void OnTriggerEnter(Collider pCollider)
	{
		if (pCollider.transform.CompareTag("Enemy"))
		{
			if (pCollider.TryGetComponent<EnemyUnits>(out var pEnemyUnits))
			{
				if (!pEnemyUnits.IsDead())
				{
					pEnemyUnits.BeAttack(m_Damage);

					if (!m_IsPenetrable) OnDead();
				}
			}
		}
	}

	protected override void OnDead()
	{
		m_CurrentHP = 0;

		ObjectPoolManager.GetInstance.Release(gameObject);
	}

	public void Fire(Vector3 pDirection, int nDamage, int nSpeed, bool bIsPenetrable)
	{
		m_Direction = pDirection;
		m_CurrentHP = m_Damage = nDamage;
		m_Speed = nSpeed;
		m_IsPenetrable = bIsPenetrable;
		m_LiftTime = 0f;

		transform.up = pDirection;
	}
}
