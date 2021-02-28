using System;
using System.Collections.Generic;

namespace Accounts.Models
{
    /// <summary>
    /// A list of transactions with additional methods.
    /// </summary>
    public class Transactions : List<Transaction>
    {
        /// <summary>
        /// Save the given transaction in the list.
        /// </summary>
        /// <param name="transaction">Transaction to save</param>
        public void Save(Transaction transaction)
        {
            // Remove the old transaction
            RemoveAll(t => t.Id == transaction.Id);
            // Insert at the right position
            var nextIndex = FindIndex(t => transaction.CompareTo(t) < 0);
            Insert(nextIndex >= 0 ? nextIndex : Count, transaction);
        }

        /// <summary>
        /// Copy a list of transactions to a new date.
        /// </summary>
        /// <param name="transactions">Transactions to copy</param>
        /// <param name="targetDate">Date for the new transactions</param>
        /// <returns>Copied transactions count</returns>
        public int Duplicate(List<Transaction> transactions, DateTime targetDate)
        {
            for (var i = 0; i < transactions.Count; i++)
                Save(new Transaction(transactions[i])
                {
                    Id = Guid.NewGuid(),
                    Date = targetDate.AddSeconds(i) // Keep transactions order
                });
            return transactions.Count;
        }
    }
}
