using System.IO;
using System.IO.Compression;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Accounts.Models
{
    /// <summary>
    /// An account with transactions.
    /// </summary>
    public class Account
    {
        /// <summary>
        /// Account file extension.
        /// </summary>
        public const string Extension = ".account";

        /// <summary>
        /// Account transactions, sorted by date.
        /// </summary>
        public Transactions Transactions { get; set; }

        /// <summary>
        /// Path to the account file.
        /// </summary>
        [JsonIgnore]
        public string? Path { get; set; }

        /// <summary>
        /// Initialize an empty account.
        /// </summary>
        public Account()
        {
            Transactions = new Transactions();
        }

        /// <summary>
        /// Initialize an account with the path and the
        /// transactions list of another account.
        /// </summary>
        /// <param name="account">Account to clone</param>
        public Account(Account account)
        {
            Transactions = account.Transactions;
            Path = account.Path;
        }

        #region Serialization

        /// <summary>
        /// Serialize the current account object to JSON.
        /// </summary>
        /// <returns>A JSON string encoded as UTF-8 bytes</returns>
        public byte[] Serialize()
        {
            return JsonSerializer.SerializeToUtf8Bytes(this,
                new JsonSerializerOptions {WriteIndented = true});
        }

        /// <summary>
        /// Deserialize the given JSON data to an account object.
        /// </summary>
        /// <param name="data">A JSON string encoded as UTF-8 bytes</param>
        /// <returns>The deserialized account</returns>
        public static Account? Deserialize(byte[] data)
        {
            return JsonSerializer.Deserialize<Account>(data);
        }

        #endregion

        #region Save & Load

        /// <summary>
        /// Save the current account to its path.
        /// </summary>
        /// <returns>true if the account has been saved</returns>
        public bool Save()
        {
            if (string.IsNullOrEmpty(Path))
                return false;
            // Serialize
            var data = Serialize();
            // Compress and write
            using FileStream file = File.Create(Path);
            using var output = new GZipStream(file, CompressionMode.Compress);
            output.Write(data, 0, data.Length);
            return true;
        }

        /// <summary>
        /// Open an account from the given file path.
        /// </summary>
        /// <param name="path">Path to the account file</param>
        /// <returns>The opened account</returns>
        public static Account Load(string path)
        {
            // Read and decompress
            byte[] data;
            using (var file = File.OpenRead(path))
            using (var input = new GZipStream(file, CompressionMode.Decompress))
            using (var output = new MemoryStream())
            {
                input.CopyTo(output);
                data = output.ToArray();
            }

            if (data.Length == 0)
                throw new InvalidDataException();
            // Deserialize
            var account = Deserialize(data);
            if (account == null)
                throw new InvalidDataException();
            // Prepare account object
            account.Path = path;
            account.Transactions.Sort();
            return account;
        }

        #endregion
    }
}
