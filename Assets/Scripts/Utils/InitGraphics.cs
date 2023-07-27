using UnityEngine;

[DefaultExecutionOrder(-14997)]
public class InitGraphics : MonoBehaviour
{
	private void Awake()
	{
		QualitySettings.vSyncCount = 0;

		Application.targetFrameRate = PlayerPrefs.GetInt(PlayerPrefNames.GAME_FPS, GameConfig.DEFAULT_FPS);

		SetQualityLevel();

#if !UNITY_EDITOR && (UNITY_STANDALONE || UNITY_WSA)
		StandaloneResolutionUtil.GetInstance.Initialize();
#endif
	}

	private void SetQualityLevel()
	{
		if (PlayerPrefs.HasKey(PlayerPrefNames.QUALITY_LEVEL))
		{
			QualitySettings.SetQualityLevel(PlayerPrefs.GetInt(PlayerPrefNames.QUALITY_LEVEL), true);
		}
		else
		{
			PlayerPrefs.SetInt(PlayerPrefNames.QUALITY_LEVEL, QualitySettings.GetQualityLevel());
		}
	}

	/*private void SetQualityLevel()
	{
		string strOperatingSystem = SystemInfo.operatingSystem;

		if(PlayerPrefs.HasKey(PlayerPrefabNames.Quality_Level))
		{
			QualitySettings.SetQualityLevel(PlayerPrefs.GetInt(PlayerPrefabNames.Quality_Level),true);

			return;
		}
		else if(!string.IsNullOrEmpty(strOperatingSystem))
		{
			try
			{
				if(Application.platform == RuntimePlatform.Android)
				{
					Version pVersion = new Version(strOperatingSystem.Split(' ')[2]);

					if(pVersion >= new Version(5,0))
					{
						SaveQualityLevel(2);
					}
					else
					{
						SaveQualityLevel(1);
					}
				}
			}
			catch(Exception pException)
			{
				Debug.LogException(pException);
			}
		}

		PlayerPrefs.SetInt(PlayerPrefabNames.Quality_Level,QualitySettings.GetQualityLevel());
	}

	private void SaveQualityLevel(int nLevel)
	{
		QualitySettings.SetQualityLevel(nLevel);

		PlayerPrefs.SetInt(PlayerPrefabNames.Quality_Level,nLevel);
	}*/
}
