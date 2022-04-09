using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BoringWorld
{
	public static class UnityUtility
	{
	    private static Vector2 m_LocalToCanvasLocalScale = Vector2.zero;
	
	    /// <summary>
	    /// 计算UI物体坐标（RectTransform.localPosition）
	    /// </summary>
	    /// <param name="canvasSizeDelta">canvas的大小（RectTransform.sizeDelta）</param>
	    /// <param name="targetPosition">目标位置（RectTransform.position）</param>
	    /// <returns></returns>
	    public static Vector2 CalcLocalPositionForCanvas(Vector2 canvasSizeDelta, Vector2 targetPosition)
	    {
	        // 1. canvas为overlay模式（其他模式没有尝试过）
	        // 2. 通过屏幕的分辨率与Canvas的sizeDelta进行除法，获得UI的RectTransform.position被缩放了多少
	        // （即 canvas.sizeDelta / Screen = UI.RectTransform.localPosition / UI.RectTransform.Position）
	        // 3. 通过2求出的UI.RectTransform.localPosition为以左下角为（0,0）的坐标，而canvas在overlay下坐标原点在中间位置
	        //  所以需要将UI.RectTransform.localPosition - canvas.sizeDelta / 2f
	        // 4. 最后得到的UI.RectTransform.localPosition即为正常的localPosition
	        //if (m_LocalToCanvasLocalScale == Vector2.zero)
	        //{
	        //    m_LocalToCanvasLocalScale = new Vector2(canvasSizeDelta.x / Screen.width, canvasSizeDelta.y / Screen.height);
	        //}
			m_LocalToCanvasLocalScale = new Vector2(canvasSizeDelta.x / Screen.width, canvasSizeDelta.y / Screen.height);
			Vector2 scalePos = targetPosition * m_LocalToCanvasLocalScale;
	        return scalePos - canvasSizeDelta / 2f;
	    }

		public static Vector3Int V3ILerp(Vector3Int v3i, Vector3Int target, double t)
		{
			var offset = target - v3i;
			return v3i + new Vector3Int((int)(offset.x * t), (int)(offset.y * t), (int)(offset.z * t));
		}
	}

	///// <summary>
	///// 集合池
	///// </summary>
	///// <typeparam name="T"></typeparam>
	//public static class ListPool<T>
	//{
	//	private static List<>
	//}
}