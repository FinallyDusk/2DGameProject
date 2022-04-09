#if UNITY_EDITOR
using Sirenix.OdinInspector.Editor;
using Sirenix.OdinInspector.Editor.ActionResolvers;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace DataMap
{
    public sealed class CustomTableListAttributeDrawer : OdinAttributeDrawer<CustomTableListAttribute, List<MapContent>>
    {
        private ActionResolver addAction;
        //private ActionResolver removeAction;

        protected override void Initialize()
        {
            base.Initialize();
            var t = ValueEntry.ParentType;
            if (Attribute.AddAction != null)
            {
                addAction = ActionResolver.Get(this.Property, this.Attribute.AddAction);
                //var addAction = t.GetMethod(Attribute.AddAction);
                //addAction.Invoke(ValueEntry.Context)
            }
            //if (Attribute.RemoveActionCallback != null)
            //{
            //    removeAction = ActionResolver.Get(this.Property, this.Attribute.RemoveActionCallback);
            //}
        }

        protected override void DrawPropertyLayout(GUIContent label)
        {
            //此处rect的xy以左上角为0,0
            Rect rect = EditorGUILayout.GetControlRect();
            //设置五个格子的大小，分别为增加/删除按钮格、打开格、Name格、GUID格、MapType格
            //五个格子宽度分别为20, 20, x/3, x/3, x/3 (x = width - 40)
            float lineHight = 20;
            float width_3 = (rect.width - 40) / 3;
            Rect[] colBaseRect = new Rect[]
            {
            new Rect(rect.x, rect.y, 20,  lineHight),
            new Rect(rect.x + 20, rect.y, 20,  lineHight),
            new Rect(rect.x + 40, rect.y, width_3,  lineHight),
            new Rect(rect.x + width_3 + 40, rect.y, width_3,  lineHight),
            new Rect(rect.x + width_3 * 2 + 40, rect.y, width_3,  lineHight)
            };
            //绘制外边框
            //SirenixEditorGUI.DrawBorders(rect, 1, Color.cyan);
            //颜色设置
            Color drawColor = Color.gray;
            //绘制标题
            if (SirenixEditorGUI.IconButton(colBaseRect[0].AlignCenter(colBaseRect[0].width - 4).AlignMiddle(colBaseRect[0].height - 2), EditorIcons.Plus, "增加映射关系"))
            {
                this.addAction.DoActionForAllSelectionIndices();
            }
            SirenixEditorGUI.DrawBorders(colBaseRect[0], 1, drawColor);
            SirenixEditorGUI.DrawBorders(colBaseRect[1], 1, drawColor);
            EditorGUI.LabelField(colBaseRect[2].AlignCenter(40).AlignMiddle(lineHight), "Name");
            SirenixEditorGUI.DrawBorders(colBaseRect[2], 1, drawColor);
            EditorGUI.LabelField(colBaseRect[3].AlignCenter(40).AlignMiddle(lineHight), "GUID");
            SirenixEditorGUI.DrawBorders(colBaseRect[3], 1, drawColor);
            EditorGUI.LabelField(colBaseRect[4].AlignCenter(60).AlignMiddle(lineHight), "MapType");
            SirenixEditorGUI.DrawBorders(colBaseRect[4], 1, drawColor);

            var content = this.ValueEntry.SmartValue as List<MapContent>;
            //待删除的列表行
            int removeIndex = -1;
            int displayIndex = -1;
            //绘制正式内容
            for (int i = 0; i < content.Count; i++)
            {
                Vector2 offset = new Vector2(0, lineHight * (i + 1));
                //每一行有五个元素要绘制
                if (SirenixEditorGUI.IconButton(colBaseRect[0].AlignCenter(colBaseRect[0].width - 4).AlignMiddle(colBaseRect[0].height - 2).AddPosition(offset), EditorIcons.X, "移除映射关系"))
                {
                    removeIndex = i;
                    //this.removeAction.DoAction(i);
                }
                SirenixEditorGUI.DrawBorders(colBaseRect[0].AddPosition(offset), 1, drawColor);
                if (SirenixEditorGUI.IconButton(colBaseRect[1].AlignCenter(colBaseRect[1].width - 2)/*.AlignMiddle(colBaseRect[1].height - 6)*/.AddPosition(offset), EditorGUIUtility.IconContent("d_OpenedFolder Icon").image, "打开面板"))
                {
                    displayIndex = i;
                }
                SirenixEditorGUI.DrawBorders(colBaseRect[1].AddPosition(offset), 1, drawColor);

                //后面三个值不允许修改
                EditorGUI.BeginDisabledGroup(true);
                content[i].Name = SirenixEditorFields.TextField(colBaseRect[2].AddPosition(offset), content[i].Name);
                SirenixEditorGUI.DrawBorders(colBaseRect[2].AddPosition(offset), 1, drawColor);
                content[i].GUID = SirenixEditorFields.TextField(colBaseRect[3].AddPosition(offset), content[i].GUID);
                SirenixEditorGUI.DrawBorders(colBaseRect[3].AddPosition(offset), 1, drawColor);
                content[i].MapType = SirenixEditorFields.TextField(colBaseRect[4].AddPosition(offset), content[i].MapType);
                SirenixEditorGUI.DrawBorders(colBaseRect[4].AddPosition(offset), 1, drawColor);
                EditorGUI.EndDisabledGroup();
            }
            if (removeIndex != -1)
            {
                content[removeIndex].Remove();
                //this.removeAction.DoActionForAllSelectionIndices();
            }
            if (displayIndex != -1)
            {
                content[displayIndex].OpenContentWindow();
            }
            EditorGUILayout.GetControlRect(true, lineHight * content.Count);
            GUIHelper.RequestRepaint();
        }
    }
}

#endif