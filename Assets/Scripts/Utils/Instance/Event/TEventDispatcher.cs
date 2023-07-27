using System.Collections.Generic;

public class TEventDispatcher<T> : TInstance<T> where T : class, IDispatcher
{
	private Dictionary<int, List<EventHandlerFunction>> m_EventHandlerDictionary;

	public TEventDispatcher()
	{
		m_EventHandlerDictionary = new Dictionary<int, List<EventHandlerFunction>>();
	}

	protected override void OnDispose()
	{
		if (m_EventHandlerDictionary != null)
		{
			ClearEvents();

			m_EventHandlerDictionary = null;
		}

		base.OnDispose();
	}

	#region ADD LISTENER'S
	public void AddEventListener(string strEventName, EventHandlerFunction hEventHandlerFunction)
	{
		OnAddEventListener(strEventName.GetHashCode(), hEventHandlerFunction);
	}

	public void AddEventListener(int nEventID, EventHandlerFunction hEventHandlerFunction)
	{
		OnAddEventListener(nEventID, hEventHandlerFunction);
	}

	private void OnAddEventListener(int nEventID, EventHandlerFunction hEventHandlerFunction)
	{
		if (m_EventHandlerDictionary.ContainsKey(nEventID))
		{
			m_EventHandlerDictionary[nEventID].Add(hEventHandlerFunction);
		}
		else
		{
			List<EventHandlerFunction> pHandlerList = new List<EventHandlerFunction>();

			pHandlerList.Add(hEventHandlerFunction);

			m_EventHandlerDictionary.Add(nEventID, pHandlerList);
		}
	}
	#endregion

	#region REMOVE LISTENER'S
	public void RemoveEventListener(string strEventName, EventHandlerFunction hEventHandlerFunction)
	{
		OnRemoveEventListener(strEventName.GetHashCode(), hEventHandlerFunction);
	}

	public void RemoveEventListener(int nEventID, EventHandlerFunction hEventHandlerFunction)
	{
		OnRemoveEventListener(nEventID, hEventHandlerFunction);
	}

	public void RemoveEventListener(string strEventName)
	{
		OnRemoveEventListener(strEventName.GetHashCode());
	}

	public void RemoveEventListener(int nEventID)
	{
		OnRemoveEventListener(nEventID);
	}

	private void OnRemoveEventListener(int nEventID, EventHandlerFunction hEventHandlerFunction)
	{
		if (m_EventHandlerDictionary.ContainsKey(nEventID))
		{
			List<EventHandlerFunction> pHandlerList = m_EventHandlerDictionary[nEventID];

			pHandlerList.Remove(hEventHandlerFunction);

			if (pHandlerList.Count == 0)
			{
				m_EventHandlerDictionary.Remove(nEventID);
			}
		}
	}

	private void OnRemoveEventListener(int nEventID)
	{
		if (m_EventHandlerDictionary.ContainsKey(nEventID))
		{
			List<EventHandlerFunction> pHandlerList = m_EventHandlerDictionary[nEventID];

			pHandlerList.Clear();

			m_EventHandlerDictionary.Remove(nEventID);
		}
	}
	#endregion

	#region DISPATCH
	public void DispatchEvent(string strEventName)
	{
		OnDispatchEvent(strEventName.GetHashCode(), null, strEventName, 0u);
	}

	public void DispatchEvent(string strEventName, uint uTransDex)
	{
		OnDispatchEvent(strEventName.GetHashCode(), null, strEventName, uTransDex);
	}

	public void DispatchEvent(string strEventName, object pData)
	{
		OnDispatchEvent(strEventName.GetHashCode(), pData, strEventName, 0u);
	}

	public void DispatchEvent(string strEventName, object pData, uint uTransDex)
	{
		OnDispatchEvent(strEventName.GetHashCode(), pData, strEventName, uTransDex);
	}

	public void DispatchEvent(int nEventID)
	{
		OnDispatchEvent(nEventID, null, string.Empty, 0u);
	}

	public void DispatchEvent(int nEventID, uint uTransDex)
	{
		OnDispatchEvent(nEventID, null, string.Empty, uTransDex);
	}

	public void DispatchEvent(int nEventID, object pData)
	{
		OnDispatchEvent(nEventID, pData, string.Empty, 0u);
	}

	public void DispatchEvent(int nEventID, object pData, uint uTransDex)
	{
		OnDispatchEvent(nEventID, pData, string.Empty, uTransDex);
	}

	private void OnDispatchEvent(int nEventID, object pData, string strEventName, uint uTransDex)
	{
		DispatchEvent(new TEvent(this, nEventID, strEventName, pData, uTransDex));
	}

	public void DispatchEvent(TEvent pTEvent)
	{
		if (m_EventHandlerDictionary.ContainsKey(pTEvent.ID))
		{
			List<EventHandlerFunction> pHandlerList = CloenList(m_EventHandlerDictionary[pTEvent.ID]);

			foreach (EventHandlerFunction hEventHandlerFunction in pHandlerList)
			{
				if (pTEvent.CanBeDispatched(hEventHandlerFunction.Target))
				{
					hEventHandlerFunction(pTEvent);
				}
			}
		}
	}
	#endregion

	#region PUBLIC METHODS
	public bool HasEventListener(string strEventName)
	{
		return HasEventListener(strEventName.GetHashCode());
	}

	public bool HasEventListener(int nEventID)
	{
		return m_EventHandlerDictionary.ContainsKey(nEventID);
	}

	public void ClearEvents()
	{
		foreach (int nEventID in m_EventHandlerDictionary.Keys)
		{
			List<EventHandlerFunction> pHandlerList = m_EventHandlerDictionary[nEventID];

			pHandlerList.Clear();
		}

		m_EventHandlerDictionary.Clear();
	}

	public void ClearEvents(object pTarget)
	{
		foreach (int nEventID in m_EventHandlerDictionary.Keys)
		{
			List<EventHandlerFunction> pHandlerList = m_EventHandlerDictionary[nEventID];

			for (int i = 0; i < pHandlerList.Count; i++)
			{
				EventHandlerFunction hEventHandlerFunction = pHandlerList[i];

				if (hEventHandlerFunction.Target == pTarget)
				{
					pHandlerList.Remove(hEventHandlerFunction);

					i--;
				}
			}
		}
	}
	#endregion

	#region PRIVATE METHODS
	private List<EventHandlerFunction> CloenList(List<EventHandlerFunction> pSourceHandlerList)
	{
		List<EventHandlerFunction> pHandlerList = new List<EventHandlerFunction>();

		foreach (EventHandlerFunction hEventHandlerFunction in pSourceHandlerList)
		{
			pHandlerList.Add(hEventHandlerFunction);
		}

		return pHandlerList;
	}
	#endregion
}

