using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BoringWorld
{
    /// <summary>
    /// 计时系统
    /// </summary>
    public class GameTimerSystem
    {
        /// <summary>
        /// 空闲计时器组
        /// </summary>
        private List<GameTimerItem> m_FreeTimer;
        /// <summary>
        /// 待机计时器组（装备工作的）
        /// </summary>
        private List<GameTimerItem> m_IdleTimerList;
        private List<GameTimerItem> m_WorkTimerList;
        private List<GameTimerItem> m_PauseTimerList;
        private List<GameTimerItem> m_PrepareAddWorkTimerList;
        private List<GameTimerItem> m_PrepareRemoveWorkTimerList;
        private List<GameTimerItem> m_PreparePauseWorkTimerList;
        private List<GameTimerItem> m_PrepareContinueWorkTimerList;

        public void InitSystem()
        {
            m_FreeTimer = new List<GameTimerItem>();
            m_IdleTimerList = new List<GameTimerItem>();
            m_WorkTimerList = new List<GameTimerItem>();
            m_PauseTimerList = new List<GameTimerItem>();
            m_PrepareAddWorkTimerList = new List<GameTimerItem>();
            m_PrepareRemoveWorkTimerList = new List<GameTimerItem>();
            m_PreparePauseWorkTimerList = new List<GameTimerItem>();
            m_PrepareContinueWorkTimerList = new List<GameTimerItem>();
        }

        public GameTimerItem GetTimer()
        {
            GameTimerItem item = null;
            if (m_FreeTimer.Count == 0)
            {
                item = new GameTimerItem(this);
            }
            else
            {
                item = m_FreeTimer[0];
                m_FreeTimer.RemoveAt(0);
            }
            m_IdleTimerList.Add(item);
            return item;
        }

        public void AddWorkTimer(GameTimerItem item)
        {
            if (m_IdleTimerList.Contains(item) == false)
            {
                throw new System.Exception("请从GameTimerSystem.GetTimer()来获取GameTimerItem");
            }
            m_PrepareAddWorkTimerList.Add(item);
        }

        public void RemoveWorkTimer(GameTimerItem item)
        {
            if (m_WorkTimerList.Contains(item) == false) return;
            m_PrepareRemoveWorkTimerList.Add(item);
        }

        public void PauseWorkTimer(GameTimerItem item)
        {
            if (m_WorkTimerList.Contains(item) == false) return;
            m_PreparePauseWorkTimerList.Add(item);
        }

        public void ContinueWorkTimer(GameTimerItem item)
        {
            if (m_PauseTimerList.Contains(item) == false) return;
            m_PrepareContinueWorkTimerList.Add(item);
        }

        /// <summary>
        /// 更新计时器
        /// </summary>
        /// <param name="deltaTime"></param>
        public void OnUpdate(float deltaTime)
        {
            AddWorkTimer();
            PauseWorkTimer();
            ContinueWorkTimer();
            RemoveWorkTimer();
            PollWorkTimer(deltaTime);
        }

        /// <summary>
        /// 增加工作计时器
        /// </summary>
        private void AddWorkTimer()
        {
            for (int i = 0; i < m_PrepareAddWorkTimerList.Count; i++)
            {
                m_IdleTimerList.Remove(m_PrepareAddWorkTimerList[i]);
                m_WorkTimerList.Add(m_PrepareAddWorkTimerList[i]);
            }
            m_PrepareAddWorkTimerList.Clear();
        }

        /// <summary>
        /// 暂停工作计时器
        /// </summary>
        private void PauseWorkTimer()
        {
            for (int i = 0; i < m_PreparePauseWorkTimerList.Count; i++)
            {
                m_WorkTimerList.Remove(m_PreparePauseWorkTimerList[i]);
                m_PauseTimerList.Add(m_PreparePauseWorkTimerList[i]);
            }
            m_PreparePauseWorkTimerList.Clear();
        }

        /// <summary>
        /// 将暂停的计时器继续工作
        /// </summary>
        private void ContinueWorkTimer()
        {
            for (int i = 0; i < m_PrepareContinueWorkTimerList.Count; i++)
            {
                m_PauseTimerList.Remove(m_PrepareContinueWorkTimerList[i]);
                m_WorkTimerList.Add(m_PrepareContinueWorkTimerList[i]);
            }
            m_PrepareContinueWorkTimerList.Clear();
        }

        /// <summary>
        /// 移除工作计时器
        /// </summary>
        private void RemoveWorkTimer()
        {
            for (int i = 0; i < m_PrepareRemoveWorkTimerList.Count; i++)
            {
                m_WorkTimerList.Remove(m_PrepareRemoveWorkTimerList[i]);
                m_FreeTimer.Add(m_PrepareRemoveWorkTimerList[i]);
            }
            m_PrepareRemoveWorkTimerList.Clear();
        }

        /// <summary>
        /// 轮询工作计时器
        /// </summary>
        private void PollWorkTimer(float deltaTime)
        {
            for (int i = 0; i < m_WorkTimerList.Count; i++)
            {
                m_WorkTimerList[i].UpdateItem(deltaTime);
            }
        }

    }

    /// <summary>
    /// 计时组件
    /// </summary>
    [System.Serializable]
    public class GameTimerItem
    {
        /// <summary>
        /// 标准次数
        /// </summary>
        private int m_CountStandard;
        /// <summary>
        /// 次数
        /// </summary>
        private int m_Count;
        /// <summary>
        /// 间隔
        /// </summary>
        private float m_IntervalTimer;
        /// <summary>
        /// 当前计时
        /// </summary>
        private float m_Time;
        /// <summary>
        /// 第一次是否触发
        /// </summary>
        private bool m_StartTrigger;
        /// <summary>
        /// 触发后的回调
        /// </summary>
        private System.Action<GameTimerExecuteData> m_TriggerCallback;
        /// <summary>
        /// 该计时器的控制者（一般用于修改时间流速）
        /// </summary>
        private ITimerController m_Controller;
        /// <summary>
        /// 计时器结束后的回调
        /// </summary>
        private System.Action m_EndCallback;
        private GameTimerSystem m_GTSystem;
        private GameTimerExecuteData m_TimerArg;
        private bool m_Init;
        public bool HasInit
        {
            get
            {
                if (m_Init == false)
                {
                    throw new GameTimerItemNotInitException("该计时器还未初始化，无法使用");
                }
                return true;
            }
        }
        /// <summary>
        /// 运行次数
        /// </summary>
        public int RunCount { get { return m_CountStandard - m_Count; } }

        public GameTimerItem(GameTimerSystem m_System)
        {
            m_Init = false;
            m_GTSystem = m_System;
            m_TimerArg = new GameTimerExecuteData();
        }

        public void InitItem(int count, float intervalTime, System.Action<GameTimerExecuteData> callback, bool startTrigger = false, ITimerController controller = null)
        {
            if (m_Init) return;
            m_Init = true;
            m_CountStandard = count;
            m_Count = count;
            m_IntervalTimer = intervalTime;
            m_StartTrigger = startTrigger;
            m_TriggerCallback = callback;
            m_Controller = controller;
            m_TimerArg = new GameTimerExecuteData();
            m_Time = 0;
        }

        public void InitExecuteData(object userData)
        {
            m_TimerArg.Init(this, userData);
        }

        /// <summary>
        /// 开始
        /// </summary>
        public void Start()
        {
            m_GTSystem.AddWorkTimer(this);
        }

        /// <summary>
        /// 暂停
        /// </summary>
        public void Pause()
        {
            m_GTSystem.PauseWorkTimer(this);
        }

        /// <summary>
        /// 继续
        /// </summary>
        public void Continue()
        {
            m_GTSystem.ContinueWorkTimer(this);
        }

        /// <summary>
        /// 强行停止计时器
        /// </summary>
        public void ForcedToStop()
        {
            EndTimer();
        }

        public void SetEndCallback(System.Action action)
        {
            m_EndCallback += action;
        }

        public void UpdateItem(float deltaTime)
        {
            if (HasInit == false) return;
            if (m_StartTrigger)
            {
                m_StartTrigger = false;
                ExecuteCallback();
            }
            if (m_Controller != null)
            {
                deltaTime = m_Controller.UpdateDeltaTime(deltaTime);
            }
            m_Time += deltaTime;
            if (m_Time >= m_IntervalTimer)
            {
                ExecuteCallback();
                m_Time -= m_IntervalTimer;
            }
        }

        private void ExecuteCallback()
        {
            m_TriggerCallback?.Invoke(m_TimerArg);
            m_Count--;
            if (m_Count == 0)
            {
                EndTimer();
            }
        }

        private void EndTimer()
        {
            m_EndCallback?.Invoke();
            m_EndCallback = null;
            m_GTSystem.RemoveWorkTimer(this);
            m_Init = false;
            m_TimerArg.Clear();
        }
    }

    /// <summary>
    /// 计时器调用时的参数
    /// </summary>
    public class GameTimerExecuteData
    {
        public GameTimerItem Timer { get; private set; }
        public object UserData { get; private set; }

        public void Init(GameTimerItem timer, object userData)
        {
            Timer = timer;
            UserData = userData;
        }

        public void Clear()
        {
            Timer = null;
            UserData = null;
        }
    }

    public class GameTimerItemNotInitException : System.Exception
    {
        public GameTimerItemNotInitException(string message) : base(message)
        {

        }
    }

    public interface ITimerController
    {
        float UpdateDeltaTime(float deltaTime);
    }
}