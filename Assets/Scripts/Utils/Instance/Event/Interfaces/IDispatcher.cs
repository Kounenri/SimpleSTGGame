public delegate void EventHandlerFunction(TEvent pTEvent);

public interface IDispatcher
{
	#region ADD LISTENER'S
	void AddEventListener(string strEventName, EventHandlerFunction hEventHandlerFunction);
	void AddEventListener(int nEventID, EventHandlerFunction hEventHandlerFunction);
	#endregion

	#region REMOVE LISTENER'S
	void RemoveEventListener(string strEventName, EventHandlerFunction hEventHandlerFunction);
	void RemoveEventListener(int nEventID, EventHandlerFunction hEventHandlerFunction);
	#endregion

	#region DISPATCH
	void DispatchEvent(int nEventID);
	void DispatchEvent(int nEventID, object pData);
	void DispatchEvent(string strEventName);
	void DispatchEvent(string strEventName, object pData);
	#endregion

	#region METHODS
	bool HasEventListener(string strEventName);
	bool HasEventListener(int nEventID);
	void ClearEvents();
	void ClearEvents(object pTarget);
	#endregion
}
