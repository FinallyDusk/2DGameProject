using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AStarUtil;
using UnityEngine.Tilemaps;

namespace BoringWorld
{
    /// <summary>
    /// 关卡地图逻辑
    /// </summary>
    public class CheckpointMapLogic :MonoBehaviour, IAstarHelper
    {
        private Tilemap m_Map;
        private Tilemap m_AdornMap;

        private void Awake()
        {
            m_Map = this.GetComponentWithName<Tilemap>("Map");
            m_AdornMap = this.GetComponentWithName<Tilemap>("Adorn");
        }

        public bool IsAdornTile(Vector3Int pos)
        {
            return m_AdornMap.HasTile(pos);
        }

        public Vector3Int[] GetAllMapTilePos()
        {
            return InternaleGetAllTilePos(m_Map);
        }

        public Vector3Int[] GetAllAdornMapTilePos()
        {
            return InternaleGetAllTilePos(m_AdornMap);
        }

        private Vector3Int[] InternaleGetAllTilePos(Tilemap map)
        {
            int count = map.GetUsedTilesCount();
            Vector3Int[] result = new Vector3Int[count];
            TileBase[] allTiles = new TileBase[count];
            map.GetUsedTilesNonAlloc(allTiles);
            for (int i = 0; i < count; i++)
            {
                var localPos = (allTiles[i] as Tile).gameObject.transform.localPosition;
                Vector3Int pos = new Vector3Int((int)localPos.x, (int)localPos.y, 0);
                result[i] = pos;
            }
            return result;
        }

        public bool PositionIsValid(Vector3Int pos, bool ingoreUnit)
        {
            var tile = m_AdornMap.GetTile(pos);
            if (tile == null)
            {
                return true;
            }
            //todo
            //此处还需要判断是否有敌人，物品之类的
            return false;
        }

        public bool EndPathForward(Vector3Int pos)
        {
            return false;
        }
    }
}