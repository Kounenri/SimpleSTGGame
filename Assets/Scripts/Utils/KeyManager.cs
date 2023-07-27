using System.Collections.Generic;
using UnityEngine;

public class KeyManager : MonoBehaviour
{
	private List<GameObject> m_ViewOrders = new List<GameObject>();

	public List<GameObject> ViewOrders
	{
		get
		{
			return m_ViewOrders;
		}
	}

	void Update()
	{
		if (Input.GetKeyUp(KeyCode.Escape) == false) return;

		if (m_ViewOrders.Count > 0)
		{
			GameObject pGameObject = m_ViewOrders[m_ViewOrders.Count - 1];

			if (pGameObject != null)
			{
				BaseCanvas pBaseCanvas = pGameObject.GetComponent<BaseCanvas>();

				if (pBaseCanvas != null && pBaseCanvas.isActiveAndEnabled && pBaseCanvas.destroyWhenBackKey)
				{
					pBaseCanvas.DestroyCanvas();
				}
			}
		}
	}

	private void OnQuitGame(object pParam)
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			Application.Quit();
		}

#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
#endif
	}

	public void AddView(GameObject pView)
	{
		if (m_ViewOrders != null)
		{
			RemoveView(pView);

			m_ViewOrders.Add(pView);
		}
	}

	public void RemoveView(GameObject pView)
	{
		m_ViewOrders?.Remove(pView);
	}

	void OnDestroy()
	{
		m_ViewOrders = null;
	}
}
