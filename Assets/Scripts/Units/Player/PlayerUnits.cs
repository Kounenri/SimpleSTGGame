using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUnits : BaseUnits
{
	private PlayerController m_PlayerController;

	protected override void Awake()
	{
		base.Awake();

		m_PlayerController = GetComponent<PlayerController>();
	}

	protected override void OnHPChange()
	{
		base.OnHPChange();

		LevelController.GetInstance.OnPlayerHPChanged(m_CurrentHP);
	}

	protected override void OnDead()
	{
		m_PlayerController.OnDead();

		base.OnDead();
	}
}
