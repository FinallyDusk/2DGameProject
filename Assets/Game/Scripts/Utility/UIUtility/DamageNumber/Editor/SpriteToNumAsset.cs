using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using FantasyDiary;

public class SpriteToNumAsset : Editor
{
    [MenuItem("Assets/SpriteToNum")]
    private static void CreateAsset()
    {
        var obj = Selection.activeObject;
        string objPath = AssetDatabase.GetAssetPath(obj);
        TextureImporter ti = TextureImporter.GetAtPath(AssetDatabase.GetAssetPath(obj)) as TextureImporter;
        if (ti == null)
        {
            Debug.Log("请选择图片文件");
            return;
        }
        if (ti.spriteImportMode != SpriteImportMode.Multiple)
        {
            Debug.Log("图片需要为Multiple模式，如果不为此模式，则无法进行");
            return;
        }
        Texture2D t2d = obj as Texture2D;
        var widthAndHeight = new Vector2(t2d.width, t2d.height);
        var allSprites = ti.spritesheet;
        List<FontAndBorderAndIndex> fontInfos = new List<FontAndBorderAndIndex>();
        for (int i = 0; i < allSprites.Length; i++)
        {
            var info = GetFontInfo(allSprites[i], widthAndHeight);
            fontInfos.Add(info);
        }
        Material mat = new Material(Shader.Find("Custom/SpriteToNum"));
        mat.name = Path.GetFileName(objPath);
        mat.SetTexture("_MainTex", t2d);
        var data = CreateInstance<SpriteToNumData>();
        data.SourceMat = mat;
        data.FontInfos = GetFontInfosAndSort(fontInfos);
        //AssetDatabase.SaveAssets();
        string outPath = Path.ChangeExtension(objPath, ".asset");
        AssetDatabase.CreateAsset(data, outPath);
        AssetDatabase.AddObjectToAsset(mat, outPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private static FontAndBorderAndIndex GetFontInfo(SpriteMetaData data, Vector2 spriteWidthAndHeight)
    {
        FontAndBorderAndIndex result = new FontAndBorderAndIndex();
        var strs = data.name.Split('^');
        result.Index = int.Parse(strs[1]);
        result.Font = strs[0][0];
        result.Border = new Vector4(data.rect.xMin / spriteWidthAndHeight.x, data.rect.xMax / spriteWidthAndHeight.x, data.rect.yMin / spriteWidthAndHeight.y, data.rect.yMax / spriteWidthAndHeight.y);
        return result;
    }

    private static FontAndBorderAndIndex[] GetFontInfosAndSort(List<FontAndBorderAndIndex> source)
    {
        source.Sort((f1, f2) =>
        {
            if (f1.Index < f2.Index)
            {
                return -1;
            }
            else if (f1.Index == f2.Index)
            {
                return 0;
            }
            else
            {
                return 1;
            }
        });
        return source.ToArray();
    }
}
