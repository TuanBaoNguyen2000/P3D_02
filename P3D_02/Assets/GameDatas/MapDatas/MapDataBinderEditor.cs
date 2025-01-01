using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MapDataBinder))]
public class MapDataBinderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        GUILayout.Space(10);

        MapDataBinder MapDataBinder = (MapDataBinder)target;

        if (GUILayout.Button("Bind Data To ScriptableObject"))
        {
            if (MapDataBinder.MapDataSO != null)
                MapDataBinder.BindDataToScriptableObject();
            else
                Debug.LogError("MapDataSO ScriptableObject is missing!");
        }

    }
}
