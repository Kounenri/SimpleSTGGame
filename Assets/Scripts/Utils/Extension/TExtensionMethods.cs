using System;
using UnityEngine;
using UnityEngine.UI;

public static class TExtensionMethods
{
	private static string GetGameObjectPath(GameObject pGameObject)
	{
		string strPath = "/" + pGameObject.name;

		while (pGameObject.transform.parent != null)
		{
			pGameObject = pGameObject.transform.parent.gameObject;
			strPath = "/" + pGameObject.name + strPath;
		}

		return strPath;
	}

	private static Transform GetTransform(MonoBehaviour pMonoBehaviour, string strPath, Type pType)
	{
		Transform pTransform = pMonoBehaviour.transform.Find(strPath);

		if (pTransform == null)
		{
			throw new Exception(string.Format("Can't find[{0}].\nPath : {1}\nTarget Path : {2}", pType.ToString(), GetGameObjectPath(pMonoBehaviour.gameObject).Substring(1), strPath));
		}
		else
		{
			return pTransform;
		}
	}

	public static T Find<T>(this MonoBehaviour pMonoBehaviour, string strPath)
	{
		Transform pTarget = pMonoBehaviour.transform.Find(strPath);

		if (pTarget != null)
		{
			return pTarget.GetComponent<T>();
		}
		else
		{
			throw new Exception(string.Format("Can't find[{0}].\nPath : {1}\nTarget Path : {2}", typeof(T).ToString(), GetGameObjectPath(pMonoBehaviour.gameObject).Substring(1), strPath));
		}
	}

	public static T Find<T>(this Transform pTransform, string strPath)
	{
		Transform pTarget = pTransform.Find(strPath);

		if (pTarget != null)
		{
			return pTarget.GetComponent<T>();
		}
		else
		{
			throw new Exception(string.Format("Can't find[{0}].\nPath : {1}\nTarget Path : {2}", typeof(T).ToString(), GetGameObjectPath(pTransform.gameObject).Substring(1), strPath));
		}
	}

	public static T GetTUI<T>(this MonoBehaviour pMonoBehaviour, string strPath)
	{
		Type pType = typeof(T);
		Component pComponent = GetTransform(pMonoBehaviour, strPath, pType).GetComponent(pType);

		object pObject = Convert.ChangeType(pComponent, pType);

		return (T)pObject;
	}

	public static GameObject GetTGameObject(this MonoBehaviour pMonoBehaviour, string strPath)
	{
		return GetTransform(pMonoBehaviour, strPath, typeof(GameObject)).gameObject;
	}

	public static Transform GetTTransform(this MonoBehaviour pMonoBehaviour, string strPath)
	{
		return GetTransform(pMonoBehaviour, strPath, typeof(Transform));
	}

	public static RectTransform GetTRectTransform(this MonoBehaviour pMonoBehaviour, string strPath)
	{
		return GetTransform(pMonoBehaviour, strPath, typeof(RectTransform)).GetComponent<RectTransform>();
	}

	public static Canvas GetTCanvas(this MonoBehaviour pMonoBehaviour, string strPath)
	{
		return GetTransform(pMonoBehaviour, strPath, typeof(Canvas)).GetComponent<Canvas>();
	}

	public static CanvasGroup GetTCanvasGroup(this MonoBehaviour pMonoBehaviour, string strPath)
	{
		return GetTransform(pMonoBehaviour, strPath, typeof(CanvasGroup)).GetComponent<CanvasGroup>();
	}

	[Obsolete("Use TText instead Text , Replace to GetTUI<TText>().")]
	public static Text GetTText(this MonoBehaviour pMonoBehaviour, string strPath)
	{
		return GetTransform(pMonoBehaviour, strPath, typeof(Text)).GetComponent<Text>();
	}

	public static Button GetTButton(this MonoBehaviour pMonoBehaviour, string strPath)
	{
		return GetTransform(pMonoBehaviour, strPath, typeof(Button)).GetComponent<Button>();
	}

	public static Image GetTImage(this MonoBehaviour pMonoBehaviour, string strPath)
	{
		return GetTransform(pMonoBehaviour, strPath, typeof(Image)).GetComponent<Image>();
	}

	public static Slider GetTSlider(this MonoBehaviour pMonoBehaviour, string strPath)
	{
		return GetTransform(pMonoBehaviour, strPath, typeof(Slider)).GetComponent<Slider>();
	}

	public static RawImage GetTRawImage(this MonoBehaviour pMonoBehaviour, string strPath)
	{
		return GetTransform(pMonoBehaviour, strPath, typeof(RawImage)).GetComponent<RawImage>();
	}

	public static InputField GetTInputField(this MonoBehaviour pMonoBehaviour, string strPath)
	{
		return GetTransform(pMonoBehaviour, strPath, typeof(InputField)).GetComponent<InputField>();
	}

	public static Toggle GetTToggle(this MonoBehaviour pMonoBehaviour, string strPath)
	{
		return GetTransform(pMonoBehaviour, strPath, typeof(Toggle)).GetComponent<Toggle>();
	}

	public static ToggleGroup GetTToggleGroup(this MonoBehaviour pMonoBehaviour, string strPath)
	{
		return GetTransform(pMonoBehaviour, strPath, typeof(ToggleGroup)).GetComponent<ToggleGroup>();
	}

	public static HorizontalLayoutGroup GetTHorizontalLayoutGroup(this MonoBehaviour pMonoBehaviour, string strPath)
	{
		return GetTransform(pMonoBehaviour, strPath, typeof(HorizontalLayoutGroup)).GetComponent<HorizontalLayoutGroup>();
	}

	public static VerticalLayoutGroup GetTVerticalLayoutGroup(this MonoBehaviour pMonoBehaviour, string strPath)
	{
		return GetTransform(pMonoBehaviour, strPath, typeof(VerticalLayoutGroup)).GetComponent<VerticalLayoutGroup>();
	}

	public static GridLayoutGroup GetTGridLayoutGroup(this MonoBehaviour pMonoBehaviour, string strPath)
	{
		return GetTransform(pMonoBehaviour, strPath, typeof(GridLayoutGroup)).GetComponent<GridLayoutGroup>();
	}

	public static ScrollRect GetTScrollRect(this MonoBehaviour pMonoBehaviour, string strPath)
	{
		return GetTransform(pMonoBehaviour, strPath, typeof(ScrollRect)).GetComponent<ScrollRect>();
	}

	public static Dropdown GetTDropdown(this MonoBehaviour pMonoBehaviour, string strPath)
	{
		return GetTransform(pMonoBehaviour, strPath, typeof(Dropdown)).GetComponent<Dropdown>();
	}
}
