using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Security.Cryptography;
using System.Text;
using System.IO;

namespace LiveWorld
{
    public class LWSecurity : MonoBehaviour
    {
        private static string aesKey = "k3NZ09bK8E4YU3mVKKnKvwmSycGXGWNr";
        private static string aesIV = "yZPyXIzs1SxOJnjo";

        public static string Encrypt(string text)
        {
            byte[] plaintextbytes = Encoding.ASCII.GetBytes(text);
            Aes aes = Aes.Create();
            aes.BlockSize = 128;
            aes.KeySize = 256;
            aes.Key = Encoding.ASCII.GetBytes(aesKey);
            aes.IV = Encoding.ASCII.GetBytes(aesIV);
            aes.Padding = PaddingMode.PKCS7;
            aes.Mode = CipherMode.CBC;
            ICryptoTransform crypto = aes.CreateEncryptor(aes.Key, aes.IV);
            byte[] encrypted = crypto.TransformFinalBlock(plaintextbytes, 0, plaintextbytes.Length);
            return Encoding.ASCII.GetString(encrypted);
        }
    }
    //-------------------------------------------------------------------------------------------------------
}
