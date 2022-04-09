using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework;
using UnityGameFramework.Runtime;
using System.Linq;

namespace BoringWorld
{
	public static class CustomExtension
	{
        public static void SetActive<T>(this T com, bool value) where T : Component
        {
            if (com.gameObject.activeSelf == value) return;
            com.gameObject.SetActive(value);
        }

        public static T ToEnum<T>(this string str) where T : System.Enum
        {
            return (T)System.Enum.Parse(typeof(T), str);
        }

        public static T[] ToEnumArr<T>(this string str, char splitOperator = ',') where T : System.Enum
        {
            string[] args = str.Split(splitOperator);
            T[] result = new T[args.Length];
            for (int i = 0; i < args.Length; i++)
            {
                result[i] = args[i].ToEnum<T>();
            }
            return result;
        }

        public static bool IsNullOrEmpty<T>(this IList<T> ts)
        {
            if (ts == null || ts.Count == 0) return true;
            return false;
        }

        public static void SetRaycastAndAlpha(this CanvasGroup cg, bool enabled)
        {
            cg.blocksRaycasts = enabled;
            cg.alpha = enabled ? 1 : 0;
        }

        public static T GetComponentWithName<T>(this Component com, string GameObjectName) where T : Object
        {
            if (com.name == GameObjectName)
            {
                return com.GetComponent<T>();
            }
            foreach (Transform item in com.transform)
            {
                T temp = item.GetComponentWithName<T>(GameObjectName);
                if (temp != null)
                {
                    return temp;
                }
            }
            return null;
        }

        public static string Format(this string str, object arg0)
        {
            return Utility.Text.Format(str, arg0);
        }

        public static string Format(this string str, object arg0, object arg1)
        {
            return Utility.Text.Format(str, arg0, arg1);
        }

        public static string Format(this string str, object arg0, object arg1, object arg2)
        {
            return Utility.Text.Format(str, arg0, arg1, arg2);
        }

        public static string Format(this string str, params object[] args)
        {
            return Utility.Text.Format(str, args);
        }

        public static void ArrCopy<T>(this T[] ori, out T[] target)
        {
            target = new T[ori.Length];
            ori.CopyTo(target, 0);
        }

        public static string AddColor(this string ori, string color)
        {
            return $"<color={color}>{ori}</color>";
        }

        public static Color ToColor(this string source)
        {
            string colorStr = source.Substring(1, source.Length - 1);
            float r = System.Convert.ToInt32(colorStr.Substring(0, 2), 16) / 255f;
            float g = System.Convert.ToInt32(colorStr.Substring(2, 2), 16) / 255f;
            float b = System.Convert.ToInt32(colorStr.Substring(4, 2), 16) / 255f;
            if (colorStr.Length == 8)
            {
                float a = System.Convert.ToInt32(colorStr.Substring(6, 2), 16) / 255f;
                return new Color(r, g, b, a);
            }
            return new Color(r, g, b);
        }

        public static Vector2 ToV2(this string source, char splitOperator = ',')
        {
            string[] args = source.Split(splitOperator);
            if (args.Length != 2)
            {
                Log.Error("传入的值{0}不能解析为Vector2类型".Format(source));
            }
            float x = float.Parse(args[0]);
            float y = float.Parse(args[1]);
            Vector2 result = new Vector2(x, y);
            return result;
        }

        public static int[] ToIntArr(this string str, char splitOperator = ',')
        {
            if (string.IsNullOrEmpty(str))
            {
                return System.Array.Empty<int>();
            }
            string[] strs = str.Split(splitOperator);
            int[] result = new int[strs.Length];
            for (int i = 0; i < strs.Length; i++)
            {
                result[i] = int.Parse(strs[i]);
            }
            return result;
        }

        public static float[] ToFloatArr(this string str, char splitOperator = ',')
        {
            if (string.IsNullOrEmpty(str))
            {
                return System.Array.Empty<float>();
            }
            string[] strs = str.Split(splitOperator);
            float[] result = new float[strs.Length];
            for (int i = 0; i < strs.Length; i++)
            {
                result[i] = float.Parse(strs[i]);
            }
            return result;
        }

        /// <summary>
        /// 随机一个值
        /// </summary>
        /// <returns></returns>
        public static T RandomOne<T>(this IList<T> list)
        {
            if (list.IsNullOrEmpty()) return default;
            return list[Random.Range(0, list.Count)];
        }

        public static Vector3Int ToInt(this Vector3 v3)
        {
            Vector3Int v3i = new Vector3Int(Mathf.FloorToInt(v3.x), Mathf.FloorToInt(v3.y), Mathf.FloorToInt(v3.z));
            return v3i;
        }

        public static bool Has<T>(this T[] source, T value)
        {
            if (source.IsNullOrEmpty()) return false;
            for (int i = 0; i < source.Length; i++)
            {
                if (source[i].Equals(value))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 两个数组中是否存在有一个或以上的相同的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static bool HasOne<T>(this T[] source, T[] values)
        {
            if (values == null || source == null) return false;
            for (int i = 0; i < source.Length; i++)
            {
                if (values.Has(source[i]))
                {
                    return true;
                }
            }
            return false;
        }
    }
}