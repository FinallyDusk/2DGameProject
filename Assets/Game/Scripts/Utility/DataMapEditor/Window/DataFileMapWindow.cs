#if UNITY_EDITOR
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace DataMap
{
    /// <summary>
    /// 数据文件映射
    /// </summary>
    public class DataFileMapWindow : OdinEditorWindow
    {
        [MenuItem("Tools/Custom/Data Window")]
        private static void OpenWindow()
        {
            var window = GetWindow<DataFileMapWindow>();
            window.position = GUIHelper.GetEditorWindowRect().AlignCenter(800, 600);
            //window.minSize = new Vector2(500, 300);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            MapFile = AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/Game/Editor/DataMap/DatamapFile.json");
            OnMapFileValueChange();
        }

        [OnValueChanged("OnSearchStrValueChange"), LabelText("搜索"), LabelWidth(30)]
        public string SearchStr;

        private void OnSearchStrValueChange()
        {
            if (string.IsNullOrEmpty(SearchStr))
            {
                if (ShowMap.Count == AllMap.Count)
                {
                    return;
                }
                else
                {
                    ShowMap.Clear();
                    ShowMap.AddRange(AllMap);
                }
            }
            else
            {
                ShowMap.Clear();
                foreach (var item in AllMap)
                {
                    if (item.Name.Contains(SearchStr))
                    {
                        ShowMap.Add(item);
                    }
                }
            }
        }

        /// <summary>
        /// 映射文件
        /// </summary>
        [OnValueChanged("OnMapFileValueChange")]
        public TextAsset MapFile;

        private void OnMapFileValueChange()
        {
            AllMap.Clear();
            ShowMap.Clear();
            if (MapFile == null || string.IsNullOrEmpty(MapFile.text))
            {
                AllMap.Clear();
                //m_ClearCallback?.Invoke();
                return;
            }
            var data = JsonConvert.DeserializeObject<List<MapContent>>(MapFile.text);
            foreach (var item in data)
            {
                item.Instance = this;
                AllMap.Add(item);
            }
            ShowMap.AddRange(AllMap);
            System.GC.Collect();
        }

        [CustomTableList(AddAction = "AddFileMap"/*, RemoveActionCallback = "RemoveFileMapCallback"*/)]
        [ShowIf("$MapFile", null)]
        public List<MapContent> ShowMap;

        private List<MapContent> AllMap;

        public DataFileMapWindow()
        {
            AllMap = new List<MapContent>();
            ShowMap = new List<MapContent>();
            EnableAddFileMap = false;
        }

        /// <summary>
        /// 创建文件映射
        /// </summary>
        [Button(Name = "创建映射文件")]
        [HideIf("EnableAddFileMap")]
        [HorizontalGroup("MapFileActionBtn")]
        public void CreateFileMap()
        {
            string showPath = string.Empty;
            if (Selection.assetGUIDs.Length > 0)
            {
                showPath = Application.dataPath.Replace("Assets", string.Empty) + AssetDatabase.GUIDToAssetPath(Selection.assetGUIDs[0]);
            }
            else
            {
                showPath = Application.dataPath;
            }
            var path = EditorUtility.SaveFilePanel("请选择文件保存路径", showPath, string.Empty, "json");
            if (string.IsNullOrEmpty(path)) return;
            var dirPath = Path.GetDirectoryName(path);
            if (Directory.Exists(dirPath))
            {
                var fs = File.Create(path);
                fs.Dispose();
                fs.Close();
                AssetDatabase.Refresh();
                AssetDatabase.SaveAssets();
                //将创建的文件复制给MapFile
                var filePath = path.Replace(Application.dataPath, "Assets");
                MapFile = AssetDatabase.LoadAssetAtPath<TextAsset>(filePath);
                OnMapFileValueChange();
            }
            else
            {
                EditorUtility.DisplayDialog("出错", "请选择工程内资源文件夹", "确定");
            }
        }

        [Button(Name = "增加映射关系")]
        [HideIf("EnableAddFileMap")]
        [EnableIf("MapFile", null)]
        [HorizontalGroup("MapFileActionBtn")]
        public void AddFileMap()
        {
            EnableAddFileMap = true;
        }

        /// <summary>
        /// 移除映射关系
        /// </summary>
        public void RemoveMapContent(string removeGUID)
        {
            MapContent removeContent = null;
            foreach (var item in ShowMap)
            {
                if (item.GUID == removeGUID)
                {
                    removeContent = item;
                    break;
                }
            }
            if (removeContent != null)
            {
                ShowMap.Remove(removeContent);
            }
            foreach (var item in AllMap)
            {
                if (item.GUID == removeGUID)
                {
                    removeContent = item;
                    break;
                }
            }
            if (removeContent != null)
            {
                AllMap.Remove(removeContent);
            }
            SaveToMapFile();
        }


        #region 新增映射的地方

#pragma warning disable 0414
        private bool EnableAddFileMap;
#pragma warning restore 0414

        [InfoBox("$TemplateMessage", InfoMessageType.Warning, VisibleIf = "EnableMsg")]
        [ShowIf("EnableAddFileMap")]
        [TabGroup("FileMap")]
        [LabelWidth(60), LabelText("数据文件")]
        public TextAsset TempMapContactKey;


        [ShowIf("EnableAddFileMap"), ValueDropdown("GetAllDataType")]
        [TabGroup("FileMap")]
        [LabelWidth(60), LabelText("映射类型")]
        public string TempMapContactValue;

        private List<string> GetAllDataType()
        {
            Assembly asm = Assembly.Load("Assembly-CSharp");
            var allTypes = asm.GetTypes();
            List<string> result = new List<string>();
            Type condition = typeof(UnityGameFramework.Runtime.DataRowBase);
            foreach (var item in allTypes)
            {
                if (condition.IsAssignableFrom(item) && item.IsAbstract == false && item.IsInterface == false)
                {
                    result.Add(item.FullName);
                }
            }
            return result;
        }

        //[HideInInspector]
        private string TemplateMessage;

        private bool EnableMsg()
        {
            return !string.IsNullOrEmpty(TemplateMessage);
        }

        [Button(Name = "确定"), ShowIf("EnableAddFileMap")]
        [HorizontalGroup("FileMap")]
        [EnableIf("EnableConfirmAddFileMapBtn")]
        private void ConfirmAddFileMap()
        {
            MapContent mc = new MapContent();
            mc.Instance = this;
            mc.Name = TempMapContactKey.name;
            mc.GUID = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(TempMapContactKey));
            mc.MapType = TempMapContactValue;
            AllMap.Add(mc);
            TempMapContactKey = null;
            TempMapContactValue = null;
            EnableAddFileMap = false;
            //清除搜索
            SearchStr = string.Empty;
            OnSearchStrValueChange();
            //反写入源文件
            SaveToMapFile();
        }

        private bool EnableConfirmAddFileMapBtn()
        {
            if (TempMapContactKey == null) return false;
            if (string.IsNullOrEmpty(TempMapContactValue)) return false;
            var guid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(TempMapContactKey));
            if (AllMap.Find((mc) => { return mc.GUID == guid; }) != null)
            {
                TemplateMessage = "已经有相同的文件了";
                return false;
            }
            TemplateMessage = string.Empty;
            return true;
        }


        [Button(Name = "取消"), ShowIf("EnableAddFileMap")]
        [HorizontalGroup("FileMap")]
        private void CancelAddFileMap()
        {
            TempMapContactKey = null;
            TempMapContactValue = null;
            EnableAddFileMap = false;
        }

        #endregion

        /// <summary>
        /// 将当前映射关系写入映射文件中
        /// </summary>
        [Button(Name = "保存")]
        [HideIf("EnableAddFileMap")]
        [EnableIf("MapFile", null)]
        [HorizontalGroup("MapFileActionBtn")]
        private void SaveToMapFile()
        {
            var content = JsonConvert.SerializeObject(AllMap);
            var tPath = AssetDatabase.GetAssetPath(MapFile);
            var outPath = Application.dataPath.Replace("Assets", string.Empty) + tPath;
            if (File.Exists(outPath))
            {
                File.WriteAllText(outPath, content);
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        #region 第二行按钮，多与映射文件无关

        private bool EnabledRefreshBtn()
        {
            if (AllMap != null && AllMap.Count > 0)
            {
                return true;
            }
            return false;
        }

        #endregion


    }
}

#endif