using System;
using System.Text;
using System.Security.Cryptography;
using System.IO;
namespace FantasyDiary
{
	public static class TextTool
	{
	    /// <summary>
	    /// 默认的密钥
	    /// </summary>
	    public const string DefaultKey = "WEQEQW2837XCIEMDJCIEKSO2931K2I3K";
	
	    /// <summary>
	    /// 返回加密的内容
	    /// </summary>
	    /// <param name="ContentInfo">加密版的信息</param>
	    /// <param name="secretKey">32位密钥Key</param>
	    /// <returns></returns>
	    public static string TextEncryption(string ContentInfo, string secretKey = DefaultKey)
	    {
	        byte[] keyArray = UTF8Encoding.UTF8.GetBytes(secretKey);
	        RijndaelManaged encryption = new RijndaelManaged
	        {
	            Key = keyArray,
	            Mode = CipherMode.ECB,
	            Padding = PaddingMode.PKCS7
	        };
	        ICryptoTransform cTransform = encryption.CreateEncryptor();
	        byte[] _EncryptArray = UTF8Encoding.UTF8.GetBytes(ContentInfo);
	        byte[] resultArray = cTransform.TransformFinalBlock(_EncryptArray, 0, _EncryptArray.Length);
	        return Convert.ToBase64String(resultArray, 0, resultArray.Length);
	    }
	
	    /// <summary>
	    /// 返回解密的内容
	    /// </summary>
	    /// <param name="encryptionContent">被加密的内容</param>
	    /// <param name="secretKey">32位密钥Key</param>
	    /// <returns></returns>
	    public static string TextDecoder(string encryptionContent, string secretKey = DefaultKey)
	    {
	        byte[] keyArray = UTF8Encoding.UTF8.GetBytes(secretKey);
	        RijndaelManaged decipher = new RijndaelManaged
	        {
	            Key = keyArray,
	            Mode = CipherMode.ECB,
	            Padding = PaddingMode.PKCS7
	        };
	        ICryptoTransform cTransform = decipher.CreateDecryptor();
	        byte[] _EncryptArray = Convert.FromBase64String(encryptionContent);
	        byte[] resultArray = cTransform.TransformFinalBlock(_EncryptArray, 0, _EncryptArray.Length);
	        return UTF8Encoding.UTF8.GetString(resultArray);
	    }
	    /// <summary>
	    /// 写入加密文本
	    /// </summary>
	    /// <param name="path">写入的路径</param>
	    /// <param name="ContentInfo">要加密的字符串信息</param>
	    /// <param name="secretKey">32位密钥Key</param>
	    public static void TextEncryptionWrite(string path, string ContentInfo, string secretKey = DefaultKey)
	    {
	        CheckBuildPaths(path);
	        File.WriteAllText(path, TextEncryption(ContentInfo, secretKey));
	    }
	    /// <summary>
	    /// 读取文本并解密
	    /// </summary>
	    /// <param name="path">文本路径</param>
	    /// <param name="secretKey">32位密钥Key</param>
	    /// <returns></returns>
	    public static string TextDecoderRead(string path, string secretKey = DefaultKey)
	    {   
	        return TextDecoder(File.ReadAllText(path), secretKey);
	    }
	
	    /// <summary>
	    /// 检查路径完整性
	    /// </summary>
	    /// <param name="path"></param>
	    static void CheckBuildPaths(string path)
	    {
	        path = Path.GetDirectoryName(path);
	        if (!Directory.Exists(path))
	        {
	            Directory.CreateDirectory(path);      
	        }
	    }
	}
}