using UnityEngine;
using UnityEngine.SceneManagement;

public class GoInit : MonoBehaviour
{
#if UNITY_EDITOR
	private bool m_IsCallOnAwake = false;

	void Awake()
	{
		m_IsCallOnAwake = true;

		Go();
	}

	void OnEnable()
	{
		if (!m_IsCallOnAwake)
		{
			Go();
		}
	}

	private void Go()
	{
		if (!LevelController.HasInstance)
		{
			SceneManager.LoadScene(LevelNameEnum.InitScene);
		}
	}
#endif
}
