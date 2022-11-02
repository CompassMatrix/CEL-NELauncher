using System;
using System.Security.Cryptography;
using System.Text;

namespace NeteaseLogin
{
	public class AESHelper
	{
		public static byte[] AESEncrypt128(byte[] data, byte[] keyBytes, byte[] ivBytes)
		{
			RijndaelManaged rijndaelManaged = new RijndaelManaged();
			int num = (int)(rijndaelManaged.Mode = CipherMode.CBC);
			int num2 = (int)(rijndaelManaged.Padding = PaddingMode.PKCS7);
			int num4 = (rijndaelManaged.KeySize = 128);
			int num6 = (rijndaelManaged.BlockSize = 128);
			byte[] array2 = (rijndaelManaged.Key = keyBytes);
			byte[] array4 = (rijndaelManaged.IV = ivBytes);
			return rijndaelManaged.CreateEncryptor().TransformFinalBlock(data, 0, data.Length);
		}

		public static byte[] GetIv(int n)
		{
			char[] array = new char[60]
			{
				'a', 'b', 'd', 'c', 'e', 'f', 'g', 'h', 'i', 'j',
				'k', 'l', 'm', 'n', 'p', 'r', 'q', 's', 't', 'u',
				'v', 'w', 'z', 'y', 'x', '0', '1', '2', '3', '4',
				'5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E',
				'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'Q',
				'P', 'R', 'T', 'S', 'V', 'U', 'W', 'X', 'Y', 'Z'
			};
			StringBuilder stringBuilder = new StringBuilder();
			Random random = new Random(DateTime.Now.Millisecond);
			for (int i = 0; i < n; i++)
			{
				stringBuilder.Append(array[random.Next(0, array.Length)].ToString());
			}
			return Encoding.UTF8.GetBytes(stringBuilder.ToString());
		}

		public static byte[] AESDecrypt128(byte[] data, byte[] keyBytes, byte[] ivBytes)
		{
			RijndaelManaged rijndaelManaged = new RijndaelManaged();
			int num = (int)(rijndaelManaged.Mode = CipherMode.CBC);
			int num2 = (int)(rijndaelManaged.Padding = PaddingMode.PKCS7);
			int num4 = (rijndaelManaged.KeySize = 128);
			int num6 = (rijndaelManaged.BlockSize = 128);
			byte[] array2 = (rijndaelManaged.Key = keyBytes);
			byte[] array4 = (rijndaelManaged.IV = ivBytes);
			return rijndaelManaged.CreateDecryptor().TransformFinalBlock(data, 0, data.Length);
		}

		public static byte[] AESEncrypt128Ex(byte[] data, byte[] keyBytes, byte[] ivBytes)
		{
			RijndaelManaged rijndaelManaged = new RijndaelManaged();
			int num = (int)(rijndaelManaged.Mode = CipherMode.CBC);
			int num2 = (int)(rijndaelManaged.Padding = PaddingMode.Zeros);
			int num4 = (rijndaelManaged.KeySize = 128);
			int num6 = (rijndaelManaged.BlockSize = 128);
			byte[] array2 = (rijndaelManaged.Key = keyBytes);
			byte[] array4 = (rijndaelManaged.IV = ivBytes);
			return rijndaelManaged.CreateEncryptor().TransformFinalBlock(data, 0, data.Length);
		}
	}
}
