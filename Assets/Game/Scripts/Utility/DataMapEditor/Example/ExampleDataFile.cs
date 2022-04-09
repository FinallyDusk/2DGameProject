#if UNITY_EDITOR
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace DataMap.Example
{
    public class ExampleDataFile : IDisplayTreeItem
    {
        [JsonProperty]
        [ShowInInspector]
        private int id;

        [JsonProperty]
        [ShowInInspector]
        public string Name { get; private set; }
        [JsonProperty]
        [ShowInInspector]
        [ValueDropdown("AllScirptFileName")]
        public string ScriptFile { get; private set; }

        private List<string> AllScirptFileName()
        {
            //var allTextAsset = AssetImporter.FindObjectsOfType<TextureImporter>();
            var allTextAsset = AssetDatabase.GetAllAssetPaths();
            // var allTextAsset = AssetUtilities.GetAllAssetsOfType<TextAsset>();
            //var allTextAsset = AssetDatabase.LoadAllAssetRepresentationsAtPath(Application.dataPath);
            //var allTextAsset = Resources.FindObjectsOfTypeAll<TextAsset>();
            string[] extensions = { ".txt", ".json" };
            List<string> result = new List<string>();
            foreach (var item in allTextAsset)
            {
                var path = item;//AssetDatabase.GetAssetPath(item);
                var fileEx = Path.GetExtension(path);
                foreach (var ex in extensions)
                {
                    if (ex == fileEx)
                    {
                        result.Add(Path.GetFileName(path));
                        break;
                    }
                }
            }
            return result;
        }

        [JsonProperty]
        [ShowInInspector]
        public float MoveSpeed { get; private set; }

        [JsonProperty]
        [ShowInInspector]
        public BodySize Size { get; private set; }
    }

    public enum BodySize
    {
        Large,
        Middle,
        Small,
    }
}

#endif