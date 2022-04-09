using GameFramework.Fsm;
using GameFramework.Procedure;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace BoringWorld
{
	public class GameTitleProcedure : ProcedureBase
	{
        private int m_LoadCount = 0;

        protected override void OnInit(IFsm<IProcedureManager> procedureOwner)
        {
            base.OnInit(procedureOwner);
            GameEntry.InitComponent();
            GameMain.StartGame();
        }

        protected override void OnEnter(IFsm<IProcedureManager> procedureOwner)
        {
            base.OnEnter(procedureOwner);
            if (GameEntry.Base.EditorResourceMode)
            {
                InitResourceSuccessCallback();
            }
            else
            {
                GameEntry.Resource.InitResources(InitResourceSuccessCallback);
            }
        }

        private void InitResourceSuccessCallback()
        {
            GameEntry.Event.Subscribe(LoadConfigSuccessEventArgs.EventId, LoadConfigSuccessCallback);
            m_LoadCount++;
            GameEntry.Config.ReadData("Assets/Game/GameResource/Config/GameMain.txt", this);
            m_LoadCount++;
            GameEntry.Config.ReadData("Assets/Game/GameResource/Config/DataTablePathConfig.txt", this);
            m_LoadCount++;
            GameEntry.Config.ReadData("Assets/Game/GameResource/Config/EquipTypeDes.txt", this);
        }

        private void LoadConfigSuccessCallback(object sender, System.EventArgs e)
        {
            LoadConfigSuccessEventArgs le = e as LoadConfigSuccessEventArgs;
            if (le.UserData != this) return;
            m_LoadCount--;
            if (m_LoadCount == 0)
            {
                //设置配置
                GameMain.Config.OnInit();
                GameEntry.Event.Subscribe(LoadDataTableSuccessEventArgs.EventId, LoadDataTableSuccessCallback);
                //读取UI界面数据表
                m_LoadCount++;
                var dataTable = GameEntry.DataTable.CreateDataTable(typeof(UIFormDataRow), DataTableName.UI_FORM_DATA_NAME);
                dataTable.ReadDataByConfig(this);
                GameEntry.Event.Unsubscribe(LoadConfigSuccessEventArgs.EventId, LoadConfigSuccessCallback);
            }
        }

        private void LoadDataTableSuccessCallback(object sender, System.EventArgs e)
        {
            LoadDataTableSuccessEventArgs le = e as LoadDataTableSuccessEventArgs;
            if (le.UserData != this) return;
            m_LoadCount--; 
            if (m_LoadCount == 0)
            {
                m_LoadCount--;
                //开启标题界面
                GameEntry.UI.OpenUIFormByType(UIFormType.GameTitle);
                //开启读取进度界面
                GameEntry.UI.OpenUIFormByType(UIFormType.LoadProgress);
                //开启提示面板
                GameEntry.UI.OpenUIFormByType(UIFormType.TipForm);
                GameEntry.Event.Unsubscribe(LoadDataTableSuccessEventArgs.EventId, LoadDataTableSuccessCallback);
            }
        }

        protected override void OnUpdate(IFsm<IProcedureManager> procedureOwner, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);
            if (GameMain.GameStatus == GameStatus.PreLoad)
            {
                ChangeState<PreLoadProcedure>(procedureOwner);
            }
        }

        protected override void OnLeave(IFsm<IProcedureManager> procedureOwner, bool isShutdown)
        {
            base.OnLeave(procedureOwner, isShutdown);
        }


    }
}