using UnityEngine;
using UnityEngine.SceneManagement;

public class BlockCanvas : MonoBehaviour
{
	private static BlockCanvas g_Instance;

	public static BlockCanvas GetInstance
	{
		get
		{
			return g_Instance;
		}
	}

	public static bool HasInstance
	{
		get
		{
			return g_Instance != null;
		}
	}

	private Canvas m_Canvas;
	private GameObject m_BlockBG;

	private void Awake()
	{
		if (HasInstance)
		{
			Destroy(gameObject);
		}
		else
		{
			g_Instance = this;

			m_Canvas = GetComponent<Canvas>();
			m_BlockBG = this.GetTGameObject("Graphic");

			DontDestroyOnLoad(gameObject);

			SceneManager.sceneLoaded += OnLevelLoaded;
		}
	}

	public void Show(float fTime)
	{
		CancelInvoke();

		Invoke(nameof(Hide), fTime);

		m_BlockBG.SetActive(true);
	}

	public void Hide()
	{
		CancelInvoke();

		m_BlockBG.SetActive(false);
	}

	private void OnLevelLoaded(Scene arg0, LoadSceneMode arg1)
	{
		if (m_Canvas.worldCamera == null && UICamera.HasInstance)
		{
			m_Canvas.worldCamera = UICamera.GetInstance.Camera;
		}
	}
}
