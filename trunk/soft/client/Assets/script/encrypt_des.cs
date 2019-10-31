using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Text;
using Renci.SshNet.Security.Cryptography.Ciphers;
using Renci.SshNet.Security.Cryptography.Ciphers.Paddings;
using Renci.SshNet.Security.Cryptography.Ciphers.Modes;

public class encrypt_des {

	private static string ekey = "tsjhtsjh";
	private static string eiv = "51478543";

	public static byte[] encode(byte[] inputByteArray)
	{
		byte[] rgbKey = Encoding.UTF8.GetBytes(ekey);
		byte[] rgbIV = Encoding.UTF8.GetBytes(eiv);
		PKCS5Padding pp = new PKCS5Padding();
		byte[] pss = pp.Pad(8, inputByteArray);
		DesCipher cc = new DesCipher(rgbKey, new CbcCipherMode(rgbIV), null);
		return cc.Encrypt(pss);
	}
}
