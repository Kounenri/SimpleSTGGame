using UnityEngine;

public class InputUtility
{
	private static bool m_MarkInput;

	public static bool IsSingleTouch
	{
		get
		{
			if (Input.touchCount < 2) return true;

			if (Input.touchCount == 2)
			{
				m_MarkInput = !m_MarkInput;
				return m_MarkInput;
			}

			return false;
		}
	}
}