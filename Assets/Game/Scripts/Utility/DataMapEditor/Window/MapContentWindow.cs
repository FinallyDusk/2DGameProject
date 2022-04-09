#if UNITY_EDITOR
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;
using UnityGameFramework.Runtime;
using BoringWorld;

namespace DataMap
{
    /// <summary>
    /// 数据类窗口
    /// </summary>
    public class MapContentWindow : OdinEditorWindow
    {
        [HideInInspector]
        public MapContent Content;
        //[TableList]//不好拖拽，创建新数据有点卡，数据多了有点卡
        [ListDrawerSettings(ListElementLabelName = "Id"), PropertyOrder(4)]
        public IEnumerable<DataRowBase> DisplayObjs;

        [Button("保存"), PropertyOrder(0), HorizontalGroup("Btn")]
        private void SaveData()
        {
            Content.SaveData();
        }

        [Button("选择文本"), HorizontalGroup("Btn"), PropertyOrder(0)]
        private void TryChooseTextFile()
        {
            Selection.activeObject = Content.DataFile;
            //Selection.SetActiveObjectWithContext(Content.DataFile, null);
            //AssetDatabase.Refresh();
        }


        /// <summary>
        /// 尝试打开类的文本文件
        /// </summary>
        [Button("尝试选择类型文件"), HorizontalGroup("Btn"), PropertyOrder(0)]
        private void TryChooseClassTypeText()
        {
            var t = Content.MapType;
            var name = t.Substring(t.LastIndexOf(".") + 1);
            var search = AssetDatabase.FindAssets(name);
            if (search == null || search.Length == 0)
            {
                Debug.LogWarning("查找不到具体的类文件");
                return;
            }
            int index = 0;
            string path = string.Empty;
            while (index < search.Length)
            {
                path = AssetDatabase.GUIDToAssetPath(search[index]);
                if (Path.GetExtension(path) != ".cs")
                {
                    index++;
                }
                else
                {
                    break;
                }
            }
            var obj = AssetDatabase.LoadAssetAtPath<TextAsset>(path);
            Selection.activeObject = obj;
            //Selection.SetActiveObjectWithContext(obj, null);
            //var allPath = AssetDatabase.GetAllAssetPaths();
            //foreach (var item in allPath)
            //{
            //    string fileName = Path.GetFileName(item);
            //    if (fileName == name)
            //    {
            //        var obj = AssetDatabase.LoadAssetAtPath<TextAsset>(item);
            //        Selection.SetActiveObjectWithContext(obj, null);
            //        return;
            //    }
            //}
        }

        protected override void OnDestroy()
        {
            MapContentWindowManager.CloseContentWindow(Content);
        }
    }
}

#endif