using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class TInstance<T> : IDisposable where T : class
{
	/// <summary>
	/// Static Handle
	/// </summary>
	protected static T g_Instance;
	/// <summary>
	/// Lock
	/// </summary>
	protected static readonly object g_InstanceLock = new object();

	protected bool m_IsDisposed;
	protected bool m_IsInit;

	public static T GetInstance
	{
		get
		{
			if (g_Instance == null)
			{
				if (Application.isPlaying)
				{
					lock (g_InstanceLock)
					{
						g_Instance ??= Activator.CreateInstance<T>();
					}
				}
			}
			return g_Instance;
		}
	}

	public static bool HasInstance
	{
		get
		{
			return g_Instance != null ? true : false;
		}
	}

	protected string InitializeError
	{
		get
		{
			return string.Format(@"TInstance Error - {0},Has not initialized!", typeof(T).Name);
		}
	}

	public bool IsInit
	{
		get
		{
			return m_IsInit;
		}
	}

	public bool IsDisposed
	{
		get
		{
			return m_IsDisposed;
		}
	}

	public TInstance()
	{
		m_IsDisposed = false;
		m_IsInit = false;

#if UNITY_EDITOR
		EditorApplication.playModeStateChanged += HandlePlayModeStateChanged;
#endif
	}

	~TInstance()
	{
		Dispose(false);
	}

	public void Initialize()
	{
		if (!m_IsInit)
		{
			OnInitialize();
		}
		else
		{
			Debug.LogWarning(string.Format(@"TInstance Warning - {0},Has already initialized!", typeof(T).Name));
		}
	}

	protected virtual void OnInitialize()
	{
	}

	public void Dispose()
	{
		Dispose(true);

		GC.SuppressFinalize(this);
	}

	private void Dispose(bool bIsDispos)
	{
		if (!m_IsInit)
		{
			Debug.LogWarning(string.Format(@"TInstance Warning - {0},Has not initialized but now request dispose!", typeof(T).Name));
		}

		if (!m_IsDisposed)
		{
			if (bIsDispos)
			{
				// Free any other managed objects here.
				OnDispose();
			}

			// Free any unmanaged objects here.
			g_Instance = null;

			m_IsDisposed = true;
		}
		else
		{
			Debug.LogWarning(string.Format(@"TInstance Warning - {0},Has already disposed!", typeof(T).Name));
		}
	}

	protected virtual void OnDispose()
	{
	}

#if UNITY_EDITOR
	private void HandlePlayModeStateChanged(PlayModeStateChange pState)
	{
		// This method is run whenever the playmode state is changed.
		if (pState == PlayModeStateChange.ExitingPlayMode || pState == PlayModeStateChange.EnteredEditMode)
		{
			EditorApplication.playModeStateChanged -= HandlePlayModeStateChanged;

			// Do stuff when the editor is stop or compiling.
			if (HasInstance && m_IsInit && !m_IsDisposed)
			{
				EditorApplication.delayCall += Dispose;
			}
		}
	}
#endif
}
