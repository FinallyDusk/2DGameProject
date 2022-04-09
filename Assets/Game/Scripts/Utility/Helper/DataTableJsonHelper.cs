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
     /// Ĭ��Json���ݱ�������
     /// </summary>
    public class DataTableJsonHelper : DataTableHelperBase
    {
        private static readonly string BytesAssetExtension = ".bytes";

        private ResourceComponent m_ResourceComponent = null;

        /// <summary>
        /// ��ȡ���ݱ�
        /// </summary>
        /// <param name="dataTable">���ݱ�</param>
        /// <param name="dataTableAssetName">���ݱ���Դ���ơ�</param>
        /// <param name="dataTableAsset">���ݱ���Դ��</param>
        /// <param name="userData">�û��Զ������ݡ�</param>
        /// <returns>�Ƿ��ȡ���ݱ�ɹ���</returns>
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
        /// ��ȡ���ݱ�
        /// </summary>
        /// <param name="dataTable">���ݱ�</param>
        /// <param name="dataTableAssetName">���ݱ���Դ���ơ�</param>
        /// <param name="dataTableBytes">���ݱ����������</param>
        /// <param name="startIndex">���ݱ������������ʼλ�á�</param>
        /// <param name="length">���ݱ���������ĳ��ȡ�</param>
        /// <param name="userData">�û��Զ������ݡ�</param>
        /// <returns>�Ƿ��ȡ���ݱ�ɹ���</returns>
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
        /// �������ݱ�
        /// </summary>
        /// <param name="dataTable">���ݱ�</param>
        /// <param name="dataTableString">Ҫ���������ݱ��ַ�����</param>
        /// <param name="userData">�û��Զ������ݡ�</param>
        /// <returns>�Ƿ�������ݱ�ɹ���</returns>
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
                            Log.Error("�����ˣ�������д����");
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
        /// �������ݱ�
        /// </summary>
        /// <param name="dataTable">���ݱ�</param>
        /// <param name="dataTableBytes">Ҫ���������ݱ����������</param>
        /// <param name="startIndex">���ݱ������������ʼλ�á�</param>
        /// <param name="length">���ݱ���������ĳ��ȡ�</param>
        /// <param name="userData">�û��Զ������ݡ�</param>
        /// <returns>�Ƿ�������ݱ�ɹ���</returns>
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
        /// �ͷ����ݱ���Դ��
        /// </summary>
        /// <param name="dataTable">���ݱ�</param>
        /// <param name="dataTableAsset">Ҫ�ͷŵ����ݱ���Դ��</param>
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