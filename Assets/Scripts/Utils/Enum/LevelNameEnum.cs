using UnityEngine.SceneManagement;

public class LevelNameEnum
{
	public const string InitScene = @"InitScene";
	public const string EmptyScene = @"EmptyScene";
	public const string GameScene = @"GameScene";


	public static string ActiveSceneName
	{
		get
		{
			return ActiveScene.name;
		}
	}

	public static Scene ActiveScene
	{
		get
		{
			return SceneManager.GetActiveScene();
		}
	}

	public static bool GetActiveSceneIs(string strName)
	{
		return ActiveSceneName.Equals(strName);
	}
}
