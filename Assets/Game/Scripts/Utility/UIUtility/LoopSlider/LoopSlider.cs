using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;

namespace BoringWorld
{
    public enum ScrollDir
    {
        Vertical,
        Horizontal,
    }

    /*
     * 使用注意：
     * 1. content记得加对应的Layout布局组件，布局的spacing最好为0（或者远远小于物体的宽度和高度），不然会出现bug
     * 2. content的Anchors必须为左上角
     * 3. content的Pivot必须为0，1
     * 4. 看不见的地方格子必须要和refreshCount个数一样
     */
	public class LoopSlider : MonoBehaviour,IScrollHandler,IDragHandler
	{
        public float dragSpeed;
        public float scrollSpeed;
        public int hideCount;
        /// <summary>
        /// 刷新个数
        /// </summary>
        public int refreshCount = 1;
        public ScrollDir scrollDir;

        private LinkedList<ILoopSliderItem> items;
        /// <summary>
        /// 上限，往上滚动的时候
        /// </summary>
        private Vector2 topLeftLimit;
        /// <summary>
        /// 下限，往下滚动的时候
        /// </summary>
        private Vector2 downRightLimit;
        /// <summary>
        /// 数据源
        /// </summary>
        protected virtual ILoopSliderDataSource DataSource { get; set; }
        private int upSign;
        private int downSign;
        private int maxCount;
        private int showCount;
        private float moveableHeight;
        private float moveableWidth;
        public RectTransform contentRT;

        public void InitLoopSlider(ILoopSliderDataSource dataSource, object userData)
        {
            InitLoopSlider(dataSource, dataSource.GetDataMaxCount(), userData);
        }

        public void InitLoopSlider(ILoopSliderDataSource dataSource, int maxCount, object userData)
        {
            if (items == null)
            {
                items = new LinkedList<ILoopSliderItem>();
                var temp = GetComponentsInChildren<ILoopSliderItem>(true);
                LinkedListNode<ILoopSliderItem> first = new LinkedListNode<ILoopSliderItem>(temp[0]);
                items.AddFirst(first);
                for (int i = 1; i < temp.Length; i++)
                {
                    first = items.AddAfter(first, temp[i]);
                }
                showCount = items.Count - hideCount;
                if (contentRT == null)
                {
                    contentRT = GetComponent<RectTransform>();
                }
                for (int i = 0; i < temp.Length; i++)
                {
                    temp[i].InitItem(userData);
                }
            }
            DataSource = dataSource;
            this.maxCount = maxCount;
            upSign = 0;
            downSign = 1;
            //items.Clear();
            //var temp = GetComponentsInChildren<ILoopSliderItem>(true);
            //LinkedListNode<ILoopSliderItem> first = new LinkedListNode<ILoopSliderItem>(temp[0]);
            //items.AddFirst(first);
            //for (int i = 1; i < temp.Length; i++)
            //{
            //    first = items.AddAfter(first, temp[i]);
            //}
            //showCount = items.Count - hideCount;
            //DataSource = dataSource;
            //this.maxCount = maxCount;
            //upSign = 0;
            //downSign = 1;
            //if (contentRT == null)
            //{
            //    contentRT = GetComponent<RectTransform>();
            //}
            //for (int i = 0; i < temp.Length; i++)
            //{
            //    temp[i].InitItem(userData);
            //}
            LinkedListNode<ILoopSliderItem> tempNode = items.First ;
            if (maxCount != 0)
            {
                tempNode.Value.RefreshData(DataSource.GetFirstData());
                for (int i = 1; i < Math.Min(items.Count, maxCount); i++)
                {
                    tempNode = tempNode.Next;
                    tempNode.Value.RefreshData(DataSource.GetNextData(downSign++));
                }
            }
            else
            {
                tempNode.Value.Hide();
            }
            for (int i = Math.Min(items.Count, maxCount); i < items.Count; i++)
            {
                tempNode = tempNode.Next;
                if (tempNode == null || tempNode.Value == null) break;
                tempNode.Value.Hide();
            }

            var limit = items.First.Value.GetMoveableWidthAndHeight();
            moveableHeight = limit.y;
            moveableWidth = limit.x;
            topLeftLimit = new Vector2(moveableHeight, -moveableWidth);
            downRightLimit = Vector2.zero; 
        }

