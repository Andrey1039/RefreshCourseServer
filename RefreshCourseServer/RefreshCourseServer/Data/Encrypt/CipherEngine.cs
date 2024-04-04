using System.Text;
using System.Security.Cryptography;
using System.Globalization;

namespace RefreshCourseServer.Data.Encrypt
{
    public class CipherEngine
    {
        // Шифрование текста (AES256)
        public static string EncryptString(string originalText, string strKey)
        {
            string result = string.Empty;
            byte[] key = Convert.FromHexString(strKey);

            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                using (MemoryStream output = new MemoryStream())
                {
                    output.Write(aes.IV);

                    using (CryptoStream cryptoStream = new(output, aes.CreateEncryptor(), CryptoStreamMode.Write, true))
                    {
                        cryptoStream.Write(Encoding.UTF8.GetBytes(originalText));
                    }

                    result = Convert.ToBase64String(output.ToArray());
                }
            };

            return result;
        }

        // Расшифрование текста (AES256)
        public static string DecryptString(string chiperText, string strKey)
        {
            string result = string.Empty;
            byte[] key = Convert.FromHexString(strKey);

            using (MemoryStream input = new MemoryStream(Convert.FromBase64String(chiperText)))
            {
                byte[] initVector = new byte[16];
                input.Read(initVector);

                using (Aes aes = Aes.Create())
                {
                    aes.Key = key;
                    aes.IV = initVector;

                    using (CryptoStream cryptoStream = new CryptoStream(input, aes.CreateDecryptor(), CryptoStreamMode.Read, true))
                    {
                        using (MemoryStream output = new MemoryStream())
                        {
                            cryptoStream.CopyTo(output);
                            result = Encoding.UTF8.GetString(output.ToArray());
                        }
                    }
                }
            }

            return result;
        }
    }
}