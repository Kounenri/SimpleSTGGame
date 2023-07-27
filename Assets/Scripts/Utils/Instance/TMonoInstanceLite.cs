#define NO_SHOW_LOG

using UnityEngine;

public class TMonoInstanceLite<T> : MonoBehaviour where T : MonoBehaviour
{
	/// <summary>
	/// Static Handle
	/// </summary>
	protected static T g_Instance;
	/// <summary>
	/// Lock
	/// </summary>
	protected static readonly object g_InstanceLock = new object();
	/// <summary>
	/// Application already quit
	/// </summary>
	protected static bool g_ApplicationIsQuitting = false;

	public static T GetInstance
	{
		get
		{
			if (g_ApplicationIsQuitting)
			{
				Debug.LogWarning(typeof(T) + @" is already destroyed. Returning null. Please check HasInstance first before accessing instance in destructor.");

				return null;
			}

			if (g_Instance == null)
			{
				lock (g_InstanceLock)
				{
					g_Instance = FindObjectOfType(typeof(T)) as T;

					g_Instance ??= new GameObject(typeof(T).Name).AddComponent<T>();
				}
			}
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

	protected virtual void Awake()
	{
#if SHOW_LOG
		Debug.Log(string.Format(@"TMonoInstanceLite Log - {0},Awake!",typeof(T).Name));
#endif
		DontDestroyOnLoad(gameObject);
	}

	protected virtual void OnEnable()
	{
	}

	protected virtual void OnDisable()
	{
	}

	protected virtual void Start()
	{
	}

	protected virtual void OnDestroy()
	{
#if SHOW_LOG
		Debug.Log(string.Format(@"TMonoInstanceLite Log - {0},OnDestroy!",typeof(T).Name));
#endif

		if (HasInstance && g_Instance.GetInstanceID() == GetInstanceID())
		{
			g_Instance = null;
		}
	}

	protected virtual void OnApplicationQuit()
	{
		g_ApplicationIsQuitting = true;

		g_Instance = null;

		Destroy(gameObject);
	}

	public void Destroy()
	{
#if SHOW_LOG
		Debug.Log(string.Format(@"TMonoInstanceLite Log - {0},Call Destroy!",typeof(T).Name));
#endif
		g_Instance = null;

		Destroy(gameObject);
	}
}
