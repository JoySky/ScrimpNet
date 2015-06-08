using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace ScrimpNet.Cryptography
{
    /// <summary>
    /// Properties about a single crytographic key
    /// </summary>
    [Serializable]
    public class KeyFile
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        private KeyFile() { }

        internal KeyFile(byte[] key, byte[] IV)
        {
            _key = _key.Clone() as byte[];
            _iv = IV.Clone() as byte[];
        }

        private byte[] _key;
        /// <summary>
        /// Primary encryption key used for encryption.  Exact size is dependent on algorithm consuming these values
        /// </summary>
        public byte[] Key {
            get
            {
                if (IsEffective)
                {
                    return _key;
                }
                throw ExceptionFactory.New<InvalidOperationException>("This key file is valid only from {0:yyyy-MM-dd HH:mm:ss.ff} UTC to {1:yyyy-MM-dd HH:mm:ss.ff} UTC",
                    ValidFromUtc, ValidToUtc);
            }
            set
            {
                _key = value;
            }
        }

        /// <summary>
        /// Determines if this key file is effective at this instant in time
        /// </summary>
        public bool IsEffective
        {
            get
            {
                long utcTicks = DateTime.UtcNow.Ticks;
                if (utcTicks < ValidFromUtc.Ticks) return false;
                if (utcTicks > ValidToUtc.Ticks) return false;
                return true;
            }
        }

        private byte[] _iv;
        /// <summary>
        /// Primary offset (some algorithms call this 'InsertionVector').  Exact size is dependent on algorithm
        /// </summary>
        public byte[] IV
        {
            get
            {
                return _iv;
            }
            set
            {
                _iv = value;
            }
        }

        private DateTime _validFrom = Utils.Date.SqlMaxDate; // default to NOT effective

        /// <summary>
        /// Time (UTC) before which this key file is no longer valid
        /// </summary>
        public DateTime ValidFromUtc
        {
            get
            {
                return _validFrom;
            }
            set
            {
                _validFrom = value;
            }
        }

        private DateTime _validTo = Utils.Date.SqlMinDate; // default to NOT effective

        /// <summary>
        /// Time (UTC) afer which this keyfile is no longer valid
        /// </summary>
        public DateTime ValidToUtc
        {
            get
            {
                return _validTo;
            }
            set
            {
                _validTo = value;
            }
        }

        /// <summary>
        /// Encrypt this class, serialize it, and convert it to Base64 which is suitable for storing in .config file or database fields.
        /// Encryption uses ActiveKey,ActiveIv (usually defaulting to internal keys of CryptoUtils class)
        /// </summary>
        /// <returns>Base64 string</returns>
        public string Pack()
        {          
            byte[] encryptedBytes = CryptoUtils.Encrypt(Serialize.To.Binary(this), CryptoUtils.ActiveKey.Key, CryptoUtils.ActiveKey.IV);
            return Convert.ToBase64String(encryptedBytes);
        }

        /// <summary>
        /// Unencrypts a previously 'Packed' key file using default keys
        /// </summary>
        /// <param name="encryptedKeyFile">Base64 of encrypted serialized object</param>
        /// <returns>Hydrated key file</returns>
        public static KeyFile Unpack(string encryptedKeyFile)
        {
            byte[] encryptedBytes = Convert.FromBase64String(encryptedKeyFile);
            byte[] plainTextBytes = CryptoUtils.Decrypt(encryptedBytes, CryptoUtils.ActiveKey.Key, CryptoUtils.ActiveKey.IV);
            return Serialize.From.Binary<KeyFile>(plainTextBytes);
        }

        /// <summary>
        /// Factory method to create a new hydrated key file
        /// </summary>
        /// <returns>Hydrated key file with 256bit values</returns>
        public static KeyFile New()
        {
            Random rdmGenerator = new Random();

            Rfc2898DeriveBytes generator = new Rfc2898DeriveBytes(Guid.NewGuid().ToByteArray(), Guid.NewGuid().ToByteArray(), rdmGenerator.Next(200, 500));
            KeyFile kf = new KeyFile();
            kf.Key = generator.GetBytes(256 / 8);
            kf.IV = generator.GetBytes(256 / 8);
            return kf;
        }

        /// <summary>
        /// Create a new key file with random encryption components and is valid for a period of time
        /// </summary>
        /// <param name="effectiveFrom">Instant in time this key file becomes effective</param>
        /// <param name="effectiveTo">Last instant in time this file is effective</param>
        /// <returns></returns>
        public static KeyFile New(DateTime effectiveFrom, DateTime effectiveTo)
        {
            KeyFile file = KeyFile.New();
            file.ValidFromUtc = effectiveFrom;
            file.ValidToUtc = effectiveTo;
            return file;
        }

        /// <summary>
        /// Use this key file to encrypt a data block.  Helper facade over CryptoUtils.Encrypt.
        /// </summary>
        /// <param name="plainBytes">Data block to encrypt</param>
        /// <returns>Encrypted bytes</returns>
        public byte[] Encrypt(byte[] plainBytes)
        {
            return CryptoUtils.Encrypt(plainBytes, this);
        }

        /// <summary>
        /// Use this key file to decrypt a data block.  Helper facade over CryptoUtils.Decrypt
        /// </summary>
        /// <param name="encryptedBytes">Bytes that had previously been encrypted</param>
        /// <returns>Plain text bytes</returns>
        public byte[] Decrypt(byte[] encryptedBytes)
        {
            return CryptoUtils.Decrypt(encryptedBytes, this);
        }
    }
}
