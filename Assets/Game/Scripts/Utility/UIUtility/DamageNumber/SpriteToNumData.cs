using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FantasyDiary
{
	public class SpriteToNumData : ScriptableObject
	{
	    public Material SourceMat;
	    public FontAndBorderAndIndex[] FontInfos;
	}
	
	[System.Serializable]
	public class FontAndBorderAndIndex
	{
	    public int Index;
	    public char Font;
	    public Vector4 Border;
}}