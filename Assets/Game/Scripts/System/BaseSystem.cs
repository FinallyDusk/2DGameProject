using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BoringWorld
{
	public abstract class BaseSystem
	{
		private System.Action m_PreLoadCallback;
		/// <summary>
		/// 预加载时分配的加载进度
		/// </summary>
		private float m_PreLoadTotalProgress;

		/// <summary>
		/// 系统预加载方法
		/// </summary>
		/// <param name="callback"></param>
		public void PreLoad(System.Action callback, float totalProgress)
        {
			m_PreLoadCallback = callback;
			m_PreLoadTotalProgress = totalProgress;
			InternalPreLoadResources();
        }

		/// <summary>
		/// 系统预加载资源，在数据表读取完之后发生
		/// </summary>
		protected abstract void InternalPreLoadResources();

		/// <summary>
		/// 预加载完成
		/// </summary>
		protected void PreLoadFinsh()
        {
			GameMain.LoadProgress.ChangeLoadProgress(m_PreLoadTotalProgress);
			m_PreLoadCallback?.Invoke();
		}

		/// <summary>
		/// 系统初始化
		/// </summary>
		public virtual void OnInit()
        {

        }

		/// <summary>
		/// 系统正式启动
		/// </summary>
		public virtual void OnEnter()
        {

        }

		public virtual void OnExit()
        {

        }
	}
}