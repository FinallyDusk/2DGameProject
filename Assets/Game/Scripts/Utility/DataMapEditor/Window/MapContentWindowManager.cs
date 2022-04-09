#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DataMap
{
    /// <summary>
    /// 数据类窗口管理器
    /// </summary>
    public static class MapContentWindowManager
    {
        private static Dictionary<MapContent, MapContentWindow> AllWindow = new Dictionary<MapContent, MapContentWindow>();

        public static void OpenContentWindow(MapContent content)
        {
            if (AllWindow.TryGetValue(content, out var window))
            {
                window.Focus();
            }
            else
            {
                window = ScriptableObject.CreateInstance<MapContentWindow>();
                window.Show();
                window.Content = content;
                window.DisplayObjs = content.DisplayObjs;
                AllWindow.Add(content, window);
            }
        }

        public static void CloseContentWindow(MapContent content)
        {
            AllWindow.Remove(content);
        }
    }
}

#endif