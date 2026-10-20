using System.Security.Cryptography;
using System.Text;

namespace SQLChecker.Models.Services
{
    public class Encryptor
    {
        private const int SaltSize = 32;
        private const int HashSize = 64;
        private const int Iterations = 200000;

        public static string HashPassword(string password)
        {
            using var rng = RandomNumberGenerator.Create();
            byte[] salt = new byte[SaltSize];
            rng.GetBytes(salt);

            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256);
            byte[] hash = pbkdf2.GetBytes(HashSize);

            return $"{Iterations}:{Convert.ToBase64String(salt)}:{Convert.ToBase64String(hash)}";
        }

        public static bool VerifyPassword(string password, string storedHash)
        {
            var parts = storedHash.Split(':');
            if (parts.Length != 3) return false;

            int iterations = int.Parse(parts[0]);
            byte[] salt = Convert.FromBase64String(parts[1]);
            byte[] hash = Convert.FromBase64String(parts[2]);

            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256);
            byte[] newHash = pbkdf2.GetBytes(HashSize);

            return CryptographicOperations.FixedTimeEquals(newHash, hash);
        }

        public class UserTokenEncryptor
        {
            private static readonly byte[] FixedSalt = Encoding.UTF8.GetBytes("OBC-FIXED-SALT-1234");
            private readonly byte[] key;

            public UserTokenEncryptor(string secret)
            {
                if (string.IsNullOrEmpty(secret))
                    throw new ArgumentNullException(nameof(secret));

                using var derive = new Rfc2898DeriveBytes(secret, FixedSalt, 100_000, HashAlgorithmName.SHA256);

                key = derive.GetBytes(32); // AES-256 key
            }

            public string EncryptPayload(string json)
            {
                using var aes = Aes.Create();
                aes.Key = key;
                aes.GenerateIV();

                using var encryptor = aes.CreateEncryptor();

                var plainBytes = Encoding.UTF8.GetBytes(json);
                var cipherBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

                // [IV | ciphertext]
                var result = new byte[aes.IV.Length + cipherBytes.Length];
                Buffer.BlockCopy(aes.IV, 0, result, 0, aes.IV.Length);
                Buffer.BlockCopy(cipherBytes, 0, result, aes.IV.Length, cipherBytes.Length);

                return Convert.ToBase64String(result);
            }

            public string DecryptPayload(string base64)
            {
                var fullData = Convert.FromBase64String(base64);

                var iv = fullData[..16];
                var cipher = fullData[16..];

                using var aes = Aes.Create();
                aes.Key = key;
                aes.IV = iv;

                using var decryptor = aes.CreateDecryptor();

                var plainBytes = decryptor.TransformFinalBlock(cipher, 0, cipher.Length);

                return Encoding.UTF8.GetString(plainBytes);
            }
        }

        public static string ComputeUserHash(string email, string course, string secret)
        {
            var combined = $"{email}-{course}";
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(combined));
            return Convert.ToHexString(hash).ToLowerInvariant();
        }
        public static bool IsTimestampValid(long ts, int maxAgeSeconds = 300)
        {
            long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            return Math.Abs(now - ts) <= maxAgeSeconds;
        }
        
    }
}
