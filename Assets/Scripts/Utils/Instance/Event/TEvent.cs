public class TEvent
{
	private object m_Target;
	private int m_ID;
	private string m_Name;
	private object m_Data;
	private uint m_TransDex;

	private bool m_IsStopped = false;
	private bool m_IsLocked = false;

	#region GET / SET
	public int ID
	{
		get
		{
			return m_ID;
		}
	}

	public string Name
	{
		get
		{
			return m_Name;
		}
	}

	public object Data
	{
		get
		{
			return m_Data;
		}
	}

	public uint TransDex
	{
		get
		{
			return m_TransDex;
		}
	}

	public object Target
	{
		get
		{
			return m_Target;
		}
	}

	public bool IsStopped
	{
		get
		{
			return m_IsStopped;
		}
	}

	public bool IsLocked
	{
		get
		{
			return m_IsLocked;
		}
	}
	#endregion

	#region INITIALIZE
	public TEvent(object pTarget, string strName)
	{
		m_Target = pTarget;
		m_ID = strName.GetHashCode();
		m_Name = strName;
	}

	public TEvent(object pTarget, string strName, object pData)
	{
		m_Target = pTarget;
		m_ID = strName.GetHashCode();
		m_Name = strName;
		m_Data = pData;
	}

	public TEvent(object pTarget, string strName, object pData, uint uTransDex)
	{
		m_Target = pTarget;
		m_ID = strName.GetHashCode();
		m_Name = strName;
		m_Data = pData;
		m_TransDex = uTransDex;
	}

	public TEvent(object pTarget, int nID, string strName, object pData, uint uTransDex)
	{
		m_Target = pTarget;
		m_ID = nID;
		m_Name = strName;
		m_Data = pData;
		m_TransDex = uTransDex;
	}
	#endregion

	#region PUBLIC METHODS
	public void StopPropagation()
	{
		m_IsStopped = true;
	}

	public void StopImmediatePropagation()
	{
		m_IsStopped = true;
		m_IsLocked = true;
	}

	public bool CanBeDispatched(object pValue)
	{
		if (m_IsLocked)
		{
			return false;
		}

		if (m_IsStopped)
		{
			return (m_Target == pValue);
		}
		else
		{
			m_Target = pValue;

			return true;
		}
	}
	#endregion
}
