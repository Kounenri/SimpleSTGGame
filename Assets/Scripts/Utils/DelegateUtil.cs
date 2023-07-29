using System;

internal static class DelegateUtil
{
	internal static void SafeCall(this Delegate @delegate, params object[] args)
	{
		if (@delegate != null)
		{
			Delegate[] delegates = @delegate.GetInvocationList();

			for (int i = 0; i < delegates.Length; i++)
			{
				if (delegates[i] != null && delegates[i].Target != null)
				{
					delegates[i].DynamicInvoke(args);
				}
			}
		}
	}
}
