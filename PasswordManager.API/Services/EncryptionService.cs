
using System.Security.Cryptography;
using System.Text;

namespace PasswordManager.API.Services
{
    public class EncryptionService
    {
        private readonly IConfiguration _configuration;

        public EncryptionService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private byte[] GetValidKey()
        {
            var keyString = _configuration["Encryption:Key"]!;
            var keyBytes = Encoding.UTF8.GetBytes(keyString);

            // Upewnij się, że klucz ma dokładnie 32 bajty (256 bitów)
            var validKey = new byte[32];

            if (keyBytes.Length >= 32)
            {
                Array.Copy(keyBytes, validKey, 32);
            }
            else
            {
                Array.Copy(keyBytes, validKey, keyBytes.Length);
                // Wypełnij resztę zerami
                for (int i = keyBytes.Length; i < 32; i++)
                {
                    validKey[i] = 0;
                }
            }

            return validKey;
        }

        public string Encrypt(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
                throw new ArgumentException("Plain text cannot be null or empty");

            var key = GetValidKey();

            using var aes = Aes.Create();
            aes.Key = key;
            aes.GenerateIV();

            using var encryptor = aes.CreateEncryptor();
            var plainBytes = Encoding.UTF8.GetBytes(plainText);
            var encryptedBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

            // Połącz IV z zaszyfrowanymi danymi
            var result = new byte[aes.IV.Length + encryptedBytes.Length];
            Array.Copy(aes.IV, 0, result, 0, aes.IV.Length);
            Array.Copy(encryptedBytes, 0, result, aes.IV.Length, encryptedBytes.Length);

            return Convert.ToBase64String(result);
        }

        public string Decrypt(string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText))
                throw new ArgumentException("Cipher text cannot be null or empty");

            try
            {
                var fullCipher = Convert.FromBase64String(cipherText);
                var key = GetValidKey();

                using var aes = Aes.Create();
                aes.Key = key;

                // Wyodrębnij IV (pierwsze 16 bajtów)
                var iv = new byte[16];
                var cipher = new byte[fullCipher.Length - 16];

                Array.Copy(fullCipher, 0, iv, 0, 16);
                Array.Copy(fullCipher, 16, cipher, 0, cipher.Length);

                aes.IV = iv;

                using var decryptor = aes.CreateDecryptor();
                var decryptedBytes = decryptor.TransformFinalBlock(cipher, 0, cipher.Length);

                return Encoding.UTF8.GetString(decryptedBytes);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to decrypt data", ex);
            }
        }
    }
}