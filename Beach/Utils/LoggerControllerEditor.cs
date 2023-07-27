using UnityEditor;
using UnityEngine;

// Place in Script folder

[CustomEditor(typeof(LoggerController))]
public class LoggerControllerEditor : Editor
{
    private SerializedProperty logEnabledProp;

    private void OnEnable()
    {
        logEnabledProp = serializedObject.FindProperty("logEnabled");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(logEnabledProp);

        serializedObject.ApplyModifiedProperties();

        Debug.unityLogger.logEnabled = logEnabledProp.boolValue;
    }
}