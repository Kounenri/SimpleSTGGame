#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using UnityEngine;

public class TInstanceLite<T> : IDisposable where T : class
{
	/// <summary>
	/// Static Handle
	/// </summary>
	protected static T g_Instance;
	/// <summary>
	/// Lock
	/// </summary>
	protected static readonly object g_InstanceLock = new object();

	public static T GetInstance
	{
		get
		{
			if (g_Instance == null)
			{
				lock (g_InstanceLock)
				{
					g_Instance ??= Activator.CreateInstance<T>();
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

	protected bool m_IsDisposed = false;

	public bool IsDisposed
	{
		get
		{
			return m_IsDisposed;
		}
	}

	public TInstanceLite()
	{
#if UNITY_EDITOR
		EditorApplication.playModeStateChanged += HandlePlayModeStateChanged;
#endif
	}

	~TInstanceLite()
	{
		Dispose(false);
	}

	public void Dispose()
	{
		Dispose(true);

		GC.SuppressFinalize(this);
	}

	private void Dispose(bool bIsDispos)
	{
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
			Debug.LogWarning(string.Format(@"TInstanceLite Warning - {0},Has already disposed!", typeof(T).Name));
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
			if (HasInstance && !m_IsDisposed)
			{
				EditorApplication.delayCall += Dispose;
			}
		}
	}
#endif
}
