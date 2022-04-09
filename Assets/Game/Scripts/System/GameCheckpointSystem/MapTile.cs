using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.IO;
using UnityEngine.EventSystems;

namespace BoringWorld
{
	public class MapTile : Tile
	{
	    public TileTag Tag;


		/// <summary>
		/// 初始化数据
		/// </summary>
		public void InitData(TileTag tag)
        {
			Tag = tag;
        }
    }
	
	public enum TileTag
	{
	    /// <summary>
	    /// 普通
	    /// </summary>
	    Normal,
		/// <summary>
		/// 怪
		/// </summary>
		Enemy,
		/// <summary>
		/// 物品
		/// </summary>
		Item,
		/// <summary>
		/// 墙
		/// </summary>
		Wall,
	}
}