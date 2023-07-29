using UnityEngine;

public class BaseUnits : MonoBehaviour
{
	[SerializeField]
	protected float m_TotalHP = 100f;
	[SerializeField]
	protected float m_MoveSpeed = 2.0f;

	protected float m_CurrentHP = 0f;

	public bool IsDead()
	{
		return m_CurrentHP <= 0f;
	}

	public float MoveSpeed
	{
		get { return m_MoveSpeed; }
	}

	protected virtual void OnHPChange()
	{

	}

	protected virtual void OnDead()
	{

	}

	public virtual void ResetUnit()
	{
		m_CurrentHP = m_TotalHP;

		OnHPChange();
	}

	public void BeAttack(float fDamage)
	{
		m_CurrentHP -= fDamage;

		OnHPChange();

		if (IsDead())
		{
			OnDead();
		}
	}
}
