using Sirenix.OdinInspector.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Sirenix.OdinInspector;

public class UnityAllTextureWindow : OdinMenuEditorWindow
{
    [MenuItem("Tools/Custom/All Texture Window")]
    public static void OpenWindow()
    {
        var window = GetWindow<UnityAllTextureWindow>();
    }

    protected override OdinMenuTree BuildMenuTree()
    {
        OdinMenuTree tree = new OdinMenuTree();
        OdinMenuTreeDrawingConfig config = new OdinMenuTreeDrawingConfig();
        config.DrawSearchToolbar = true;
        tree.Config = config;
        var m_Icons = new List<PreviewTexture>();
        Texture2D[] t = Resources.FindObjectsOfTypeAll<Texture2D>();
        foreach (Texture2D x in t)
        {
            Debug.unityLogger.logEnabled = false;
            GUIContent gc = EditorGUIUtility.IconContent(x.name);
            Debug.unityLogger.logEnabled = true;
            if (gc != null && gc.image != null)
            {
                PreviewTexture pt = new PreviewTexture();
                pt.Name = gc.image.name;
                pt.Image = gc.image;
                m_Icons.Add(pt);
            }
        }
        foreach (var item in m_Icons)
        {
            tree.Add(item.Name, item, item.Image);
        }
        return tree;
    }

    public class PreviewTexture
    {
        public string Name;
        [PreviewField(Height = 300f, Alignment = ObjectFieldAlignment.Center)]
        public Texture Image;
    }
}
