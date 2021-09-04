using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(CryptoPlayerPrefsManager))]
public class CryptoPlayerPrefsManagerEditor : Editor
{
	public override void OnInspectorGUI()
	{
		EditorGUILayout.HelpBox("Once these settings are set once, no changes should be made to these.", MessageType.Warning, true);
		
		DrawDefaultInspector ();
		
		EditorGUILayout.HelpBox("If you experiment with the settings (not productive) and cryptographic exceptions occur, then you have to clear the PlayerPrefs (because new keys are generated).", MessageType.Info, true);
		
		if (GUILayout.Button("Clear PlayerPrefs"))
        {
            CryptoPlayerPrefs.DeleteAll();
        }
	}
}

