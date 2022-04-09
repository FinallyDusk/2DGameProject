using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace BoringWorld
{
    public interface ILoopSliderItem
    {
        /// <summary>
        /// 往下移动时的操作
        /// </summary>
        void NextMove();
        /// <summary>
        /// 往前移动时的操作
        /// </summary>
        void PrevMove();
        /// <summary>
        /// 刷新数据显示
        /// </summary>
        /// <param name="data"></param>
        void RefreshData(object data);
        /// <summary>
        /// 初始化Item
        /// </summary>
        /// <param name="data"></param>
        void InitItem(object userData);
        /// <summary>
        /// 无内容需要隐藏时
        /// </summary>
        void Hide();
        /// <summary>
        /// 获得一个Item的可移动范围
        /// </summary>
        /// <returns></returns>
        Vector2 GetMoveableWidthAndHeight();
    }
    //public class ILoopSliderItem : MonoBehaviour
    //{
    //       public void NextMove(object data)
    //       {
    //           transform.SetAsLastSibling();
    //           RefreshData(data);
    //       }

    //       public void PrevMove(object data)
    //       {
    //           transform.SetAsFirstSibling();
    //           RefreshData(data);
    //       }

    //       public void RefreshData(object data)
    //       {

    //       }

    //       public void InitItem(object data)
    //       {
    //           transform.SetActive(true);
    //           RefreshData(data);
    //       }

    //       public void Hide()
    //       {
    //           transform.SetActive(false);
    //       }

    //}

}