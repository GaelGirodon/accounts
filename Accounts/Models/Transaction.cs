using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using static Accounts.Properties.Resources;

namespace Accounts.Models
{
    /// <summary>
    /// An account transaction.
    /// </summary>
    public class Transaction : IComparable<Transaction>
    {
        /// <summary>
        /// Suggested values for the payment method.
        /// </summary>
        public static readonly List<string> Methods = new()
        {
            Transaction_Method_Card, Transaction_Method_Cheque,
            Transaction_Method_Cash, Transaction_Method_Transfer
        };

        /// <summary>
        /// Transaction id.
        /// </summary>
        public Guid Id { get; init; }

        /// <summary>
        /// Transaction name.
        /// </summary>
        public string Name { get; init; }

        /// <summary>
        /// Transaction date.
        /// </summary>
        public DateTime Date { get; init; }

        /// <summary>
        /// Transaction method.
        /// </summary>
        public string Method { get; init; }

        /// <summary>
        /// Transaction amount (positive or negative).
        /// </summary>
        public decimal Amount { get; init; }

        /// <summary>
        /// Transaction amount, formatted.
        /// </summary>
        [JsonIgnore]
        public string AmountAsString => $"{Amount:F2}";

        /// <summary>
        /// Indicates whether the transaction is checked or not.
        /// </summary>
        public bool IsChecked { get; init; }

        /// <summary>
        /// Initialize a new transaction with default values.
        /// </summary>
        public Transaction()
        {
            Id = Guid.NewGuid();
            Name = Transaction_Name_DefaultValue;
            Date = DateTime.Today;
            Method = string.Empty;
        }

        /// <summary>
        /// Initialize a new transaction using the given transaction fields values.
        /// </summary>
        /// <param name="transaction">Other transaction</param>
        public Transaction(Transaction transaction)
        {
            Id = transaction.Id;
            Name = transaction.Name;
            Date = transaction.Date;
            Method = transaction.Method;
            Amount = transaction.Amount;
            IsChecked = transaction.IsChecked;
        }

        /// <summary>
        /// Compare two transactions by date, or if equals, by id.
        /// </summary>
        /// <param name="other">Other transaction to compare with this</param>
        public int CompareTo(Transaction? other)
        {
            var dateComparison = Date.CompareTo(other?.Date);
            return dateComparison != 0 ? dateComparison : Id.CompareTo(other?.Id);
        }
    }
}
