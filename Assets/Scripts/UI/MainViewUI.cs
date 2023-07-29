using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MainViewUI : BaseCanvas
{
	private TMP_Text m_HPText;
	private TMP_Text m_WeaponNameText;
	private TMP_Text m_WeaponStatusText;

	protected override void Awake()
	{
		base.Awake();

		m_HPText = this.Find<TMP_Text>("Panel_HP/Text_HP");
		m_WeaponNameText = this.Find<TMP_Text>("Panel_Weapon/Text_Name");
		m_WeaponStatusText = this.Find<TMP_Text>("Panel_Weapon/Text_Count");
	}

	protected override void Start()
	{
		base.Start();

		if (LevelController.HasInstance)
		{
			LevelController.GetInstance.AddEventListener(LevelController.ON_PLAYER_HP_CHANG, OnPlayerHPChanged);
			LevelController.GetInstance.AddEventListener(LevelController.ON_WEAPON_CHANGE, OnWeaponChanged);
			LevelController.GetInstance.AddEventListener(LevelController.ON_BULLET_COUNT_CHANGE, OnBulletCountChanged);
		}
	}

	protected override void OnDestroy()
	{
		if (LevelController.HasInstance)
		{
			LevelController.GetInstance.RemoveEventListener(LevelController.ON_BULLET_COUNT_CHANGE, OnBulletCountChanged);
			LevelController.GetInstance.RemoveEventListener(LevelController.ON_WEAPON_CHANGE, OnWeaponChanged);
			LevelController.GetInstance.RemoveEventListener(LevelController.ON_PLAYER_HP_CHANG, OnPlayerHPChanged);
		}

		base.OnDestroy();
	}

	private void OnPlayerHPChanged(TEvent pTEvent)
	{
		m_HPText.text = "HP : " + pTEvent.Data.ToString();
	}

	private void OnWeaponChanged(TEvent pTEvent)
	{
		m_WeaponNameText.text = (pTEvent.Data as WeaponVO).Name;
	}

	private void OnBulletCountChanged(TEvent pTEvent)
	{
		m_WeaponStatusText.text = pTEvent.Data.ToString() + "/" + LevelController.GetInstance.CurrentWeapon.Capacity;
	}
}
