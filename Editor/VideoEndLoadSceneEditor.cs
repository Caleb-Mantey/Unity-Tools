// Create a custom inspector to conditionally show fields based on the selected reference type
using UnityEngine;
using UnityEditor;
using Relu.Utils;

[CustomEditor(typeof(VideoEndLoadScene))]
public class VideoEndLoadSceneEditor : Editor {

    SerializedProperty loadOnVideoEndProp;
    SerializedProperty referenceTypeProp;
    SerializedProperty sceneBuildIndexProp;
    SerializedProperty sceneNameProp;
    SerializedProperty debugLogProp;

    void OnEnable() {
        loadOnVideoEndProp = serializedObject.FindProperty("loadOnVideoEnd");
        referenceTypeProp = serializedObject.FindProperty("referenceType");
        sceneBuildIndexProp = serializedObject.FindProperty("sceneBuildIndex");
        sceneNameProp = serializedObject.FindProperty("sceneName");
        debugLogProp = serializedObject.FindProperty("debugLog");
    }

    public override void OnInspectorGUI() {
        serializedObject.Update();

        EditorGUILayout.PropertyField(loadOnVideoEndProp);
        EditorGUILayout.PropertyField(referenceTypeProp);

        // Show only the relevant scene reference field
        var referenceType = (VideoEndLoadScene.SceneReferenceType)referenceTypeProp.enumValueIndex;
        if (referenceType == VideoEndLoadScene.SceneReferenceType.ByBuildIndex) {
            EditorGUILayout.PropertyField(sceneBuildIndexProp);
        } else {
            EditorGUILayout.PropertyField(sceneNameProp);
        }

        EditorGUILayout.PropertyField(debugLogProp);

        serializedObject.ApplyModifiedProperties();
    }
}

