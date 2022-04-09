using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

/// <summary>
/// 图片工具面板
/// </summary>
public class SpriteWindow : EditorWindow
{
    private Vector2 m_PrepareChangePivot;

    [MenuItem("Tools/Custom/Sprite Tool")]
    private static void CreateWindow()
    {
        var window = CreateInstance<SpriteWindow>();
        window.minSize = new Vector2(500, 300);
        window.Show();
    }

    private void OnGUI()
    {
        DrawChangeSpritePivotArea();
        DrawChangeSpritePixelsPerUnitArea();
    }

    #region 对spirte的pivot进行更改

    /// <summary>
    /// 更改Sprite的Pivot
    /// </summary>
    private void DrawChangeSpritePivotArea()
    {
        GUILayout.BeginVertical();
        GUILayout.Label("更改选中图片的Pivot（不支持多张图的），值为-1时表示不更改此值");
        m_PrepareChangePivot = EditorGUILayout.Vector2Field(string.Empty, m_PrepareChangePivot);
        if (GUILayout.Button("更改"))
        {
            ChangeSpritePivot();
        }
        GUILayout.EndVertical();
    }

    private void ChangeSpritePivot()
    {
        var allPaths = GetAllTexturePath();
        for (int i = 0; i < allPaths.Length; i++)
        {
            string path = allPaths[i];
            TextureImporter ti = (TextureImporter)TextureImporter.GetAtPath(path);
            if (ti == null) continue;
            var setting = new TextureImporterSettings();
            ti.ReadTextureSettings(setting);
            setting.spriteAlignment = 9;
            ti.SetTextureSettings(setting);
            ti.spritePivot = new Vector2(m_PrepareChangePivot.x == -1 ? ti.spritePivot.x : m_PrepareChangePivot.x, m_PrepareChangePivot.y == -1 ? ti.spritePivot.y : m_PrepareChangePivot.y);
            AssetDatabase.WriteImportSettingsIfDirty(path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }

    #endregion

    #region 对spirte的PixelsPerUnit进行更改

    private SpritePixelsPerUnitType m_SPPUT;
    private float m_PixelsPerUnitVale = -1;

    private void DrawChangeSpritePixelsPerUnitArea()
    {
        GUILayout.BeginVertical();
        GUILayout.Label("更改选中图片的PixelsPerUnit，默认使用图片宽度作为值");
        m_SPPUT = (SpritePixelsPerUnitType)EditorGUILayout.EnumPopup(m_SPPUT);
        m_PixelsPerUnitVale = EditorGUILayout.FloatField(m_PixelsPerUnitVale);
        if (GUILayout.Button("更改图片像素对应大小"))
        {
            ChangeSpritePixelsPerUnit();
        }
        GUILayout.EndVertical();
    }

    private void ChangeSpritePixelsPerUnit()
    {
        var allPath = GetAllTexturePath();
        for (int i = 0; i < allPath.Length; i++)
        {
            string path = allPath[i];
            TextureImporter ti = (TextureImporter)TextureImporter.GetAtPath(path);
            if (ti == null) continue;
            var t2d = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            if (m_PixelsPerUnitVale != -1 && m_PixelsPerUnitVale > 0)
            {
                ti.spritePixelsPerUnit = m_PixelsPerUnitVale;
            }
            else if (m_SPPUT == SpritePixelsPerUnitType.Width)
            {
                ti.spritePixelsPerUnit = t2d.width;
            }
            else if (m_SPPUT == SpritePixelsPerUnitType.Height)
            {
                ti.spritePixelsPerUnit = t2d.height;
            }
            AssetDatabase.WriteImportSettingsIfDirty(path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }

    #endregion

    private string[] GetAllTexturePath()
    {
        var allT2ds = Selection.GetFiltered(typeof(Texture2D), SelectionMode.DeepAssets);
        string[] result = new string[allT2ds.Length];
        for (int i = 0; i < allT2ds.Length; i++)
        {
            result[i] = AssetDatabase.GetAssetPath(allT2ds[i]);
        }
        return result;
    }
}

public enum SpritePixelsPerUnitType
{
    Width,
    Height,
}