using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MainViewUI : BaseCanvas
{
	private TMP_Text m_HPText;

	protected override void Awake()
	{
		base.Awake();

		m_HPText = this.Find<TMP_Text>("Panel/Text_HP");
	}

	protected override void Start()
	{
		base.Start();

		if (LevelController.HasInstance)
		{
			LevelController.GetInstance.AddEventListener(LevelController.ON_PLAYER_HP_CHANGED, OnPlayerHPChanged);
		}
	}

	protected override void OnDestroy()
	{
		if (LevelController.HasInstance)
		{
			LevelController.GetInstance.RemoveEventListener(LevelController.ON_PLAYER_HP_CHANGED);
		}

		base.OnDestroy();
	}

	private void OnPlayerHPChanged(TEvent pTEvent)
	{
		m_HPText.text = "HP : " + pTEvent.Data.ToString();
	}
}
