using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class MainViewUI : BaseCanvas
{
	private TMP_Text m_CountDownText;
	private TMP_Text m_HPText;
	private TMP_Text m_WeaponNameText;
	private TMP_Text m_WeaponStatusText;

	private WeaponVO m_WeaponVO;

	protected override void Awake()
	{
		base.Awake();

		m_CountDownText = this.Find<TMP_Text>("Text_CountDown");
		m_HPText = this.Find<TMP_Text>("Panel_HP/Text_HP");
		m_WeaponNameText = this.Find<TMP_Text>("Panel_Weapon/Text_Name");
		m_WeaponStatusText = this.Find<TMP_Text>("Panel_Weapon/Text_Count");
	}

	protected override void Start()
	{
		base.Start();

		if (LevelManager.HasInstance)
		{
			LevelManager.GetInstance.AddEventListener(LevelManager.ON_PLAYER_HP_CHANG, OnPlayerHPChanged);
			LevelManager.GetInstance.AddEventListener(LevelManager.ON_WEAPON_CHANGE, OnWeaponChanged);
			LevelManager.GetInstance.AddEventListener(LevelManager.ON_BULLET_COUNT_CHANGE, OnBulletCountChanged);
		}

		OnPlayerHPChanged();
		OnWeaponChanged();

		StartCoroutine(OnCountDownCoroutine());
	}

	protected override void OnDestroy()
	{
		if (LevelManager.HasInstance)
		{
			LevelManager.GetInstance.RemoveEventListener(LevelManager.ON_BULLET_COUNT_CHANGE, OnBulletCountChanged);
			LevelManager.GetInstance.RemoveEventListener(LevelManager.ON_WEAPON_CHANGE, OnWeaponChanged);
			LevelManager.GetInstance.RemoveEventListener(LevelManager.ON_PLAYER_HP_CHANG, OnPlayerHPChanged);
		}

		base.OnDestroy();
	}

	private IEnumerator OnCountDownCoroutine()
	{
		while (LevelController.HasInstance)
		{
			float fLeftTime = LevelController.GetInstance.LeftTime;

			if (fLeftTime > 60f)
			{
				m_CountDownText.text = TimeSpan.FromSeconds(fLeftTime).ToString(@"mm\:ss");

				yield return new WaitForSeconds(1f);
			}
			else if (fLeftTime >= 0f)
			{
				m_CountDownText.text = fLeftTime.ToString("N2");

				yield return new WaitForSeconds(0.1f);
			}
			else
			{
				m_CountDownText.text = "";

				yield return new WaitForEndOfFrame();
			}
		}
	}

	private void OnPlayerHPChanged(TEvent pTEvent = null)
	{
		int nPlayerHP;

		if (pTEvent != null)
		{
			nPlayerHP = (int)pTEvent.Data;
		}
		else
		{
			nPlayerHP = LevelManager.GetInstance.PlayerHP;
		}

		m_HPText.text = "HP : " + nPlayerHP;
	}

	private void OnWeaponChanged(TEvent pTEvent = null)
	{
		if (pTEvent != null)
		{
			m_WeaponVO = pTEvent.Data as WeaponVO;
		}
		else
		{
			m_WeaponVO = LevelManager.GetInstance.CurrentWeaponVO;
		}

		m_WeaponNameText.text = m_WeaponVO.Name;
	}

	private void OnBulletCountChanged(TEvent pTEvent)
	{
		m_WeaponStatusText.text = (int)pTEvent.Data + "/" + m_WeaponVO.Capacity;
	}
}