        public void OnDrag(PointerEventData eventData)
        {
            ItemMove(eventData.delta * dragSpeed * 0.02f);
        }

        public void OnScroll(PointerEventData eventData)
        {
            ItemMove(eventData.scrollDelta * scrollSpeed * -0.02f);
        }

        /// <summary>
        /// 物体移动，垂直位移量
        /// </summary>
        /// <param name="detail"></param>
        private void ItemMove(Vector2 detail)
        {
            if (items.Count - hideCount >= maxCount) return;
            if (scrollDir == ScrollDir.Vertical)
            {
                contentRT.anchoredPosition += new Vector2(0, detail.y);
                if (detail.y > 0)
                {
                    //如果，在向上移动时超过了上限
                    while (contentRT.anchoredPosition.y >= topLeftLimit.x)
                    {
                        //已经达到最大数值或总物体数等于最大数据个数
                        if (downSign >= maxCount || items.Count == maxCount)
                        {
                            contentRT.anchoredPosition = new Vector2(0, moveableHeight);
                            return;
                        }
                        else
                        {
                            for (int i = 0; i < refreshCount; i++)
                            {
                                NextMove();
                            }
                            contentRT.anchoredPosition -= new Vector2(0, moveableHeight);
                        }
                    }
                }
                else if (detail.y < 0)
                {
                    while (contentRT.anchoredPosition.y <= downRightLimit.x)
                    {
                        if (upSign <= 0)
                        {
                            contentRT.anchoredPosition = Vector2.zero;
                            return;
                        }
                        else
                        {
                            for (int i = 0; i < refreshCount; i++)
                            {
                                PrevMove();
                            }
                            contentRT.anchoredPosition += new Vector2(0, moveableHeight);
                        }
                    }
                }
            }
            else if (scrollDir == ScrollDir.Horizontal)
            {
                contentRT.anchoredPosition += new Vector2(detail.x, 0);
                if (detail.x < 0)
                {
                    //如果，在向左移动时超过了上限
                    while (contentRT.anchoredPosition.x <= topLeftLimit.y)
                    {
                        //已经达到最大数值或者总物体数等于最大数据
                        if (downSign >= maxCount || items.Count == maxCount)
                        {
                            contentRT.anchoredPosition = new Vector2(-moveableWidth, 0);
                            return;
                        }
                        else
                        {
                            for (int i = 0; i < refreshCount; i++)
                            {
                                NextMove();
                            }
                            contentRT.anchoredPosition += new Vector2(moveableWidth, 0);
                        }
                    }
                }
                else if (detail.x > 0)
                {
                    while (contentRT.anchoredPosition.x >= downRightLimit.y)
                    {
                        if (upSign <= 0)
                        {
                            contentRT.anchoredPosition = Vector2.zero;
                            return;
                        }
                        else
                        {
                            for (int i = 0; i < refreshCount; i++)
                            {
                                PrevMove();
                            }
                            contentRT.anchoredPosition -= new Vector2(moveableWidth, 0);
                        }
                    }
                }
            }
        }

        private void NextMove()
        {
            var first = items.First;
            first.Value.NextMove();
            if (downSign < maxCount)
            {
                first.Value.RefreshData(DataSource.GetNextData(downSign));
                downSign++;
                upSign++;
            }
            else
            {
                first.Value.Hide();
            }
            items.RemoveFirst();
            items.AddLast(first);
        }

        private void PrevMove()
        {
            if (upSign <= 0) return;
            var last = items.Last;
            last.Value.PrevMove();
            last.Value.RefreshData(DataSource.GetPrevData(upSign));
            upSign--;
            downSign--;
            items.RemoveLast();
            items.AddFirst(last);
        }
    }
	
}