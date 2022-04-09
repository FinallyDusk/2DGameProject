using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityGameFramework.Runtime;

namespace BoringWorld
{
	public class BWEntityGroupHelper : EntityGroupHelperBase
	{
        private void Start()
        {
            var allNames = System.Enum.GetNames(typeof(EntityGroup));
            foreach (var item in allNames)
            {
                if (gameObject.name.Contains(item))
                {
                    var sg = gameObject.AddComponent<SortingGroup>();
                    sg.sortingOrder = (int)item.ToEnum<EntityGroup>();
                    return;
                }
            }
            throw new System.Exception($"该名称找不到对应的枚举：gameObject.name = { gameObject.name}");
        }
    }
}