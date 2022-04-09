using GameFramework;
using GameFramework.DataTable;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace CustomGame
{
    /// <summary>
     /// 默认Json数据表辅助器。
     /// </summary>
    public class DataTableJsonHelper : DataTableHelperBase
    {
        private static readonly string BytesAssetExtension = ".bytes";

        private ResourceComponent m_ResourceComponent = null;

        /// <summary>
        /// 读取数据表。
        /// </summary>
        /// <param name="dataTable">数据表。</param>
        /// <param name="dataTableAssetName">数据表资源名称。</param>
        /// <param name="dataTableAsset">数据表资源。</param>
        /// <param name="userData">用户自定义数据。</param>
        /// <returns>是否读取数据表成功。</returns>
        public override bool ReadData(DataTableBase dataTable, string dataTableAssetName, object dataTableAsset, object userData)
        {
            TextAsset dataTableTextAsset = dataTableAsset as TextAsset;
            if (dataTableTextAsset != null)
            {
                if (dataTableAssetName.EndsWith(BytesAssetExtension, StringComparison.Ordinal))
                {
                    return dataTable.ParseData(dataTableTextAsset.bytes, userData);
                }
                else
                {
                    return dataTable.ParseData(dataTableTextAsset.text, userData);
                }
            }

            Log.Warning("Data table asset '{0}' is invalid.", dataTableAssetName);
            return false;
        }

        /// <summary>
        /// 读取数据表。
        /// </summary>
        /// <param name="dataTable">数据表。</param>
        /// <param name="dataTableAssetName">数据表资源名称。</param>
        /// <param name="dataTableBytes">数据表二进制流。</param>
        /// <param name="startIndex">数据表二进制流的起始位置。</param>
        /// <param name="length">数据表二进制流的长度。</param>
        /// <param name="userData">用户自定义数据。</param>
        /// <returns>是否读取数据表成功。</returns>
        public override bool ReadData(DataTableBase dataTable, string dataTableAssetName, byte[] dataTableBytes, int startIndex, int length, object userData)
        {
            if (dataTableAssetName.EndsWith(BytesAssetExtension, StringComparison.Ordinal))
            {
                return dataTable.ParseData(dataTableBytes, startIndex, length, userData);
            }
            else
            {
                return dataTable.ParseData(Utility.Converter.GetString(dataTableBytes, startIndex, length), userData);
            }
        }

        /// <summary>
        /// 解析数据表。
        /// </summary>
        /// <param name="dataTable">数据表。</param>
        /// <param name="dataTableString">要解析的数据表字符串。</param>
        /// <param name="userData">用户自定义数据。</param>
        /// <returns>是否解析数据表成功。</returns>
        public override bool ParseData(DataTableBase dataTable, string dataTableString, object userData)
        {
            int count = 0;
            int offset = 0;
            int length = 0;
            int index = 0;
            while (index < dataTableString.Length)
            {
                switch (dataTableString[index])
                {
                    case '{':
                        if (count == 0)
                        {
                            offset = index;
                        }
                        count++;
                        break;
                    case '}':
                        count--;
                        if (count == 0)
                        {
                            length++;
                            dataTable.AddDataRow(dataTableString.Substring(offset, length), userData);
                            length = 0;
                        }
                        else if (count < 0)
                        {
                            Log.Error("出错了，数据书写有误");
                            return false;
                        }
                        break;
                }
                if (count > 0)
                {
                    length++;
                }
                index++;
            }
            return true;
        }

        /// <summary>
        /// 解析数据表。
        /// </summary>
        /// <param name="dataTable">数据表。</param>
        /// <param name="dataTableBytes">要解析的数据表二进制流。</param>
        /// <param name="startIndex">数据表二进制流的起始位置。</param>
        /// <param name="length">数据表二进制流的长度。</param>
        /// <param name="userData">用户自定义数据。</param>
        /// <returns>是否解析数据表成功。</returns>
        public override bool ParseData(DataTableBase dataTable, byte[] dataTableBytes, int startIndex, int length, object userData)
        {
            try
            {
                using (MemoryStream memoryStream = new MemoryStream(dataTableBytes, startIndex, length, false))
                {
                    using (BinaryReader binaryReader = new BinaryReader(memoryStream, Encoding.UTF8))
                    {
                        while (binaryReader.BaseStream.Position < binaryReader.BaseStream.Length)
                        {
                            int dataRowBytesLength = binaryReader.Read7BitEncodedInt32();
                            if (!dataTable.AddDataRow(dataTableBytes, (int)binaryReader.BaseStream.Position, dataRowBytesLength, userData))
                            {
                                Log.Warning("Can not parse data row bytes.");
                                return false;
                            }

                            binaryReader.BaseStream.Position += dataRowBytesLength;
                        }
                    }
                }

                return true;
            }
            catch (Exception exception)
            {
                Log.Warning("Can not parse dictionary bytes with exception '{0}'.", exception.ToString());
                return false;
            }
        }

        /// <summary>
        /// 释放数据表资源。
        /// </summary>
        /// <param name="dataTable">数据表。</param>
        /// <param name="dataTableAsset">要释放的数据表资源。</param>
        public override void ReleaseDataAsset(DataTableBase dataTable, object dataTableAsset)
        {
            m_ResourceComponent.UnloadAsset(dataTableAsset);
        }

        private void Start()
        {
            m_ResourceComponent = UnityGameFramework.Runtime.GameEntry.GetComponent<ResourceComponent>();
            if (m_ResourceComponent == null)
            {
                Log.Fatal("Resource component is invalid.");
                return;
            }
        }
    }
}