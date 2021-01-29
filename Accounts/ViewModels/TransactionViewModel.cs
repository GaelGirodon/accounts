using System;
using System.Windows;
using System.Windows.Media;
using Accounts.Models;

namespace Accounts.ViewModels
{
    /// <summary>
    /// Transaction view model.
    /// </summary>
    public class TransactionViewModel
    {
        /// <summary>
        /// Transaction date.
        /// </summary>
        public DateTime Date => Transaction.Date;

        /// <summary>
        /// Transaction name.
        /// </summary>
        public string Name => Transaction.Name;

        /// <summary>
        /// Transaction payment method.
        /// </summary>
        public string PaymentMethod => Transaction.Method;

        /// <summary>
        /// Formatted transaction amount.
        /// </summary>
        public decimal Amount => Transaction.Amount;

        /// <summary>
        /// Amount color.
        /// </summary>
        public SolidColorBrush AmountColor => new(Transaction.Amount >= 0
            ? Color.FromRgb(68, 189, 50)
            : Color.FromRgb(194, 54, 22));

        /// <summary>
        /// Checked image visibility.
        /// </summary>
        public Visibility CheckedVisibility => Transaction.IsChecked
            ? Visibility.Visible
            : Visibility.Collapsed;

        /// <summary>
        /// Not checked image visibility
        /// </summary>
        public Visibility NotCheckedVisibility => Transaction.IsChecked
            ? Visibility.Collapsed
            : Visibility.Visible;

        /// <summary>
        /// Original transaction.
        /// </summary>
        public Transaction Transaction { get; }

        public TransactionViewModel(Transaction transaction)
        {
            Transaction = transaction;
        }
    }
}
