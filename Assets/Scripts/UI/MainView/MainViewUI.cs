using DG.Tweening;
using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class MainViewUI : BaseCanvas
{
	private TMP_Text m_CountDownText;
	private TMP_Text m_EnemyCountText;
	private TMP_Text m_HPText;
	private TMP_Text m_WeaponNameText;
	private TMP_Text m_WeaponStatusText;
	private SimpleVList m_VList;

	private Tweener m_Tweener;
	private WeaponVO m_WeaponVO;

	protected override void Awake()
	{
		base.Awake();

		m_CountDownText = this.Find<TMP_Text>("Text_CountDown");
		m_EnemyCountText = this.Find<TMP_Text>("Text_EnemyCount");
		m_HPText = this.Find<TMP_Text>("Panel_HP/Text_HP");
		m_WeaponNameText = this.Find<TMP_Text>("Panel_Weapon/Text_Name");
		m_WeaponStatusText = this.Find<TMP_Text>("Panel_Weapon/Text_Count");
		m_VList = this.Find<SimpleVList>("Panel_Weapon/Panel");
	}

	protected override void Start()
	{
		base.Start();

		if (LevelManager.HasInstance)
		{
			LevelManager.GetInstance.AddEventListener(LevelManager.ON_LEVEL_CHANGE, OnLevelChange);
			LevelManager.GetInstance.AddEventListener(LevelManager.ON_PLAYER_HP_CHANG, OnPlayerHPChanged);
			LevelManager.GetInstance.AddEventListener(LevelManager.ON_WEAPON_CHANGE, OnWeaponChanged);
			LevelManager.GetInstance.AddEventListener(LevelManager.ON_BULLET_COUNT_CHANGE, OnBulletCountChanged);
			LevelManager.GetInstance.AddEventListener(LevelManager.ON_LEFT_ENEMY_COUNT_CHANGE, OnLeftEnemyCountChange);
		}

		m_VList.DataProvider = DataProviderUtil.GetDataProvier(WeaponConfProxy.GetInstance.GetDataVOList());

		OnWeaponChanged();

		StartCoroutine(OnCountDownCoroutine());
	}

	protected override void OnDestroy()
	{
		if (LevelManager.HasInstance)
		{
			LevelManager.GetInstance.RemoveEventListener(LevelManager.ON_LEFT_ENEMY_COUNT_CHANGE, OnLeftEnemyCountChange);
			LevelManager.GetInstance.RemoveEventListener(LevelManager.ON_BULLET_COUNT_CHANGE, OnBulletCountChanged);
			LevelManager.GetInstance.RemoveEventListener(LevelManager.ON_WEAPON_CHANGE, OnWeaponChanged);
			LevelManager.GetInstance.RemoveEventListener(LevelManager.ON_PLAYER_HP_CHANG, OnPlayerHPChanged);
			LevelManager.GetInstance.RemoveEventListener(LevelManager.ON_LEVEL_CHANGE, OnLevelChange);
		}

		base.OnDestroy();
	}

	private IEnumerator OnCountDownCoroutine()
	{
		while (LevelController.HasInstance)
		{
			if (!LevelManager.GetInstance.IsRunning)
			{
				if (m_Tweener != null)
				{
					m_Tweener.Kill();
					m_Tweener = null;
				}

				m_CountDownText.text = "";

				yield return new WaitForEndOfFrame();
			}
			else
			{
				float fLeftTime = LevelController.GetInstance.LeftTime;

				if (fLeftTime > 60f)
				{
					if (m_Tweener != null)
					{
						m_Tweener.Kill();
						m_Tweener = null;
					}

					m_CountDownText.text = TimeSpan.FromSeconds(fLeftTime).ToString(@"mm\:ss");
					m_CountDownText.color = Color.white;

					yield return new WaitForSeconds(1f);
				}
				else if (fLeftTime >= 0f)
				{
					m_Tweener ??= m_CountDownText.DOColor(Color.red, 1f).SetLoops(-1);

					m_CountDownText.text = fLeftTime.ToString("N2");

					yield return new WaitForSeconds(0.1f);
				}
			}
		}
	}

	private void OnLevelChange(TEvent pTEvent = null)
	{
		LevelVO pLevelVO = pTEvent.Data as LevelVO;

	}

	private void OnPlayerHPChanged(TEvent pTEvent)
	{
		m_HPText.text = "HP : " + (int)pTEvent.Data;
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

	private void OnLeftEnemyCountChange(TEvent pTEvent)
	{
		m_EnemyCountText.text = "Enemy " + pTEvent.Data.ToString() + " " + GameObject.FindGameObjectsWithTag("Enemy").Length;
	}
}
