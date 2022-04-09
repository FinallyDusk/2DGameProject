using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace BoringWorld
{
	public class PlayerInputSystem
	{
		private Dictionary<KeyCodeStatus, Dictionary<KeyCode, System.Action>> m_InputAction;
		private Dictionary<KeyCodeStatus, Dictionary<KeyCode, List<System.Action>>> m_PrepareRemoveKeyCodeAction;
		private KeyCodeStatus[] m_AllKeyCodeStatus;
		private Dictionary<KeyCodeStatus, List<KeyCode>> m_DisableKeyCode;
	
	    public void OnInit()
	    {

			m_DisableKeyCode = new Dictionary<KeyCodeStatus, List<KeyCode>>();
			m_InputAction = new Dictionary<KeyCodeStatus, Dictionary<KeyCode, System.Action>>();
			m_PrepareRemoveKeyCodeAction = new Dictionary<KeyCodeStatus, Dictionary<KeyCode, List<System.Action>>>();
			var t_AllStatus = System.Enum.GetValues(typeof(KeyCodeStatus));
			m_AllKeyCodeStatus = new KeyCodeStatus[t_AllStatus.Length];
            for (int i = 0; i < t_AllStatus.Length; i++)
            {
				m_AllKeyCodeStatus[i] = (KeyCodeStatus)t_AllStatus.GetValue(i);
            }
            for (int i = 0; i < m_AllKeyCodeStatus.Length; i++)
            {
				m_InputAction.Add(m_AllKeyCodeStatus[i], new Dictionary<KeyCode, System.Action>());
				m_PrepareRemoveKeyCodeAction.Add(m_AllKeyCodeStatus[i], new Dictionary<KeyCode, List<System.Action>>());
				m_DisableKeyCode.Add(m_AllKeyCodeStatus[i], new List<KeyCode>());
            }
	    }

		/// <summary>
		/// 添加禁用按钮按下事件
		/// </summary>
		/// <param name="code"></param>
		public void AddDisableKeyCodeDown(KeyCode code)
        {
			AddDisableKeyCode(KeyCodeStatus.Down, code);
		}

		/// <summary>
		/// 移除禁用按钮按下事件
		/// </summary>
		/// <param name="code"></param>
		public void RemoveDisableKeyCodeDown(KeyCode code)
        {
			RemoveDisableKeyCode(KeyCodeStatus.Down, code);
		}

		private void AddDisableKeyCode(KeyCodeStatus status, KeyCode code)
		{
			if (m_DisableKeyCode[status].Contains(code)) return;
			m_DisableKeyCode[status].Add(code);
		}

		private void RemoveDisableKeyCode(KeyCodeStatus status, KeyCode code)
        {
			m_DisableKeyCode[status].Remove(code);
		}
	
		/// <summary>
		/// 注册按钮按下事件
		/// </summary>
		/// <param name="code"></param>
		/// <param name="action"></param>
	    public void RegisterKeyCodeDownAction(KeyCode code, System.Action action)
	    {
			RegisterKeyCodeAction(KeyCodeStatus.Down, code, action);
	    }
	
		/// <summary>
		/// 取消注册按钮按下事件
		/// </summary>
		/// <param name="code"></param>
		/// <param name="action"></param>
	    public void UnRegisterKeyCodeDownAction(KeyCode code, System.Action action)
	    {
			UnRegisterKeyCodeAction(KeyCodeStatus.Down, code, action);
	    }

		/// <summary>
		/// 注册按钮持续按压事件
		/// </summary>
		/// <param name="code"></param>
		/// <param name="action"></param>
		public void RegisterKeyCodeHoldOnAction(KeyCode code, System.Action action)
        {
			RegisterKeyCodeAction(KeyCodeStatus.Hold, code, action);
		}

		/// <summary>
		/// 注册按钮抬起事件
		/// </summary>
		/// <param name="code"></param>
		/// <param name="action"></param>
		public void RegisterKeyCodeUpAction(KeyCode code, System.Action action)
        {
			RegisterKeyCodeAction(KeyCodeStatus.Up, code, action);
        }

		private void RegisterKeyCodeAction(KeyCodeStatus status, KeyCode code, System.Action action)
        {
			var allKeyCodeAction = m_InputAction[status];
			if (allKeyCodeAction.ContainsKey(code) == false)
			{
				allKeyCodeAction.Add(code, action);
			}
			else
			{
				allKeyCodeAction[code] += action;
			}
		}

		private void UnRegisterKeyCodeAction(KeyCodeStatus status, KeyCode code, System.Action action)
        {
			var allPrepareRemoveActions = m_PrepareRemoveKeyCodeAction[status];
			if (!allPrepareRemoveActions.TryGetValue(code, out List<System.Action> result))
			{
				result = new List<System.Action>();
				allPrepareRemoveActions[code] = result;
			}
			result.Add(action);
		}

		/// <summary>
		/// 移除键盘事件
		/// </summary>
		private void RemoveKeyCodeAction()
	    {
	        foreach (KeyValuePair<KeyCodeStatus, Dictionary<KeyCode, List<System.Action>>> item in m_PrepareRemoveKeyCodeAction)
	        {
                foreach (KeyValuePair<KeyCode, List<System.Action>> kv in item.Value)
				{
					if (m_InputAction[item.Key].ContainsKey(kv.Key))
					{
						for (int i = 0; i < kv.Value.Count; i++)
						{
							m_InputAction[item.Key][kv.Key] -= kv.Value[i];
						}
					}
					kv.Value.Clear();
                }
	        }
	    }
	
	    public void Update()
	    {
	        RemoveKeyCodeAction();
	        foreach (var item in m_InputAction)
	        {
                switch (item.Key)
                {
                    case KeyCodeStatus.Down:
                        foreach (var keyAndAction in item.Value)
                        {
							if (Input.GetKeyDown(keyAndAction.Key))
                            {
								if (m_DisableKeyCode[item.Key].Contains(keyAndAction.Key))
                                {
									continue;
                                }
								keyAndAction.Value.Invoke();
							}
                        }
                        break;
                    case KeyCodeStatus.Up:
						foreach (var keyAndAction in item.Value)
						{
							if (Input.GetKeyUp(keyAndAction.Key))
							{
								if (m_DisableKeyCode[item.Key].Contains(keyAndAction.Key))
								{
									continue;
								}
								keyAndAction.Value.Invoke();
							}
						}
						break;
                    case KeyCodeStatus.Hold:
						foreach (var keyAndAction in item.Value)
						{
							if (Input.GetKey(keyAndAction.Key))
							{
								if (m_DisableKeyCode[item.Key].Contains(keyAndAction.Key))
								{
									continue;
								}
								keyAndAction.Value.Invoke();
							}
						}
						break;
                    default:
						Log.Fatal($"还未实现该类型的输入 KeyCodeStatus = {item.Key.ToString()}");
                        break;
                }
	        }
	    }

		private enum KeyCodeStatus
        {
			Down,
			Up,
			Hold,
        }
	}
}