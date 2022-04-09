using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using BoringWorld.UI.CombatForm;

[CustomEditor(typeof(SliderBar))]
public class SliderBarInspector : Editor
{
    private SerializedProperty m_AnimTime;
    private SerializedProperty m_AnimTimeInterval;
    private SerializedProperty m_AnimCurve;
    private SerializedProperty m_ColorGradient;
    private SerializedProperty m_EnableAnim;
    private SerializedProperty m_EnableGradient;
    private SliderBar m_TargetObj;

    private void OnEnable()
    {
        m_AnimTime = serializedObject.FindProperty("m_AnimTime");
        m_AnimTimeInterval = serializedObject.FindProperty("m_AnimTimeInterval");
        m_AnimCurve = serializedObject.FindProperty("m_AnimCurve");
        m_ColorGradient = serializedObject.FindProperty("m_ColorGradient");
        m_EnableAnim = serializedObject.FindProperty("m_EnableAnim");
        m_EnableGradient = serializedObject.FindProperty("m_EnableGradient");
        m_TargetObj = target as SliderBar;
    }

    public override void OnInspectorGUI()
    {
        m_EnableAnim.boolValue = EditorGUILayout.BeginToggleGroup("动画", m_EnableAnim.boolValue);
        {
            m_AnimTime.floatValue = EditorGUILayout.FloatField(m_AnimTime.displayName, m_AnimTime.floatValue);
            m_AnimTimeInterval.floatValue = EditorGUILayout.Slider(m_AnimTimeInterval.displayName, m_AnimTimeInterval.floatValue, 0.02f, m_AnimTime.floatValue);
            m_AnimCurve.animationCurveValue = EditorGUILayout.CurveField(m_AnimCurve.animationCurveValue);
        }
        EditorGUILayout.EndToggleGroup();
        m_EnableGradient.boolValue = EditorGUILayout.BeginToggleGroup("颜色渐变", m_EnableGradient.boolValue);
        {
            EditorGUILayout.PropertyField(m_ColorGradient, GUIContent.none);
        }
        EditorGUILayout.EndToggleGroup();
        if (EditorApplication.isPlaying)
        {
            m_TargetObj.EnableAnim = m_EnableAnim.boolValue;
            m_TargetObj.EnableGradient = m_EnableGradient.boolValue;
        }
        serializedObject.ApplyModifiedProperties();
    }
}
