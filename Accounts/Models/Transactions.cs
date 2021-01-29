using System;
using System.Collections.Generic;
using System.Linq;

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
            var nextIndex = FindIndex(t => transaction.Date < t.Date);
            Insert(nextIndex >= 0 ? nextIndex : Count, transaction);
        }

        /// <summary>
        /// Copy a list of transactions to a new date.
        /// </summary>
        /// <param name="transactions">Transactions to copy</param>
        /// <param name="targetDate">Date for the new transactions</param>
        /// <returns>Copied transactions count.</returns>
        public int Duplicate(List<Transaction> transactions, DateTime targetDate)
        {
            var nextIndex = FindIndex(t => targetDate < t.Date);
            InsertRange(nextIndex >= 0 ? nextIndex : Count,
                transactions.Select(t => new Transaction(t) {Date = targetDate}));
            return transactions.Count;
        }
    }
}
