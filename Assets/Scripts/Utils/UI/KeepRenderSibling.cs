using UnityEngine;

public class KeepRenderSibling : MonoBehaviour
{
	[SerializeField]
	private bool m_KeepInFirst = true;

	public bool KeepInFirst
	{
		get
		{
			return m_KeepInFirst;
		}
	}
}
