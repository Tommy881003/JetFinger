using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ChartData))]
public class ChartDataEditor : Editor
{
    private TrackID id;
    private float beat;
    private float gap;
    private int consecutive;

    private ChartData inspecting = null;

    private void OnEnable()
    {
        inspecting = (ChartData)target;
    }

    private void OnDisable()
    {
        id = TrackID.Snare;
        beat = 0;
        gap = 0;
        consecutive = 0;
        inspecting = null;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        base.OnInspectorGUI();
        EditorGUILayout.Space(10);

        id = (TrackID)EditorGUILayout.EnumPopup("鼓類：", id);
        beat = EditorGUILayout.FloatField("拍點：", beat);

        if (GUILayout.Button("加入拍點"))
            inspecting.AddNote(id, beat);

        EditorGUILayout.Space(10);

        gap = EditorGUILayout.FloatField("拍點間隔：", gap);
        consecutive = EditorGUILayout.IntField("持續數量：", consecutive);

        if (GUILayout.Button("加入連續拍點"))
            inspecting.AddConsecutiveNotes(id, beat, gap, consecutive);

        EditorGUILayout.Space(10);

        if (GUILayout.Button("清空譜面"))
            inspecting.ClearAll();

        serializedObject.ApplyModifiedProperties();
        EditorUtility.SetDirty(target);
    }
}
