using UnityEngine;

public class BaseUnits : MonoBehaviour
{
	protected int m_CurrentHP = 0;

	public int CurrentHP { get { return m_CurrentHP; } }

	public bool IsDead() { return m_CurrentHP <= 0; }

	protected virtual void OnHPChange() { }

	protected virtual void OnGetHit() { }

	protected virtual void OnDead() { }

	public void BeAttack(int nDamage)
	{
		m_CurrentHP -= nDamage;

		OnGetHit();

		if (m_CurrentHP <= 0f)
		{
			m_CurrentHP = 0;

			OnHPChange();

			OnDead();
		}
		else
		{
			OnHPChange();
		}
	}
}
