using System;
using System.IO;
using Accounts.Models;
using Accounts.Properties;
using Microsoft.Win32;

namespace Accounts.Services
{
    /// <summary>
    /// Account file management.
    /// </summary>
    public static class AccountService
    {
        /// <summary>
        /// Open an account file.
        /// </summary>
        /// <param name="path">Optional account file path</param>
        /// <returns>The account</returns>
        /// <exception cref="InvalidDataException">Invalid format</exception>
        public static Account? Open(string? path = null)
        {
            if (path == null) // Select file path
            {
                var dialog = new OpenFileDialog
                {
                    Title = Resources.Account_Open_Dialog_Title,
                    Filter = $"{Resources.Account_FileType} ({Account.Extension})|*{Account.Extension}"
                };
                var result = dialog.ShowDialog();
                if (result == false || !File.Exists(dialog.FileName))
                    return null;
                path = dialog.FileName;
            }

            return Account.Load(path);
        }

        /// <summary>
        /// Save an account to a file.
        /// </summary>
        /// <param name="account">Account to save</param>
        /// <returns>Saved account</returns>
        public static Account Save(Account account)
        {
            if (string.IsNullOrEmpty(account.Path)) // Select file path
            {
                var dialog = new SaveFileDialog
                {
                    Title = Resources.Account_Save_Dialog_Title,
                    FileName = Resources.Account_DefaultFileName,
                    DefaultExt = Account.Extension,
                    Filter = $"{Resources.Account_FileType} ({Account.Extension})|*{Account.Extension}"
                };
                var result = dialog.ShowDialog();
                if (result == false || string.IsNullOrWhiteSpace(dialog.FileName))
                    return account;
                account.Path = dialog.FileName;
            }

            account.Save();
            return account;
        }

        /// <summary>
        /// Create a backup of an account file.
        /// </summary>
        /// <param name="account">The account to backup</param>
        /// <returns>The account backup</returns>
        public static Account? Archive(Account account)
        {
            if (string.IsNullOrEmpty(account.Path)) return null;
            var directoryName = Path.GetDirectoryName(account.Path);
            if (directoryName == null) return null;
            var backup = new Account(account)
            {
                Path = Path.Combine(directoryName,
                    Path.GetFileNameWithoutExtension(account.Path) +
                    "-" + DateTime.Now.ToString("yyyyMMddhhmmss") +
                    Path.GetExtension(account.Path))
            };
            backup.Save();
            return backup;
        }
    }
}
