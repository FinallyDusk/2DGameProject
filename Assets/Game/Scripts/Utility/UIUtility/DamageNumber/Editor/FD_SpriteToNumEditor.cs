using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using FantasyDiary;

[CustomEditor(typeof(FloatingTextItem))]
public class FD_SpriteToNumEditor : Editor
{
    private SerializedProperty m_SourceDataProp;
    private SerializedProperty m_TextProp;
    private SerializedProperty m_FontColorProp;
    private MeshRenderer m_MeshRender;
    private SpriteToNumData m_SourceData;

    private void OnEnable()
    {
        m_SourceDataProp = serializedObject.FindProperty("m_SourceData");
        m_TextProp = serializedObject.FindProperty("m_Text");
        m_FontColorProp = serializedObject.FindProperty("m_FontColor");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        ParseStringToIntAndSetMat();
        serializedObject.ApplyModifiedProperties();
    }

    private void ParseStringToIntAndSetMat()
    {
        m_SourceData = m_SourceDataProp.objectReferenceValue as SpriteToNumData;
        if (m_SourceData == null)
        {
            return;
        }
        if (m_MeshRender == null)
        {
            m_MeshRender = (target as FloatingTextItem).GetComponent<MeshRenderer>();
            m_MeshRender.material = m_SourceData.SourceMat;
            List<Vector4> v4s = new List<Vector4>();
            MaterialPropertyBlock block = new MaterialPropertyBlock();
            m_MeshRender.GetPropertyBlock(block);
            for (int i = 0; i < m_SourceData.FontInfos.Length; i++)
            {
                v4s.Add(m_SourceData.FontInfos[i].Border);
            }
            block.SetVectorArray("_NumBorder", v4s);
            block.SetFloatArray("_ShowNum", GetIntArray(m_TextProp.stringValue));
            block.SetColor("_FontColor", m_FontColorProp.colorValue);
            m_MeshRender.SetPropertyBlock(block);
        }
        else
        {
            MaterialPropertyBlock block = new MaterialPropertyBlock();
            m_MeshRender.GetPropertyBlock(block);
            block.SetFloatArray("_ShowNum", GetIntArray(m_TextProp.stringValue));
            block.SetColor("_FontColor", m_FontColorProp.colorValue);
            m_MeshRender.SetPropertyBlock(block);
        }
    }

    private List<float> GetIntArray(string value)
    {
        List<float> result = new List<float>(20);
        int index = value.Length - 1;
        while (index >= 0)
        {
            result.Add(GetIndexByChar(value[index]));
            index--;
        }
        for (int i = result.Count; i < 20; i++)
        {
            result.Add(-1);
        }
        return result;
    }

    private int GetIndexByChar(char c)
    {
        for (int i = 0; i < m_SourceData.FontInfos.Length; i++)
        {
            if (m_SourceData.FontInfos[i].Font == c) return i;
        }
        return -1;
    }

}