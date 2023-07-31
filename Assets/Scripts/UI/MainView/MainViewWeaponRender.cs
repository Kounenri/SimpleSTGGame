using System;
using TMPro;
using UnityEditor.iOS;
using UnityEngine;
using UnityEngine.UI;

public class MainViewWeaponRender : ARender
{
	private Image m_BGImage;
	private Image m_IconImage;
	private TMP_Text m_NameText;

	private WeaponVO m_WeaponVO;

	private void Awake()
	{
		m_BGImage = this.Find<Image>("BG");
		m_IconImage = this.Find<Image>("Image_Icon");
		m_NameText = this.Find<TMP_Text>("Text_Name");
	}

	private void Start()
	{
		if (LevelManager.HasInstance)
		{
			LevelManager.GetInstance.AddEventListener(LevelManager.ON_LEVEL_CHANGE, OnChange);
			LevelManager.GetInstance.AddEventListener(LevelManager.ON_WEAPON_CHANGE, OnChange);
		}
	}

	private void OnDestroy()
	{
		if (LevelManager.HasInstance)
		{
			LevelManager.GetInstance.RemoveEventListener(LevelManager.ON_WEAPON_CHANGE, OnChange);
			LevelManager.GetInstance.RemoveEventListener(LevelManager.ON_LEVEL_CHANGE, OnChange);
		}
	}

	protected override void OnSetData()
	{
		base.OnSetData();

		m_WeaponVO = Data as WeaponVO;

		m_NameText.text = m_WeaponVO.Name;
		m_IconImage.sprite = Resources.Load<Sprite>(m_WeaponVO.Icon);
		m_IconImage.preserveAspect = true;
	}

	protected override void RefreshView()
	{
		base.RefreshView();

		if (LevelManager.GetInstance.CurrentLevelVO.UnlockedWeapon.IndexOf(m_WeaponVO.ID) != -1)
		{
			m_NameText.color = Color.white;
			m_IconImage.color = Color.white;
		}
		else
		{
			m_NameText.color = Color.gray;
			m_IconImage.color = Color.gray;
		}

		if (LevelManager.GetInstance.CurrentWeaponVO.ID == m_WeaponVO.ID)
		{
			m_BGImage.color = new Color(0f, 1f, 0f, 0.5f);
		}
		else
		{
			m_BGImage.color = new Color(0f, 0f, 0f, 0.5f);
		}
	}

	private void OnChange(TEvent pTEvent)
	{
		Invalidate = true;
	}
}
