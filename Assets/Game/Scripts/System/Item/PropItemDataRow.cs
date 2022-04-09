using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BoringWorld
{
    public class PropItemDataRow : ItemDataRow
    {
        public float CD { get; private set; }
        public int SkillID { get; private set; }

        public override string GetItemDes()
        {
            throw new System.NotImplementedException();
        }
    } 
}
