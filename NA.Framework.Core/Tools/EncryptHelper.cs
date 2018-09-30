using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace NA.Framework.Core.Tools
{
    public class EncryptHelper
    {
        #region Aes算法

        /// <summary>
        /// AES加密
        /// </summary>
        /// <param name="rawText"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string AesEncrypt(string rawText, string key)
        {
            if (key?.Length != 32)
                throw new ArgumentException("密钥长度必须为32", nameof(key));
            if (rawText == null)
                return null;

            byte[] textBytes = Encoding.UTF8.GetBytes(rawText);
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);

            var mode = CipherMode.ECB;
            var padding = PaddingMode.PKCS7;

            var aesCrypt = Aes.Create();
            aesCrypt.Key = keyBytes;
            aesCrypt.Mode = mode;
            aesCrypt.Padding = padding;

            ICryptoTransform transform = aesCrypt.CreateEncryptor();
            byte[] block = transform.TransformFinalBlock(textBytes, 0, textBytes.Length);
            string result = Convert.ToBase64String(block, 0, block.Length);
            return result;
        }

        /// <summary>
        /// AES解密
        /// </summary>
        /// <param name="content"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string AesDecrypt(string encryptText, string key)
        {
            if (key?.Length != 32)
                throw new ArgumentException("密钥长度必须为32", nameof(key));
            if (encryptText == null)
                return null;

            byte[] textBytes = Encoding.UTF8.GetBytes(encryptText);
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);

            var mode = CipherMode.ECB;
            var padding = PaddingMode.PKCS7;

            var aesCrypt = Aes.Create();
            aesCrypt.Key = keyBytes;
            aesCrypt.Mode = mode;
            aesCrypt.Padding = padding;

            ICryptoTransform transform = aesCrypt.CreateDecryptor(keyBytes, null);
            using (MemoryStream ms = new MemoryStream(textBytes))
            {
                CryptoStream cs = new CryptoStream(ms, transform, CryptoStreamMode.Read);
                StreamReader sr = new StreamReader(cs);
                string result = sr.ReadToEnd();
                return result;
            }
        }

        #endregion

        #region Des算法

        /// <summary>
        /// DES加密
        /// </summary>
        /// <param name="rawText"></param>
        /// <param name="key"></param>
        /// <param name="iv"></param>
        /// <returns></returns>
        public static string DesEncrypt(string rawText, string key, string iv)
        {
            if (key?.Length != 8)
                throw new ArgumentException("密钥长度必须为8", nameof(key));
            if (iv?.Length != 8)
                throw new ArgumentException("IV长度必须为8", nameof(iv));
            if (rawText == null)
                return null;

            using (DESCryptoServiceProvider provider = new DESCryptoServiceProvider { Key = Encoding.UTF8.GetBytes(key), IV = Encoding.UTF8.GetBytes(iv) })
            {
                using (ICryptoTransform transform = provider.CreateEncryptor())
                {
                    byte[] bytes = Encoding.UTF8.GetBytes(rawText);
                    using (var ms = new MemoryStream())
                    {
                        using (var cs = new CryptoStream(ms, transform, CryptoStreamMode.Write))
                        {
                            cs.Write(bytes, 0, bytes.Length);
                            cs.FlushFinalBlock();
                        }
                        return Convert.ToBase64String(ms.ToArray());
                    }
                }
            }
        }

        /// <summary>
        /// DES解密
        /// </summary>
        /// <param name="encryptText"></param>
        /// <param name="key"></param>
        /// <param name="iv"></param>
        /// <returns></returns>
        public static string DesDecrypt(string encryptText, string key, string iv)
        {
            if (key?.Length != 8)
                throw new ArgumentException("密钥长度必须为8", nameof(key));
            if (iv?.Length != 8)
                throw new ArgumentException("IV长度必须为8", nameof(iv));
            if (encryptText == null)
                return null;

            using (DESCryptoServiceProvider provider = new DESCryptoServiceProvider { Key = Encoding.UTF8.GetBytes(key), IV = Encoding.UTF8.GetBytes(iv) })
            {
                using (ICryptoTransform transform = provider.CreateDecryptor())
                {
                    byte[] bytes = Encoding.UTF8.GetBytes(encryptText);
                    using (var ms = new MemoryStream())
                    {
                        using (var cs = new CryptoStream(ms, transform, CryptoStreamMode.Write))
                        {
                            cs.Write(bytes, 0, bytes.Length);
                            cs.FlushFinalBlock();
                        }
                        return Convert.ToBase64String(ms.ToArray());
                    }
                }
            }
        }

        #endregion

        #region Rsa算法

        /// <summary>
        /// RSA验证签名
        /// </summary>
        /// <param name="rsaType"></param>
        /// <param name="publicKey"></param>
        /// <param name="data"></param>
        /// <param name="dataEncoding"></param>
        /// <param name="sign"></param>
        /// <returns></returns>
        public static bool RsaVerify(ERsaType rsaType, string publicKey, string data, Encoding dataEncoding, string sign)
        {
            byte[] dataBytes = dataEncoding.GetBytes(data);
            byte[] signBytes = Convert.FromBase64String(sign);
            var algorithm = rsaType == ERsaType.RSA ? HashAlgorithmName.SHA1 : HashAlgorithmName.SHA256;

            var rsa = CreateRsaFromPublicKey(publicKey);
            if (rsa == null)
                return false;

            var verify = rsa.VerifyData(dataBytes, signBytes, algorithm, RSASignaturePadding.Pkcs1);
            return verify;
        }

        /// <summary>
        /// RSA签名
        /// </summary>
        /// <param name="rsaType"></param>
        /// <param name="privateKey"></param>
        /// <param name="data"></param>
        /// <param name="dataEncoding"></param>
        /// <returns></returns>
        public static string RsaSign(ERsaType rsaType, string privateKey, string data, Encoding dataEncoding)
        {
            byte[] dataBytes = dataEncoding.GetBytes(data);
            var algorithm = rsaType == ERsaType.RSA ? HashAlgorithmName.SHA1 : HashAlgorithmName.SHA256;

            var rsa = CreateRsaFromPrivateKey(privateKey);
            if (rsa == null)
                return null;

            var signBytes = rsa.SignData(dataBytes, algorithm, RSASignaturePadding.Pkcs1);
            return Convert.ToBase64String(signBytes);
        }

        /// <summary>
        /// RSA加密
        /// </summary>
        /// <param name="text"></param>
        /// <param name="encoding"></param>
        /// <param name="publicKey"></param>
        /// <returns></returns>
        public static string RsaEncrypt(string text, Encoding encoding, string publicKey)
        {
            var rsa = CreateRsaFromPublicKey(publicKey);
            if (rsa == null)
                return null;

            return Convert.ToBase64String(rsa.Encrypt(encoding.GetBytes(text), RSAEncryptionPadding.Pkcs1));
        }

        /// <summary>
        /// RSA解密
        /// </summary>
        /// <param name="encryptText"></param>
        /// <param name="encoding"></param>
        /// <param name="privateKey"></param>
        /// <returns></returns>
        public static string RsaDecrypt(string encryptText, Encoding encoding, string privateKey)
        {
            var rsa = CreateRsaFromPrivateKey(privateKey);
            if (rsa == null)
                return null;

            return encoding.GetString(rsa.Decrypt(Convert.FromBase64String(encryptText), RSAEncryptionPadding.Pkcs1));
        }

        private static RSA CreateRsaFromPublicKey(string publicKey)
        {
            byte[] seqOID = { 0x30, 0x0D, 0x06, 0x09, 0x2A, 0x86, 0x48, 0x86, 0xF7, 0x0D, 0x01, 0x01, 0x01, 0x05, 0x00 };
            byte[] seq = new byte[15];

            byte[] x509key = Convert.FromBase64String(publicKey);

            using (var ms = new MemoryStream(x509key))
            {
                using (var binReader = new BinaryReader(ms))
                {
                    byte bt = 0;
                    ushort twoBytes = 0;

                    twoBytes = binReader.ReadUInt16();
                    if (twoBytes == 0x8130)
                        binReader.ReadByte();
                    else if (twoBytes == 0x8230)
                        binReader.ReadInt16();
                    else
                        return null;

                    seq = binReader.ReadBytes(15);
                    if (!CompareByteArrays(seq, seqOID))
                        return null;

                    twoBytes = binReader.ReadUInt16();
                    if (twoBytes == 0x8103)
                        binReader.ReadByte();
                    else if (twoBytes == 0x8203)
                        binReader.ReadInt16();
                    else
                        return null;

                    bt = binReader.ReadByte();
                    if (bt != 0x00)
                        return null;

                    twoBytes = binReader.ReadUInt16();
                    if (twoBytes == 0x8130)
                        binReader.ReadByte();
                    else if (twoBytes == 0x8230)
                        binReader.ReadInt16();
                    else
                        return null;

                    twoBytes = binReader.ReadUInt16();
                    byte lowbyte = 0x00;
                    byte highbyte = 0x00;

                    if (twoBytes == 0x8102)
                        lowbyte = binReader.ReadByte();
                    else if (twoBytes == 0x8202)
                    {
                        highbyte = binReader.ReadByte();
                        lowbyte = binReader.ReadByte();
                    }
                    else
                        return null;
                    byte[] modint = { lowbyte, highbyte, 0x00, 0x00 };
                    int modsize = BitConverter.ToInt32(modint, 0);

                    int firstbyte = binReader.PeekChar();
                    if (firstbyte == 0x00)
                    {
                        binReader.ReadByte();
                        modsize -= 1;
                    }

                    byte[] modulus = binReader.ReadBytes(modsize);

                    if (binReader.ReadByte() != 0x02)
                        return null;
                    int expBytes = (int)binReader.ReadByte();
                    byte[] exponent = binReader.ReadBytes(expBytes);

                    var rsa = RSA.Create();
                    var rsaKeyInfo = new RSAParameters
                    {
                        Modulus = modulus,
                        Exponent = exponent
                    };
                    rsa.ImportParameters(rsaKeyInfo);
                    return rsa;
                }
            }
        }

        private static RSA CreateRsaFromPrivateKey(string privateKey)
        {
            var privateKeyBytes = Convert.FromBase64String(privateKey);

            var rsa = RSA.Create();
            var rsaParams = new RSAParameters();

            using (var binReader = new BinaryReader(new MemoryStream(privateKeyBytes)))
            {
                byte bt = 0;
                ushort twobytes = 0;
                twobytes = binReader.ReadUInt16();
                if (twobytes == 0x8130)
                    binReader.ReadByte();
                else if (twobytes == 0x8230)
                    binReader.ReadInt16();
                else
                    throw new Exception("Unexpected value read binr.ReadUInt16()");

                twobytes = binReader.ReadUInt16();
                if (twobytes != 0x0102)
                    throw new Exception("Unexpected version");

                bt = binReader.ReadByte();
                if (bt != 0x00)
                    throw new Exception("Unexpected value read binr.ReadByte()");

                rsaParams.Modulus = binReader.ReadBytes(GetIntegerSize(binReader));
                rsaParams.Exponent = binReader.ReadBytes(GetIntegerSize(binReader));
                rsaParams.D = binReader.ReadBytes(GetIntegerSize(binReader));
                rsaParams.P = binReader.ReadBytes(GetIntegerSize(binReader));
                rsaParams.Q = binReader.ReadBytes(GetIntegerSize(binReader));
                rsaParams.DP = binReader.ReadBytes(GetIntegerSize(binReader));
                rsaParams.DQ = binReader.ReadBytes(GetIntegerSize(binReader));
                rsaParams.InverseQ = binReader.ReadBytes(GetIntegerSize(binReader));
            }

            rsa.ImportParameters(rsaParams);
            return rsa;
        }

        private static bool CompareByteArrays(byte[] a, byte[] b)
        {
            if (a.Length != b.Length)
                return false;

            int i = 0;
            foreach (byte c in a)
            {
                if (c != b[i])
                    return false;
                i++;
            }
            return true;
        }

        private static int GetIntegerSize(BinaryReader binReader)
        {
            byte bt = 0;
            int count = 0;
            bt = binReader.ReadByte();
            if (bt != 0x02)
                return 0;
            bt = binReader.ReadByte();

            if (bt == 0x81)
                count = binReader.ReadByte();
            else if (bt == 0x82)
            {
                byte highbyte = binReader.ReadByte();
                byte lowbyte = binReader.ReadByte();
                byte[] modint = { lowbyte, highbyte, 0x00, 0x00 };
                count = BitConverter.ToInt32(modint, 0);
            }
            else
            {
                count = bt;
            }

            while (binReader.ReadByte() == 0x00)
            {
                count -= 1;
            }
            binReader.BaseStream.Seek(-1, SeekOrigin.Current);
            return count;
        }

        #endregion
    }

    public enum ERsaType
    {
        /// <summary>
        /// SHA1
        /// </summary>
        RSA,
        /// <summary>
        /// SHA256
        /// </summary>
        RSA2
    }
}
