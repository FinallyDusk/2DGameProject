#if UNITY_EDITOR
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace DataMap
{
    [Serializable]
    public class MapContent
    {
        //[ReadOnly]
        //[VerticalGroup("Content/Right")]
        public string Name;
        //[ReadOnly]
        //[VerticalGroup("Content/Right")]
        public string GUID;
        //[ReadOnly]
        //[VerticalGroup("Content/Right")]
        public string MapType;

        [JsonIgnore]
        public DataFileMapWindow Instance;

        public TextAsset DataFile;
        [JsonIgnore]
        public IEnumerable<DataRowBase> DisplayObjs;

        /// <summary>
        /// 打开数据窗口，也相当于读取数据
        /// </summary>
        public void OpenContentWindow()
        {
            if (DataFile == null)
            {
                var path = AssetDatabase.GUIDToAssetPath(GUID);
                DataFile = AssetDatabase.LoadAssetAtPath<TextAsset>(path);
            }
            var t_type = typeof(List<>);
            Assembly asm = Assembly.Load("Assembly-CSharp");
            var genericType = asm.GetType(MapType);
            t_type = t_type.MakeGenericType(genericType);
            var objs = JsonConvert.DeserializeObject(DataFile.text, t_type);
            DisplayObjs = objs as IEnumerable<DataRowBase>;
            MapContentWindowManager.OpenContentWindow(this);
        }

        public void Remove()
        {
            Instance.RemoveMapContent(GUID);
            //此处应该还需要关闭打开的内容窗口，暂时不做
        }

        /// <summary>
        /// 存储数据
        /// </summary>
        public void SaveData()
        {
            var path = AssetDatabase.GUIDToAssetPath(GUID);
            var outputPath = Application.dataPath.Replace("Assets", string.Empty) + path;
            if (File.Exists(outputPath))
            {
                var str = JsonConvert.SerializeObject(DisplayObjs);
                File.WriteAllText(outputPath, str);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }
    }
}

#endif