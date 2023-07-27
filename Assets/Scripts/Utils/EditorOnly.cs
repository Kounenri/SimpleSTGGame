#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class EditorOnly : MonoBehaviour
{
	[HideInInspector]
	public string m_Tag = @"Untagged";

	void OnDrawGizmos()
	{
		foreach(GameObject go in GameObject.FindGameObjectsWithTag(m_Tag))
		{
			Handles.Label(go.transform.position,m_Tag);
		}
	}
}

[CustomEditor(typeof(EditorOnly))]
public class EditorOnlyEditor : Editor
{
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();
		EditorOnly gizmos = target as EditorOnly;
		EditorGUI.BeginChangeCheck();
		gizmos.m_Tag = EditorGUILayout.TagField("Tag for Objects:",gizmos.m_Tag);
		if(EditorGUI.EndChangeCheck())
		{
			EditorUtility.SetDirty(gizmos);
		}
	}
}
#endif