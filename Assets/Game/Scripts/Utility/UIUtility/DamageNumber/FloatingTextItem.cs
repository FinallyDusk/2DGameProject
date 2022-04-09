using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace FantasyDiary
{
	[DisallowMultipleComponent, RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
	public class FloatingTextItem : MonoBehaviour
	{
#pragma warning disable 0649
		[SerializeField] private SpriteToNumData m_SourceData;
#pragma warning restore 0649
		[SerializeField] private string m_Text;
	    private MeshRenderer m_Renderer;
		[SerializeField] private Color m_FontColor;
		private bool m_ColorDirty;
		public Color FontColor
        {
            get
            {
				return m_FontColor;
            }
            set
            {
				if (m_FontColor.Equals(value)) return;
				m_ColorDirty = true;
				m_FontColor = value;
            }
        }

	
	    public string Text
	    {
	        get
	        {
	            return m_Text;
	        }
	        set
	        {
	            m_Renderer.material.SetFloatArray("_ShowNum", GetIntArray(value));
				if (m_ColorDirty)
                {
					m_ColorDirty = false;
					m_Renderer.material.SetColor("_FontColor", m_FontColor);
                }
	            m_Text = value;
	        }
	    }
	
	    private void Awake()
	    {
	        m_Renderer = GetComponent<MeshRenderer>();
	        m_Renderer.material = new Material(m_SourceData.SourceMat);
	        List<Vector4> v4s = new List<Vector4>();
	        for (int i = 0; i < m_SourceData.FontInfos.Length; i++)
	        {
	            v4s.Add(m_SourceData.FontInfos[i].Border);
	        }
	        m_Renderer.material.SetVectorArray("_NumBorder", v4s);
	        m_Renderer.material.SetFloatArray("_ShowNum", GetIntArray(m_Text));
			m_ColorDirty = false;
			m_Renderer.material.SetColor("_FontColor", m_FontColor);
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
}}