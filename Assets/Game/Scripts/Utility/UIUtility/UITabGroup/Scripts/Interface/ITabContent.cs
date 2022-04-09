using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BoringWorld.UITabGroup
{
    /// <summary>
    /// 选项卡对应内容接口
    /// </summary>
    public interface ITabContent
    {
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="userData"></param>
        void OnInit(object userData);
        /// <summary>
        /// 打开内容
        /// </summary>
        void OnOpen();
        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="userData"></param>
        void OnUpdateData(object userData);
        /// <summary>
        /// 关闭内容
        /// </summary>
        void OnClose();
    } 
}