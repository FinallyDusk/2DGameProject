using GameFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace BoringWorld
{
    /// <summary>
    /// 单位属性,记得回收（属性计算如下：<see cref="UnitPropValueType.Fixed"/>总和 * （1 + <see cref="UnitPropValueType.Percentage"/>总和），其中<see cref="UnitPropValueType.Fixed"/>总和为基础值）
    /// </summary>
    public class UnitProp : IReference
    {
#pragma warning disable 0649
        /// <summary>
        /// 单位详细属性字典，暂时使用double进行值计算，三层分类，分别为<see cref="UnitPropType"/>、<see cref="UnitPropCategory"/>、<see cref="UnitPropValueType"/>
        /// </summary>
        private double[][][] m_AllPropDetialDict;
        /// <summary>
        /// 老的属性值
        /// </summary>
        private double[] m_OldPropValue;
        /// <summary>
        /// 单位单一属性总和字典
        /// </summary>
        private double[] m_AllPropDict;
        /// <summary>
        /// 标记哪个属性需要重新计算，不会重复添加
        /// </summary>
        private bool[] m_ValueAgainCalc;
        /// <summary>
        /// 属性值改变事件
        /// </summary>
        private System.Action<UnitProp>[] m_PropertyChangeEvent;
#pragma warning restore 0649
        public UnitProp()
        {
            var propValues = System.Enum.GetValues(typeof(UnitPropType));
            var categoryValues = System.Enum.GetValues(typeof(UnitPropCategory));
            var valueTypeValues = System.Enum.GetValues(typeof(UnitPropValueType));

            m_OldPropValue = new double[propValues.Length];
            for (int i = 0; i < m_OldPropValue.Length; i++)
            {
                m_OldPropValue[i] = 0;
            }
            m_AllPropDict = new double[propValues.Length];
            for (int i = 0; i < m_AllPropDict.Length; i++)
            {
                m_AllPropDict[i] = 0;
            }
            m_AllPropDetialDict = new double[propValues.Length][][];
            for (int i = 0; i < m_AllPropDetialDict.Length; i++)
            {
                double[][] categories = new double[categoryValues.Length][];
                for (int j = 0; j < categories.Length; j++)
                {
                    double[] valueTypes = new double[valueTypeValues.Length];
                    for (int m = 0; m < valueTypes.Length; m++)
                    {
                        valueTypes[m] = 0;
                    }
                    categories[j] = valueTypes;
                }
                m_AllPropDetialDict[i] = categories;
            }

            m_PropertyChangeEvent = new System.Action<UnitProp>[propValues.Length];
            m_ValueAgainCalc = new bool[propValues.Length];
            for (int i = 0; i < m_ValueAgainCalc.Length; i++)
            {
                m_ValueAgainCalc[i] = false;
            }

            //注册最大生命值和最大魔法值限制
            RegisterPropChangeEvent(UnitPropType.NowHp, HpLimit);
            RegisterPropChangeEvent(UnitPropType.NowMp, MpLimit);
        }

        /// <summary>
        /// 初始化单位属性（使用基础属性进行初始化）
        /// </summary>
        /// <param name="ud"></param>
        public void Init(double[] baseProp)
        {
            if (baseProp == null || baseProp.Length != m_AllPropDetialDict.Length)
            {
                Log.Error("传入的基础属性不对劲");
                return;
            }
            for (int i = 0; i < baseProp.Length; i++)
            {
                AddProperty((UnitPropType)i, UnitPropCategory.Base, UnitPropValueType.Fixed, baseProp[i]);
            }
            UpdateData();
            RegisterPropChangeEvent(UnitPropType.MaxHp, MaxHpChange);
            RegisterPropChangeEvent(UnitPropType.MaxMp, MaxMpChange);
        }

        /// <summary>
        /// 初始化单位属性（复制原数据）
        /// </summary>
        /// <param name="ori"></param>
        public void Init(UnitProp ori)
        {
            for (int x = 0; x < ori.m_AllPropDetialDict.Length; x++)
            {
                for (int y = 0; y < ori.m_AllPropDetialDict[x].Length; y++)
                {
                    for (int z = 0; z < m_AllPropDetialDict[x][y].Length; z++)
                    {
                        AddProperty((UnitPropType)x, (UnitPropCategory)y, (UnitPropValueType)z, ori.m_AllPropDetialDict[x][y][z]);
                    }
                }
                m_AllPropDict[x] = ori.m_AllPropDict[x];
                m_OldPropValue[x] = ori.m_OldPropValue[x];
            }
            UpdateData();
            RegisterPropChangeEvent(UnitPropType.MaxHp, MaxHpChange);
            RegisterPropChangeEvent(UnitPropType.MaxMp, MaxMpChange);
        }

        #region 基础属性限制

        /// <summary>
        /// 生命值限制
        /// </summary>
        /// <param name="prop"></param>
        private static void HpLimit(UnitProp prop)
        {
            PropLimit(UnitPropType.NowHp, UnitPropType.MaxHp, prop);
        }

        private static void MpLimit(UnitProp prop)
        {
            PropLimit(UnitPropType.NowMp, UnitPropType.MaxMp, prop);
        }

        private static void PropLimit(UnitPropType now, UnitPropType max, UnitProp prop)
        {
            double nowValue = prop.GetProperty(now);
            double maxValue = prop.GetProperty(max);
            if (nowValue > maxValue)
            {
                prop.SetProperty(now, UnitPropCategory.Base, UnitPropValueType.Fixed, maxValue);
            }
        }

        private static void MaxHpChange(UnitProp prop)
        {
            PropChange(UnitPropType.NowHp, UnitPropType.MaxHp, prop);
        }

        private static void MaxMpChange(UnitProp prop)
        {
            PropChange(UnitPropType.NowMp, UnitPropType.MaxMp, prop);
        }

        private static void PropChange(UnitPropType now, UnitPropType max, UnitProp prop)
        {
            int nowIndex = (int)now;
            int maxIndex = (int)max;
            double oldNowValue = prop.m_OldPropValue[nowIndex];
            double maxValue = prop.m_AllPropDict[maxIndex];
            double oldValue = prop.m_OldPropValue[maxIndex];
            if (oldValue == 0)
            {
                if (oldNowValue == 0)
                {
                    prop.SetProperty(now, UnitPropCategory.Base, UnitPropValueType.Fixed, prop.m_AllPropDict[maxIndex]);
                    return;
                }
                else
                {
                    throw new System.Exception($"出错了，最大生命值为0了");
                }
            }
            if (maxValue == 0)
            {
                throw new System.Exception($"出错了，最大生命值为0了");
            }
            double nowValue = (maxValue / oldValue) * oldNowValue;
            prop.SetProperty(now, UnitPropCategory.Base, UnitPropValueType.Fixed, nowValue);
        }

        #endregion

        #region 内部属性更新

        /// <summary>
        /// 重新计算单一属性的总和
        /// </summary>
        /// <param name="type"></param>
        private void CalcSinglePropertyTotal(int typeIndex)
        {
            if (m_ValueAgainCalc[typeIndex] == false)
            {
                return;
            }
            m_OldPropValue[typeIndex] = m_AllPropDict[typeIndex];
            var prop = m_AllPropDetialDict[typeIndex];
            double value = 0;
            double perPromote = 1;
            for (int i = 0; i < prop.Length; i++)
            {
                for (int j = 0; j < prop[i].Length; j++)
                {
                    if (j == (int)UnitPropValueType.Percentage)
                    {
                        perPromote += prop[i][j];
                    }
                }
            }
            value = CalcSinglePropertyBaseValue(typeIndex) * perPromote;
            m_AllPropDict[typeIndex] = value;
            m_ValueAgainCalc[typeIndex] = false;
            //发送消息
            m_PropertyChangeEvent[typeIndex]?.Invoke(this);
            m_OldPropValue[typeIndex] = m_AllPropDict[typeIndex];
        } 

        /// <summary>
        /// 计算单一属性基础值总和
        /// </summary>
        private double CalcSinglePropertyBaseValue(int typeIndex)
        {
            var prop = m_AllPropDetialDict[typeIndex];
            double value = 0;
            for (int i = 0; i < prop.Length; i++)
            {
                for (int j = 0; j < prop[i].Length; j++)
                {
                    if (j == (int)UnitPropValueType.Fixed)
                    {
                        value += prop[i][j];
                    }
                }
            }
            return value;
        }

        #endregion

        /// <summary>
        /// 增加属性，Hp和Mp只有基础属性，注意
        /// </summary>
        public void AddProperty(UnitPropType propType, UnitPropCategory category, UnitPropValueType valueType, double value)
        {
            m_AllPropDetialDict[(int)propType][(int)category][(int)valueType] += value;
            m_ValueAgainCalc[(int)propType] = true;
        }

        /// <summary>
        /// 设置属性值
        /// </summary>
        /// <param name="propType"></param>
        /// <param name="category"></param>
        /// <param name="valueType"></param>
        /// <param name="value"></param>
        public void SetProperty(UnitPropType propType, UnitPropCategory category, UnitPropValueType valueType, double value)
        {
            m_AllPropDetialDict[(int)propType][(int)category][(int)valueType] = value;
            m_ValueAgainCalc[(int)propType] = true;
        }

        /// <summary>
        /// 减少属性
        /// </summary>
        /// <param name="propType"></param>
        /// <param name="category"></param>
        /// <param name="valueType"></param>
        /// <param name="value"></param>
        public void ReduceProperty(UnitPropType propType, UnitPropCategory category, UnitPropValueType valueType, double value)
        {
            m_AllPropDetialDict[(int)propType][(int)category][(int)valueType] -= value;
            m_ValueAgainCalc[(int)propType] = true;
        }

        /// <summary>
        /// 获得单一属性总和，请放心使用，获取的一定为最新值
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public double GetProperty(UnitPropType type)
        {
            //获取属性总和前查看是否需要重新计算
            int typeIndex = (int)type;
            CalcSinglePropertyTotal(typeIndex);
            return m_AllPropDict[typeIndex];
        }

        /// <summary>
        /// 获得属性基础值
        /// </summary>
        /// <returns></returns>
        public double GetBaseProperty(UnitPropType type)
        {
            return CalcSinglePropertyBaseValue((int)type);
        }

        /// <summary>
        /// 获得属性额外值
        /// </summary>
        /// <returns></returns>
        public double GetExtraProperty(UnitPropType type)
        {
            return GetProperty(type) - GetBaseProperty(type);
        }

        /// <summary>
        /// 注册属性改变事件
        /// </summary>
        /// <param name="propType"></param>
        /// <param name="callback"></param>
        public void RegisterPropChangeEvent(UnitPropType propType, System.Action<UnitProp> callback)
        {
            int propIndex = (int)propType;
            m_PropertyChangeEvent[propIndex] += callback;
        }

        /// <summary>
        /// 取消注册属性改变事件
        /// </summary>
        /// <param name="propType"></param>
        /// <param name="callback"></param>
        public void UnRegisterPropChangeEvent(UnitPropType propType, System.Action<UnitProp> callback)
        {
            int propIndex = (int)propType;
            m_PropertyChangeEvent[propIndex] -= callback;
        }

        /// <summary>
        /// 获得属性值描述字符串
        /// </summary>
        /// <returns></returns>
        public string GetPropValueDes(UnitPropType propType)
        {
            return PropertyToString.GetStrOnlyValue(propType, GetProperty(propType));
        }

        /// <summary>
        /// 更新数据
        /// </summary>
        public void UpdateData()
        {
            //计算需要更新的属性总和（单一的属性）
            for (int i = 0; i < m_ValueAgainCalc.Length; i++)
            {
                CalcSinglePropertyTotal(i);
            }
        }

        public void Clear()
        {
            for (int i = 0; i < m_AllPropDetialDict.Length; i++)
            {
                for (int j = 0; j < m_AllPropDetialDict[i].Length; j++)
                {
                    for (int z = 0; z < m_AllPropDetialDict[i][j].Length; z++)
                    {
                        m_AllPropDetialDict[i][j][z] = 0;
                    }
                }
            }
            for (int i = 0; i < m_AllPropDict.Length; i++)
            {
                m_AllPropDict[i] = 0;
            }
            for (int i = 0; i < m_PropertyChangeEvent.Length; i++)
            {
                m_PropertyChangeEvent[i] = null;
            }
            for (int i = 0; i < m_ValueAgainCalc.Length; i++)
            {
                m_ValueAgainCalc[i] = false;
            }
            UnRegisterPropChangeEvent(UnitPropType.MaxHp, MaxHpChange);
            UnRegisterPropChangeEvent(UnitPropType.MaxMp, MaxMpChange);
        }
    }
}