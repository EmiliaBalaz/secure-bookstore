namespace secure_bookstore.Services
{
    public interface IEncryptService
    {
        byte[] EncryptPassword(string password);
        string DescryptPassword(byte[] cipheredtext, byte[] key, byte[] IV);
        string EncodePassword(string password);
        string DecodePassword(string encodedData);
    }
}