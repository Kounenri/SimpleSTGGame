#define NO_SHOW_LOG

using UnityEngine;

public class TMonoInstance<T> : MonoBehaviour where T : MonoBehaviour
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
	protected static bool g_IsApplicationQuit = false;

	protected bool m_IsInit;

	public static T GetInstance
	{
		get
		{
			if (g_IsApplicationQuit)
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
			return g_Instance != null && !g_IsApplicationQuit;
		}
	}

	protected string InitializeError
	{
		get
		{
			return string.Format(@"TMonoInstance Error - {0},Has not initialized!", typeof(T).Name);
		}
	}

	public bool IsInit
	{
		get
		{
			return m_IsInit;
		}
	}

	public TMonoInstance()
	{
		m_IsInit = false;
	}

	public void Initialize()
	{
#if SHOW_LOG
		Debug.Log(string.Format(@"TMonoInstance Log - {0},Initialize!",typeof(T).Name));
#endif

		if (!m_IsInit)
		{
			OnInitialize();
		}
		else
		{
			Debug.LogWarning(string.Format(@"TMonoInstance Warning - {0},Has already initialized!", typeof(T).Name));
		}
	}

	protected virtual void OnInitialize()
	{
#if SHOW_LOG
		Debug.Log(string.Format(@"TMonoInstance Log - {0},OnInitialize!",typeof(T).Name));
#endif
	}

	protected virtual void Awake()
	{
#if SHOW_LOG
		Debug.Log(string.Format(@"TMonoInstance Log - {0},Awake!",typeof(T).Name));
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
		Debug.Log(string.Format(@"TMonoInstance Log - {0},OnDestroy!",typeof(T).Name));
#endif
		if (HasInstance && g_Instance.GetInstanceID() == GetInstanceID())
		{
			m_IsInit = false;

			g_Instance = null;
		}
	}

	protected virtual void OnApplicationQuit()
	{
		g_IsApplicationQuit = true;
	}

	public void Destroy()
	{
#if SHOW_LOG
		Debug.Log(string.Format(@"TMonoInstance Log - {0},Call Destroy!",typeof(T).Name));
#endif
		Destroy(gameObject);
	}
}
