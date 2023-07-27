using UnityEngine;
using UnityEngine.SceneManagement;

internal class StandaloneResolutionUtil : TMonoInstance<StandaloneResolutionUtil>
{
	private Vector2 m_Resolution;

	protected override void Start()
	{
		base.Start();

		m_Resolution = new Vector2(Screen.width, Screen.height);
	}

	private void Update()
	{
		if (m_Resolution.x != Screen.width || m_Resolution.y != Screen.height)
		{
			m_Resolution.x = Screen.width;
			m_Resolution.y = Screen.height;

			if (IsInvoking(nameof(OnResolutionChange))) CancelInvoke(nameof(OnResolutionChange));

			Invoke(nameof(OnResolutionChange), 0.5f);
		}
	}

	private void OnResolutionChange()
	{
		Scene pScene = SceneManager.GetActiveScene();

		if (pScene.name == LevelNameEnum.GameScene)
		{
			SceneManager.LoadScene(pScene.name, LoadSceneMode.Single);
		}
	}
}
