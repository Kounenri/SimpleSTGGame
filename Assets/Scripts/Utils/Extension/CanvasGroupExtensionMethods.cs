using DG.Tweening;
using UnityEngine;

public static class CanvasGroupExtensionMethods
{
	public static void Reset(this Transform pTransform)
	{
		pTransform.localScale = Vector3.one;
		pTransform.localPosition = Vector3.zero;
		pTransform.localRotation = Quaternion.identity;
	}

	/// <summary>
	/// Activates/Deactivates the CanvasGroup.
	/// </summary>
	/// <param name="pCanvasGroup">CanvasGroup</param>
	/// <param name="bActive">Activate or deactivation the object.</param>
	/// <returns></returns>
	public static bool SetActive(this CanvasGroup pCanvasGroup, bool bActive, bool bChangeInteractable = true)
	{
		if (pCanvasGroup != null)
		{
			if (bActive && pCanvasGroup.alpha < 1f)
			{
				pCanvasGroup.alpha = 1f;

				if (bChangeInteractable) pCanvasGroup.interactable = pCanvasGroup.blocksRaycasts = true;

				return true;
			}
			else if (!bActive && pCanvasGroup.alpha > 0f)
			{
				pCanvasGroup.alpha = 0f;

				if (bChangeInteractable) pCanvasGroup.interactable = pCanvasGroup.blocksRaycasts = false;

				return true;
			}
		}

		return false;
	}

	public static Tweener SetTweenActive(this CanvasGroup pCanvasGroup, bool bActive, bool bChangeInteractable = true, float fDuration = 0f)
	{
		if (pCanvasGroup != null)
		{
			if (bActive && pCanvasGroup.alpha < 1f)
			{
				if (fDuration == 0f)
				{
					fDuration = BaseCanvas.EFFECT_SHOW_TIME;
				}

				return pCanvasGroup.DOFade(1f, fDuration).OnComplete(() =>
				{
					if (bChangeInteractable) pCanvasGroup.interactable = pCanvasGroup.blocksRaycasts = true;
				});
			}
			else if (!bActive && pCanvasGroup.alpha > 0f)
			{
				if (fDuration == 0f)
				{
					fDuration = BaseCanvas.EFFECT_HIDE_TIME;
				}

				if (bChangeInteractable) pCanvasGroup.interactable = pCanvasGroup.blocksRaycasts = false;

				return pCanvasGroup.DOFade(0f, fDuration);
			}
		}

		return null;
	}
}
