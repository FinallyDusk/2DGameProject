using BoringWorld.UI.CombatForm;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace BoringWorld
{
	public class TestScripts : MonoBehaviour
	{

        private void Awake()
        {
            var r = Random.Range(0, 100);
            //GameEntry.SpecialEffect.PlaySpecialEffect("", 1, new Vector3Int(), Vector3Int.zero);
           //Log.Debug()
        }

        private void Update()
        {
        }

        private void A1(string s)
        {
            Debug.Log("这是提示显示的方法");
        }

        private void A2()
        {
            Debug.Log("这是提示<color=#00ff00>隐藏</color>的方法");
        }

        private void A3(int i)
        {
            Debug.Log("这是<color=#ff0000>按钮点击</color>的方法");
        }
    }
}