using System;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace coinoneAlarm.Sources.Common
{
    class EncryptData
    {
        // encKey - to pbkdf
        private static readonly byte[] encKey = { 0xb3, 0x23, 0x5f, 0xfa, 0x15, 0x7a, 0x9d, 0xdf, 0x11, 0x56, 0x27, 0xa8, 0xb5, 0x59, 0x7c, 0x2b };

        // random string - to pbkdf
        private string strRan;
        
        private string encryptedKey;
        private string encryptedToken;

        private int GenerateRandomNumber()
        {
            try
            {
                Random random = new Random();

                return random.Next(32, 64);
            }
            catch
            {
                return 32;
            }
        }

        public void GenerateRandomString()
        {
            try
            {
                int len = GenerateRandomNumber();
                char[] charArray = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789".ToCharArray();
                string strRandom = string.Empty;

                Random random = new Random();

                for (int i = 0; i < len; i++ )
                {
                    strRandom += charArray[random.Next(0, 61)].ToString();
                }
                strRan =  strRandom;
            }
            catch
            {
                strRan = string.Empty;
            }
        }

        public void EncryptString(string plainText, bool isKey)
        {
            try
            {
                string cipherText = string.Empty;

                //plaintext string to byte array
                byte[] plainTextBytes = Encoding.Unicode.GetBytes(plainText);

                //AES 128 Encryption
                using (Aes encryptor = Aes.Create())
                {
                    //generate real key - pbkdf
                    Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(strRan, encKey);

                    //get key, IV
                    encryptor.Key = pdb.GetBytes(32);
                    encryptor.IV = pdb.GetBytes(16);

                    //enc action
                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                        {
                            cs.Write(plainTextBytes, 0, plainTextBytes.Length);
                            cs.Close();
                        }
                        cipherText = Convert.ToBase64String(ms.ToArray());
                    }
                }
                if (isKey == true) encryptedKey = cipherText;
                else encryptedToken = cipherText;
            }
            catch
            {
                if (isKey == true) encryptedKey = plainText;
                else encryptedToken = plainText;
            }
        }

        public string DecryptString(bool isKey)
        {
            try
            {
                string plainText = string.Empty;

                //ciphertext string to byte array
                byte[] cipherTextBytes = Convert.FromBase64String((isKey == true) ? encryptedKey : encryptedToken);

                //AES 128 Decryption
                using (Aes encryptor = Aes.Create())
                {
                    //generate real key - pbkdf
                    Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(strRan, encKey);

                    //get key, IV
                    encryptor.Key = pdb.GetBytes(32);
                    encryptor.IV = pdb.GetBytes(16);

                    //dec action
                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                        {
                            cs.Write(cipherTextBytes, 0, cipherTextBytes.Length);
                            cs.Close();
                        }
                        plainText = Encoding.Unicode.GetString(ms.ToArray());
                    }
                }
                return plainText;
            }
            catch
            {
                return (isKey == true) ? encryptedKey : encryptedToken;
            }
        }

        public string ComputeSha512(string strInput)
        {
            SHA512 sha512 = new SHA512Managed();
            
            return Encoding.Default.GetString(sha512.ComputeHash(Encoding.UTF8.GetBytes(strInput.ToUpper())));
        }

        public string ComputeHmacSha512(string strInput, string strKey)
        {
            HMACSHA512 hmacSha512 = new HMACSHA512(Encoding.UTF8.GetBytes(strKey.ToUpper()));

            return Encoding.Default.GetString(hmacSha512.ComputeHash(Encoding.UTF8.GetBytes(strInput.ToUpper())));
        }

    }
}
