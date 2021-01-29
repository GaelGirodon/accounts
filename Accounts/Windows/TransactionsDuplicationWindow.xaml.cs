using System;
using System.Windows;
using System.Windows.Input;

namespace Accounts.Windows
{
    /// <summary>
    /// Date picker prompt used for transactions duplication.
    /// </summary>
    public partial class TransactionsDuplicationWindow : Window
    {
        /// <summary>
        /// Selected date.
        /// </summary>
        public DateTime? SelectedDate { get; private set; }

        public TransactionsDuplicationWindow()
        {
            InitializeComponent();
        }

        #region Commands

        private void SubmitCommand_OnCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = DatePicker?.SelectedDate != null && DatePicker.SelectedDate.Value > new DateTime(2000, 1, 1);
        }

        /// <summary>
        /// Store the selected date and close the window.
        /// </summary>
        private void SubmitCommand_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            SelectedDate = DatePicker.SelectedDate;
            Close();
        }

        private void CancelCommand_OnCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        /// <summary>
        /// Close the window.
        /// </summary>
        private void CancelCommand_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Close();
        }

        #endregion
    }

    /// <summary>
    /// Transactions duplication window custom commands.
    /// </summary>
    public static class TransactionsDuplicationWindowCommands
    {
        /// <summary>
        /// Submit the form: duplicate selected transactions to the selected date.
        /// </summary>
        public static readonly RoutedUICommand Submit = new(
            Properties.Resources.Transactions_Duplicate_Dialog_Submit,
            Properties.Resources.Transactions_Duplicate_Dialog_Submit,
            typeof(TransactionsDuplicationWindowCommands),
            new InputGestureCollection {new KeyGesture(Key.Enter)});

        /// <summary>
        /// Cancel duplication and close the dialog.
        /// </summary>
        public static readonly RoutedUICommand Cancel = new(
            Properties.Resources.Transactions_Duplicate_Dialog_Cancel,
            Properties.Resources.Transactions_Duplicate_Dialog_Cancel,
            typeof(TransactionsDuplicationWindowCommands),
            new InputGestureCollection {new KeyGesture(Key.Escape)});
    }
}
