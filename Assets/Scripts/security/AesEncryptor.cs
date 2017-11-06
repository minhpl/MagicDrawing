using UnityEngine;
using System;
using System.Text;
using System.Security.Cryptography;
using System.Linq;

public static class AesEncryptor
{
	const int keyLength = 32;
	const int IvLength = 16;
	static readonly UTF8Encoding encoder;
	static readonly AesManaged aes;

	static AesEncryptor()
	{
		encoder = new UTF8Encoding();
		aes = new AesManaged ();
		aes.BlockSize = IvLength * 8; // only the 128-bit block size is specified in the AES standard.
		aes.Mode = CipherMode.CBC;
		aes.Padding = PaddingMode.PKCS7;
	}

	public static string encode(string sbuffer, string skey, string sIV)
	{
		byte[] buffer = encoder.GetBytes (sbuffer);

		byte[] key = encoder.GetBytes (skey);
		byte[] IV = encoder.GetBytes (sIV);
		using (ICryptoTransform encryptor = aes.CreateEncryptor(key, IV))
		{
			byte [] result =  encryptor.TransformFinalBlock(buffer, 0, buffer.Length);
			return Convert.ToBase64String (result);
		}
	}

	public static string decode(string sbuffer, string skey, string sIV)
	{
		byte[] buffer = Convert.FromBase64String(sbuffer);
		byte[] key = encoder.GetBytes (skey);
		byte[] IV = encoder.GetBytes (sIV);
		using (ICryptoTransform decryptor = aes.CreateDecryptor(key, IV))
		{
			byte[] result = decryptor.TransformFinalBlock (buffer, 0, buffer.Length);
			return encoder.GetString(result, 0, result.Length);
		}
	}
}