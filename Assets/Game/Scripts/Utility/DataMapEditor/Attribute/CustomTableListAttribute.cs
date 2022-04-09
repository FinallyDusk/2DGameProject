#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace DataMap
{
    /// <summary>
    /// 使用前请自己好好查看，因为这个属性的绘制<see cref="CustomTableListAttributeDrawer"/>是特定绘制的，不建议使用
    /// </summary>
    [Conditional("UNITY_EDITOR")]
    public sealed class CustomTableListAttribute : Attribute
    {
        /// <summary>
        /// 增加的方法
        /// </summary>
        public string AddAction;
        /// <summary>
        /// 移除的之后的回调
        /// </summary>
        //public string RemoveActionCallback;

        public CustomTableListAttribute()
        {

        }
    }
}

#endif