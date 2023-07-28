using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelController : TMonoEventDispatcher<LevelController>, IDispatcher
{
	public const string ON_PLAYER_HP_CHANGED = "ON_PLAYER_HP_CHANGED";

	protected override void Awake()
	{
		base.Awake();

		SceneManager.sceneLoaded += OnLoadCallback;
	}

	private void OnLoadCallback(Scene pScene, LoadSceneMode pLoadSceneMode)
	{
		if (pScene.name == LevelNameEnum.GameScene)
		{
			StartCoroutine(InitializeLevel());
		}
	}

	private IEnumerator InitializeLevel()
	{
		yield return new WaitForEndOfFrame();

		Debug.Log("Begin Initialize Level.");

		yield return null;
	}

	public void OnPlayerHPChanged(float fPlayerHP)
	{
		if (fPlayerHP < 0f) fPlayerHP = 0f;

		DispatchEvent(ON_PLAYER_HP_CHANGED, fPlayerHP);
	}
}
