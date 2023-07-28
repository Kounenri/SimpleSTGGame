using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseUnits : MonoBehaviour
{
	[SerializeField]
	private float m_TotalHP = 100f;
	[SerializeField]
	private float m_MoveSpeed = 2.0f;

	protected float m_CurrentHP = 0f;

	public bool IsDead()
	{
		return m_CurrentHP <= 0f;
	}

	public float MoveSpeed
	{
		get { return m_MoveSpeed; }
	}

	public void ResetUnit()
	{
		m_CurrentHP = m_TotalHP;
	}

	public void BeAttack(float fDamage)
	{
		m_CurrentHP -= fDamage;

		OnHPChange();

		if (m_CurrentHP < 0f)
		{
			OnDead();
		}
	}

	protected virtual void Awake()
	{
		m_CurrentHP = m_TotalHP;
	}

	protected virtual void OnHPChange()
	{

	}

	protected virtual void OnDead()
	{

	}
}
