using UnityEngine;

[ExecuteInEditMode]
public class PointerTransition : MonoBehaviour
{
	void Awake()
	{
		if(Application.isPlaying)
		{
			Destroy(this);
		}
		else
		{
			DestroyImmediate(this);
		}
	}
}
