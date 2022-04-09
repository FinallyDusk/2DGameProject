using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;
using GameFramework;
using GameFramework.DataTable;
using GameFramework.Event;
using System;

namespace BoringWorld
{
    [XLua.LuaCallCSharp]
    public static class GameFrameworkExtension
    {
        public static void Release(this IReference ir)
        {
            if (ir != null)
            {
                ReferencePool.Release(ir);
            }
        }

        /// <summary>
        /// 开启UI面板，会从Config中读取对应的资源位置，请使用这个方法打开UI面板，且从<see cref="UIPath"/>类中选取对应的面板
        /// </summary>
        /// <param name="uc"></param>
        /// <param name="uiFormAssetName"></param>
        /// <returns></returns>
        public static int OpenUIFormByType(this UIComponent uc, UIFormType formType)
        {
            var row = GameEntry.DataTable.GetDataRow<UIFormDataRow>(DataTableName.UI_FORM_DATA_NAME, (int)formType);
            if (row == null)
            {
                Log.Error($"找不到对应的FormType = {formType}");
                return -1;
            }
            return uc.OpenUIForm(row.FormAssetPath, row.GroupName, true);
        }

        public static int OpenUIFormByType(this UIComponent uc, UIFormType formType, object userData)
        {
            var row = GameEntry.DataTable.GetDataRow<UIFormDataRow>(DataTableName.UI_FORM_DATA_NAME, (int)formType);
            if (row == null)
            {
                Log.Error($"找不到对应的FormType = {formType}");
                return -1;
            }
            return uc.OpenUIForm(row.FormAssetPath, row.GroupName, true, userData);
        }

        public static void ReadDataByConfig(this DataTableBase dtb, object userData = null)
        {
            dtb.ReadData(GameEntry.Config.GetString(dtb.Name), userData);
        }

        /// <summary>
        /// 尝试移除
        /// </summary>
        /// <param name="id"></param>
        /// <param name="handler"></param>
        public static void TryUnsubscribe(this EventComponent ec, int id, EventHandler<GameEventArgs> handler)
        {
            if (ec.Check(id, handler))
            {
                ec.Unsubscribe(id, handler);
            }
        }


        //public static void CloseUIForm(this UIComponent uc, string uiFormAssetName)
        //{
        //    uc.CloseUIForm(uc.GetUIForm(UIPrefabPath + uiFormAssetName));
        //}

        public static T GetDataRow<T>(this DataTableComponent dc, string dataTableName, int rowID) where T : IDataRow
        {
            var table = dc.GetDataTable<T>(dataTableName);
            return table.GetDataRow(rowID);
        }

        public static void CloseUIForm(this UIComponent uc, UIFormLogic logic)
        {
            if (logic.Available)
                uc.CloseUIForm(logic.UIForm);
        }

        /// <summary>
        /// 显示实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entityAssetName"></param>
        /// <param name="entityGroupName"></param>
        public static void ShowEntity<T>(this EntityComponent ec, int entityID, string entityAssetName, EntityGroup group) where T : EntityLogic
        {
            ec.ShowEntity<T>(entityID, entityAssetName, group.ToString());
        }

        /// <summary>
        /// 显示实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entityAssetName"></param>
        /// <param name="entityGroupName"></param>
        public static void ShowEntity<T>(this EntityComponent ec, int entityID, string entityAssetName, EntityGroup group, object userData) where T : EntityLogic
        {
            ec.ShowEntity<T>(entityID, entityAssetName, group.ToString(), userData);
        }

        public static int GetEventID(string eventName)
        {
            if (EventIDMaps.TryGetValue(eventName, out int result))
            {
                return result;
            }
            result = EventID++;
            EventIDMaps.Add(eventName, result);
            return result;
        }
        private static Dictionary<string, int> EventIDMaps = new Dictionary<string, int>();
        private static int EventID = 0;
    }

}