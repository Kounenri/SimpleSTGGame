using UnityEngine;

public class EnemyUnits : BaseUnits
{
	private EnemyVO m_EnemyVO;
	private EnemyController m_Controller;

	private float m_LastAttackTime = 0f;

	public int MoveSpeed { get { return m_EnemyVO.MoveSpeed; } }

	public int Damage { get { return m_EnemyVO.Damage; } }

	public float AttackRange { get { return m_EnemyVO.AttackRange; } }

	public float AttackInterval { get { return m_EnemyVO.AttackInterval; } }

	private void Awake()
	{
		m_Controller = GetComponent<EnemyController>();
	}

	protected override void OnDead()
	{
		LevelController.GetInstance.OnDeactiveEnemy();

		m_Controller.OnDead();
	}

	public void ResetUnit(EnemyVO pEnemyVO)
	{
		m_EnemyVO = pEnemyVO;

		m_CurrentHP = m_EnemyVO.TotalHP;
	}

	public void DoAttack()
	{
		if (!IsDead())
		{
			if (m_LastAttackTime == 0f || Time.time - m_LastAttackTime > AttackInterval)
			{
				LevelController.GetInstance.CurrentPlayer.BeAttack(Damage);

				m_LastAttackTime = Time.time;
			}
		}
	}
}
