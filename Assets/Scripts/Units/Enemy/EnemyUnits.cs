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

	protected override void OnDead()
	{


		base.OnDead();
	}
}
