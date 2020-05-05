using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace PS4_Tools.Util
{
    public class SCEUtil
    {
        /*CFW Propthet's Method This Method still relies on samu to be dumped*/
        public static bool sceSblSsMemcmpConsttime(byte[] a, byte[] b, int len, int offsetA = 0, int offsetB = 0)
        {
            for (int i = 0; i < len; i++)
            {
                if (a[i + offsetA] != b[i + offsetB])
                    return false;
            }

            return true;
        }

        public static bool sceSblSsMemcmpConsttime(byte[] a, string b, int len, int offsetA = 0)
        {
            return ASCIIEncoding.ASCII.GetString(a, offsetA, len) == b;
        }

        public static void sceSblSsMemset(byte[] buffer, byte val, int len)
        {
            for (int i = 0; i < len; i++)
            {
                buffer[i] = val;
            }
        }

        public static void Sha256Hmac(byte[] sha256HmacResult, byte[] enc, int datalen, byte[] sha256hmacKey, int keylen)
        {
            using (HMACSHA256 hmac = new HMACSHA256(sha256hmacKey))
            {
                var result = hmac.ComputeHash(enc, 0, datalen);
                Buffer.BlockCopy(result, 0, sha256HmacResult, 0, sha256HmacResult.Length);
            }
        }

        public static long sceSblSsDecryptSealedKey(byte[] enc, byte[] dec)
        {
            long errorCode = -2146499562;

            if (enc != null && dec != null)
            {
                errorCode = -2146499532;
                if (sceSblSsMemcmpConsttime(enc, "pfsSKKey", 8))
                {
                    errorCode = -2146499538;
                    byte[] sha256hmacKey = new byte[16];
                    short keyVersion = (short)enc[8];
                    Console.WriteLine("Usering Keyset Version {0}", keyVersion);
                    byte[] allKeybytes = null;
                    switch (keyVersion)
                    {
                        case 1:
                            allKeybytes = PS4Keys.KernelKeys.SealedKey.Keyset1.Hash;
                            break;
                        case 2:
                            allKeybytes = PS4Keys.KernelKeys.SealedKey.Keyset2.Hash;
                            break;
                        case 3:
                            allKeybytes = PS4Keys.KernelKeys.SealedKey.Keyset3.Hash;
                            break;
                        case 4:
                            allKeybytes = PS4Keys.KernelKeys.SealedKey.Keyset4.Hash;
                            break;
                        case 5:
                            allKeybytes = PS4Keys.KernelKeys.SealedKey.Keyset5.Hash;
                            break;
                        case 6:
                            allKeybytes = PS4Keys.KernelKeys.SealedKey.Keyset6.Hash;
                            break;
                        case 7:
                            allKeybytes = PS4Keys.KernelKeys.SealedKey.Keyset7.Hash;
                            break;
                        case 8:
                            allKeybytes = PS4Keys.KernelKeys.SealedKey.Keyset8.Hash;
                            break;

                    }
                    //var allKeybytes = File.ReadAllBytes(string.Format("savedatamasterhashkey{0}.bin", keyVersion));
                    Buffer.BlockCopy(allKeybytes, 0, sha256hmacKey, 0, 16);

                    byte[] sha256HmacResult = new byte[32];
                    Sha256Hmac(sha256HmacResult, enc, 0x40, sha256hmacKey, 0x10);
                    errorCode = -2146499531;
                    if (sceSblSsMemcmpConsttime(sha256HmacResult, enc, 32, 0, 64))
                    {
                        Console.WriteLine("HMAC Check... Success!");
                        byte[] iv = new byte[16];
                        Buffer.BlockCopy(enc, 16, iv, 0, iv.Length);

                        byte[] encryptedKey = new byte[32];
                        Buffer.BlockCopy(enc, 32, encryptedKey, 0, 32);

                        using (AesManaged aes = new AesManaged())
                        {
                            byte[] aesKey = new byte[16];
                            //allKeybytes = File.ReadAllBytes(string.Format("savedatamasterkey{0}.bin", keyVersion));

                            switch (keyVersion)
                            {
                                case 1:
                                    allKeybytes = PS4Keys.KernelKeys.SealedKey.Keyset1.Key;
                                    break;
                                case 2:
                                    allKeybytes = PS4Keys.KernelKeys.SealedKey.Keyset2.Key;
                                    break;
                                case 3:
                                    allKeybytes = PS4Keys.KernelKeys.SealedKey.Keyset3.Key;
                                    break;
                                case 4:
                                    allKeybytes = PS4Keys.KernelKeys.SealedKey.Keyset4.Key;
                                    break;
                                case 5:
                                    allKeybytes = PS4Keys.KernelKeys.SealedKey.Keyset5.Key;
                                    break;
                                case 6:
                                    allKeybytes = PS4Keys.KernelKeys.SealedKey.Keyset6.Key;
                                    break;
                                case 7:
                                    allKeybytes = PS4Keys.KernelKeys.SealedKey.Keyset7.Key;
                                    break;
                                case 8:
                                    allKeybytes = PS4Keys.KernelKeys.SealedKey.Keyset8.Key;
                                    break;

                            }

                            Buffer.BlockCopy(allKeybytes, 0, aesKey, 0, 16);

                            aes.Mode = CipherMode.CBC;
                            aes.IV = iv;
                            aes.KeySize = 128;
                            aes.Key = aesKey;
                            aes.Padding = PaddingMode.None;
                            var stream = new MemoryStream();
                            using (var decryptor = aes.CreateDecryptor())
                            {
                                using (var cryptoStream = new CryptoStream(stream, decryptor, CryptoStreamMode.Write))
                                {
                                    using (var writer = new BinaryWriter(cryptoStream))
                                    {
                                        writer.Write(encryptedKey);
                                    }
                                }
                            }
                            byte[] cipherBytes = stream.ToArray();
                            Buffer.BlockCopy(cipherBytes, 0, dec, 0, 32);
                        }
                        errorCode = 0;
                    }
                    else
                    {
                        Console.WriteLine("HMAC Check... Failure!");
                    }
                }
            }

            return errorCode;
        }
    }
}
