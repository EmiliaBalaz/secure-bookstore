using System.Security.Cryptography;
using System.Text;

namespace secure_bookstore.Services
{
    public class EncryptService : IEncryptService
    {
        private static byte[] key = new byte[16];
        private static byte[] IV = new byte[16];
        public byte[] EncryptPassword(string password)
        {
            byte[] cipheredtext;
            
            using(RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {   rng.GetBytes(key);
                rng.GetBytes(IV);
            }
            
            using(Aes aes = Aes.Create())
            {
                ICryptoTransform encryptor = aes.CreateEncryptor(key, IV);
                using(MemoryStream ms = new MemoryStream())
                {
                    using(CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        using(StreamWriter sw = new StreamWriter(cs))
                        {
                            sw.Write(password);
                        }
                        cipheredtext = ms.ToArray();
                    }
                }
            }
            return cipheredtext;
        }

        public string DescryptPassword(byte[] cipheredtext, byte[] key, byte[] IV)
        {
            string simpletext = String.Empty;
            using(Aes aes = Aes.Create())
            {
                ICryptoTransform decryptor = aes.CreateDecryptor(key, IV);
                using(MemoryStream ms = new MemoryStream(cipheredtext))
                {
                    using(CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                    {
                        using(StreamReader sr = new StreamReader(cs))
                        {
                            simpletext = sr.ReadToEnd();
                        }
                    }
                }
            }
            return simpletext;
        }

        public string EncodePassword(string password)
        {
            try
            {
                string EncryptionKey = "EJMMDJDINA45003";
                byte[] clearBytes = Encoding.Unicode.GetBytes(password);
                using (Aes encryptor = Aes.Create())
                {
                    Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[]{0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x76});
                    encryptor.Key = pdb.GetBytes(32);
                    encryptor.IV = pdb.GetBytes(16);
                    using(MemoryStream ms = new MemoryStream())
                    {
                        using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                        {
                            cs.Write(clearBytes, 0, clearBytes.Length);
                            cs.Close();
                        }
                        password = Convert.ToBase64String(ms.ToArray());
                    }
                }
                return password;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }   
        public string DecodePassword(string encodedData)
        {
            string EncryptionKey = "EJMMDJDINA45003";
            byte[] cipheredBytes = Convert.FromBase64String(encodedData);
            using(Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[]{0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x76});
                encryptor.Key = pdb.GetBytes(32);
                    encryptor.IV = pdb.GetBytes(16);
                    using(MemoryStream ms = new MemoryStream())
                    {
                        using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                        {
                            cs.Write(cipheredBytes, 0, cipheredBytes.Length);
                            cs.Close();
                        }
                        encodedData = Convert.ToBase64String(ms.ToArray());
                    }
            }
            return encodedData;
        }   
    }
}