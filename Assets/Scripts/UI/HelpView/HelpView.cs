using UnityEngine;

public class HelpView : BaseCanvas
{
	public static HelpView Create()
	{
		GameObject pPrefab = Resources.Load<GameObject>("UI/HelpView/HelpView");

		GameObject pInstance = Instantiate(pPrefab);
		HelpView pHelpView = pInstance.GetComponent<HelpView>();

		return pHelpView;
	}

	public void OnCloseBtnClick()
	{
		LevelManager.GetInstance.OnLevelLoaded();

		Destroy(gameObject);
	}
}
