using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(-14998)]
public class InitApplication : MonoBehaviour
{
	private static float g_StartTime = 0f;
	private static string g_DeviceModel;

	public static float StartTime
	{
		get
		{
			return g_StartTime;
		}
	}

	public static string DeviceModel
	{
		get
		{
			return g_DeviceModel;
		}
	}

	private void Awake()
	{
		PlayerPrefs.SetInt(PlayerPrefNames.APP_START_COUNT, PlayerPrefs.GetInt(PlayerPrefNames.APP_START_COUNT, 0) + 1);

		g_StartTime = Time.realtimeSinceStartup;
		g_DeviceModel = SystemInfo.deviceModel;

		Screen.sleepTimeout = SleepTimeout.NeverSleep;

#if !UNITY_EDITOR
		if(PlayerPrefs.GetInt(PlayerPrefNames.Disable_Log,1) == 1)
		{
			Debug.unityLogger.filterLogType = LogType.Error;
		}
		else
		{
			Instantiate(Resources.Load<GameObject>("Reporter"));

			Debug.unityLogger.filterLogType = LogType.Log;
		}
#else
		Debug.unityLogger.filterLogType = LogType.Log;
#endif
	}

	private IEnumerator Start()
	{
		// Do some stuf
		yield return new WaitForEndOfFrame();

#if !UNITY_EDITOR
#if UNITY_IOS
		UnityEngine.iOS.Device.SetNoBackupFlag(Application.persistentDataPath);

		OnInitialize();
#elif UNITY_ANDROID
		//AndroidPermissionUtil.Create(AN_Permission.WRITE_EXTERNAL_STORAGE,pParam => { OnInitialize(); },true);
#else
		OnInitialize();
#endif
#else
		OnInitialize();
#endif
	}

	private void OnInitialize()
	{
		ViewManager.GetInstance.Initialize();

		LevelController.GetInstance.Initialize();

		// For iPhone notch
		//ScreenEdges.Create();

		SceneManager.LoadScene(LevelNameEnum.GameScene, LoadSceneMode.Single);
	}
}
