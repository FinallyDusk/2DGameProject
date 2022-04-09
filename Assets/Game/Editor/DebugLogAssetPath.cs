using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public static class DebugLogAssetPath
{
    [MenuItem("Assets/BoringWorldUtil/DebugAssetPath")]
	public static void LogAssetPath()
    {
        if (Selection.activeObject == null) return;
        Debug.Log(AssetDatabase.GetAssetPath(Selection.activeObject));
    }
}
