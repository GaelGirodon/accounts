using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Accounts.Models;
using Accounts.Properties;
using Accounts.Services;
using Accounts.ViewModels;

namespace Accounts.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml,
    /// the main window of the application.
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MainViewModel _vm;

        public MainWindow()
        {
            _vm = new MainViewModel();
            InitializeComponent();
            _vm.UpdateTransactionsView();
            DataContext = _vm;
        }

        #region Account file management

        private void NewCommand_OnCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        /// <summary>
        /// Create a new empty account file without saving it.
        /// </summary>
        private void NewCommand_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (!AskSave())
                return;
            _vm.Account = new Account();
            _vm.Clean();
        }

        /// <summary>
        /// Open an account file using the given path.
        /// </summary>
        /// <param name="path">Account file path</param>
        public void OpenFile(string path)
        {
            try
            {
                _vm.Account = AccountService.Open(path);
                _vm.Clean();
            }
            catch (Exception)
            {
                MessageBox.Show(this,
                    Properties.Resources.Account_Open_InvalidFile,
                    Properties.Resources.Account_Open_Dialog_Title,
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OpenCommand_OnCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        /// <summary>
        /// Open an account file from the disk.
        /// </summary>
        private void OpenCommand_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (!AskSave())
                return;
            try
            {
                var account = AccountService.Open();
                if (account == null)
                    return;
                _vm.Account = account;
                _vm.Clean();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                MessageBox.Show(this,
                    Properties.Resources.Account_Open_InvalidFile,
                    Properties.Resources.Account_Open_Dialog_Title,
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SaveCommand_OnCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _vm.HasAccount;
        }

        /// <summary>
        /// Save an account file to the disk. If the account has never been persisted to the disk,
        /// a dialog opens to ask the user for the destination path.
        /// </summary>
        private void SaveCommand_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (_vm.Account == null)
                return;
            try
            {
                _vm.Account = AccountService.Save(_vm.Account);
                if (!string.IsNullOrEmpty(_vm.Account.Path))
                    MessageBox.Show(this,
                        string.Format(Properties.Resources.Account_Save_Success, _vm.Account.Path),
                        Properties.Resources.Account_Save_Dialog_Title,
                        MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception)
            {
                MessageBox.Show(this,
                    Properties.Resources.Account_Save_Error,
                    Properties.Resources.Account_Save_Dialog_Title,
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ArchiveCommand_OnCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _vm.HasAccount && !string.IsNullOrEmpty(_vm.Account?.Path);
        }

        /// <summary>
        /// Create a backup of the current account file.
        /// </summary>
        private void ArchiveCommand_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (_vm.Account == null || string.IsNullOrEmpty(_vm.Account?.Path))
                return;
            try
            {
                var backup = AccountService.Archive(_vm.Account);
                if (backup != null)
                    MessageBox.Show(this,
                        string.Format(Properties.Resources.Account_Archive_Success, backup.Path),
                        Properties.Resources.Account_Archive_Title,
                        MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception)
            {
                MessageBox.Show(this,
                    Properties.Resources.Account_Archive_Error,
                    Properties.Resources.Account_Archive_Title,
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CloseCommand_OnCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _vm.HasAccount;
        }

        /// <summary>
        /// Close the current account file.
        /// </summary>
        private void CloseCommand_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (!AskSave())
                return;
            _vm.Account = null;
            _vm.Clean();
        }

        /// <summary>
        /// Ask the user for saving the file before closing the window.
        /// </summary>
        private void Window_OnClosing(object sender, CancelEventArgs e)
        {
            if (!AskSave())
                e.Cancel = true;
        }

        /// <summary>
        /// Exit the application.
        /// </summary>
        private void ExitMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Ask whether the user wants to save modifications to the account file,
        /// before closing the file or exiting the application.
        /// </summary>
        /// <returns>false to cancel the action, true to proceed</returns>
        private bool AskSave()
        {
            if (_vm.Account == null)
                return true;
            var result = MessageBox.Show(this, Properties.Resources.Account_Save_BeforeExit_Content,
                Properties.Resources.Account_Save_BeforeExit_Title, MessageBoxButton.YesNoCancel,
                MessageBoxImage.Question, MessageBoxResult.Yes);
            if (result == MessageBoxResult.Cancel)
                return false; // Cancel the original action
            if (result != MessageBoxResult.Yes)
                return true; // Confirm the original action
            try
            {
                _vm.Account = AccountService.Save(_vm.Account);
                if (!string.IsNullOrEmpty(_vm.Account.Path))
                    MessageBox.Show(this,
                        string.Format(Properties.Resources.Account_Save_Success, _vm.Account.Path),
                        Properties.Resources.Account_Save_Dialog_Title,
                        MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception)
            {
                MessageBox.Show(this,
                    Properties.Resources.Account_Save_Error,
                    Properties.Resources.Account_Save_Dialog_Title,
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return true;
        }

        #endregion

        #region About

        /// <summary>
        /// Display the "About" prompt.
        /// </summary>
        private void AboutMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(this,
                Properties.Resources.About_Content,
                Properties.Resources.About_Title,
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        #endregion

        #region Transactions list management

        private void PreviousYearCommand_OnCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _vm.HasAccount;
        }

        /// <summary>
        /// Navigate to the previous year.
        /// </summary>
        private void PreviousYearCommand_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            _vm.DateFilter = _vm.DateFilter.AddYears(-1);
            _vm.UpdateTransactionsView();
        }

        private void PreviousMonthCommand_OnCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _vm.HasAccount;
        }

        /// <summary>
        /// Navigate to the previous month.
        /// </summary>
        private void PreviousMonthCommand_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            _vm.DateFilter = _vm.DateFilter.AddMonths(-1);
            _vm.UpdateTransactionsView();
        }

        private void NextMonthCommand_OnCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _vm.HasAccount;
        }

        /// <summary>
        /// Navigate to the next month.
        /// </summary>
        private void NextMonthCommand_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            _vm.DateFilter = _vm.DateFilter.AddMonths(1);
            _vm.UpdateTransactionsView();
        }

        private void NextYearCommand_OnCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _vm.HasAccount;
        }

        /// <summary>
        /// Navigate to the next year.
        /// </summary>
        private void NextYearCommand_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            _vm.DateFilter = _vm.DateFilter.AddYears(1);
            _vm.UpdateTransactionsView();
        }

        private void CurrentMonthCommand_OnCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _vm.HasAccount;
        }

        /// <summary>
        /// Navigate to the current month.
        /// </summary>
        private void CurrentMonthCommand_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            _vm.DateFilter = DateTime.Today;
            _vm.UpdateTransactionsView();
        }

        private void DuplicateCommand_OnCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _vm.SelectedTransactions.Count > 0;
        }

        /// <summary>
        /// Duplicate selected transactions to a new date.
        /// </summary>
        private void DuplicateCommand_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (_vm.SelectedTransactions.Count == 0)
                return;
            // Open the dialog to allow the user to select the target date
            var dialog = new TransactionsDuplicationWindow {Owner = this};
            dialog.ShowDialog();
            if (!dialog.SelectedDate.HasValue)
                return;
            // Duplicate selected transactions to the target date
            var transactions = _vm.SelectedTransactions
                .Select(tvm => tvm.Transaction).ToList();
            var targetDate = dialog.SelectedDate.Value;
            if (_vm.Account?.Transactions.Duplicate(transactions, targetDate) == 0)
                return;
            MessageBox.Show(this, string.Format(Properties.Resources.Transactions_Duplicate_Success,
                    targetDate.ToShortDateString()),
                Properties.Resources.Menu_Edit_Duplicate_Description,
                MessageBoxButton.OK, MessageBoxImage.Information);
            // Navigate to the target month
            _vm.DateFilter = targetDate;
            _vm.UpdateTransactionsView();
        }

        private void CheckOffCommand_OnCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _vm.Transaction != null;
        }

        /// <summary>
        /// Check the selected transaction off.
        /// </summary>
        private void CheckOffCommand_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var transaction = e.Parameter is Transaction p ? p : _vm.Transaction;
            if (transaction == null || transaction.IsChecked)
                return;
            var updatedTransaction = new Transaction(transaction) {IsChecked = true};
            _vm.Account?.Transactions.Save(updatedTransaction);
            _vm.UpdateTransactionsView();
            _vm.SelectedTransaction = _vm.FilteredTransactions
                .Find(t => t.Transaction.Id == transaction.Id);
            Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Input, new Action(() =>
                ((UIElement) TransactionsListView.ItemContainerGenerator
                    .ContainerFromItem(_vm.SelectedTransaction)).Focus()));
        }

        #endregion

        #region Transaction form

        /// <summary>
        /// Update the transaction form when the selected transaction in the ListView changes.
        /// </summary>
        private void TransactionsListView_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _vm.SelectedTransactions = (sender as ListView)?.SelectedItems.Cast<TransactionViewModel>().ToList()
                                       ?? new List<TransactionViewModel>();
            if (_vm.Transaction == null)
                return;
            ResetComboBoxes();
            _vm.Name = _vm.Transaction.Name;
            _vm.Date = _vm.Transaction.Date;
            _vm.Method = _vm.Transaction.Method;
            _vm.Amount = _vm.Transaction.AmountAsString;
            _vm.IsChecked = _vm.Transaction.IsChecked;
        }

        private void AddTransactionCommand_OnCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _vm.HasAccount && _vm.Transaction == null;
        }

        /// <summary>
        /// Initialize the form with default values for a new transaction.
        /// </summary>
        private void AddTransactionCommand_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            _vm.Transaction = new Transaction();
            ResetComboBoxes();
            _vm.Name = _vm.Transaction.Name;
            _vm.Date = _vm.Transaction.Date;
            _vm.Method = _vm.Transaction.Method;
            _vm.Amount = string.Empty;
            _vm.IsChecked = _vm.Transaction.IsChecked;
            NameComboBox.Focus();
        }

        /// <summary>
        /// Reset transaction form combo-boxes.
        /// </summary>
        private void ResetComboBoxes()
        {
            NameComboBox.SelectedIndex = -1;
            MethodComboBox.SelectedIndex = -1;
        }

        /// <summary>
        /// Type the right currency decimal separator when the decimal key is pressed.
        /// </summary>
        private void AmountTextBox_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            var currentDecimal = NumberFormatInfo.CurrentInfo.CurrencyDecimalSeparator;
            var invariantDecimal = NumberFormatInfo.InvariantInfo.CurrencyDecimalSeparator;
            if (e.Key != Key.Decimal || currentDecimal == invariantDecimal || sender is not TextBox textBox)
                return;
            var caretIndex = textBox.CaretIndex;
            textBox.Text = textBox.Text.Substring(0, textBox.SelectionStart)
                           + currentDecimal
                           + textBox.Text.Substring(textBox.SelectionStart + textBox.SelectionLength);
            textBox.CaretIndex = caretIndex + 1;
            e.Handled = true;
        }

        /// <summary>
        /// Close the form and unset the selected transaction.
        /// </summary>
        private void CancelButton_OnClick(object sender, RoutedEventArgs e)
        {
            _vm.SelectedTransaction = null;
        }

        /// <summary>
        /// Delete the selected transaction (with confirmation).
        /// </summary>
        private void DeleteButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (_vm.Transaction == null)
                return;
            var result = MessageBox.Show(this,
                Properties.Resources.Transaction_Form_Delete_Confirmation,
                Properties.Resources.Transaction_Form_Delete_Description,
                MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result != MessageBoxResult.Yes)
                return; // Cancel
            // Delete the selected transaction
            _vm.Account?.Transactions.Remove(_vm.Transaction);
            _vm.SelectedTransaction = null;
            _vm.UpdateTransactionsView();
        }

        /// <summary>
        /// Validate and save the transaction.
        /// </summary>
        private void SaveButton_OnClick(object sender, RoutedEventArgs e)
        {
            // Validate fields
            string? error = null;
            var amount = decimal.Zero;
            if (string.IsNullOrWhiteSpace(_vm.Name))
                error = Properties.Resources.Transaction_Form_Save_Validation_NameRequired;
            else if (string.IsNullOrWhiteSpace(_vm.Method))
                error = Properties.Resources.Transaction_Form_Save_Validation_MethodRequired;
            else if (!decimal.TryParse(_vm.Amount, out amount) || amount == 0)
                error = Properties.Resources.Transaction_Form_Save_Validation_InvalidAmount;
            if (error != null)
            {
                MessageBox.Show(this, error,
                    Properties.Resources.Transaction_Form_Save_Description,
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Save (add or update) the transaction
            _vm.Account?.Transactions.Save(new Transaction
            {
                Id = _vm.Transaction?.Id ?? Guid.Empty,
                Name = _vm.Name ?? string.Empty,
                Date = _vm.Date,
                Method = _vm.Method ?? string.Empty,
                Amount = amount,
                IsChecked = _vm.IsChecked
            });
            _vm.DateFilter = _vm.Date;
            _vm.SelectedTransaction = null;
            _vm.UpdateTransactionsView();
        }

        #endregion
    }

    /// <summary>
    /// Main window custom commands.
    /// </summary>
    public static class MainWindowCommands
    {
        /// <summary>
        /// Create a backup of the current account file.
        /// </summary>
        public static readonly RoutedUICommand Archive = new(
            Resources.Menu_File_Archive,
            Resources.Menu_File_Archive,
            typeof(MainWindowCommands),
            new InputGestureCollection
            {
                new KeyGesture(Key.B, ModifierKeys.Control)
            });

        /// <summary>
        /// Close the current account file.
        /// </summary>
        public static readonly RoutedUICommand Close = new(
            Resources.Menu_File_Close,
            Resources.Menu_File_Close,
            typeof(MainWindowCommands),
            new InputGestureCollection
            {
                new KeyGesture(Key.W, ModifierKeys.Control)
            });

        /// <summary>
        /// Duplicate transactions selected in the list to another date.
        /// </summary>
        public static readonly RoutedUICommand Duplicate = new(
            Resources.Menu_Edit_Duplicate_Description,
            Resources.Menu_Edit_Duplicate,
            typeof(MainWindowCommands),
            new InputGestureCollection
            {
                new KeyGesture(Key.D, ModifierKeys.Control)
            });

        /// <summary>
        /// Check the selected transaction off.
        /// </summary>
        public static readonly RoutedUICommand CheckOff = new(
            Resources.Transaction_CheckOff,
            Resources.Transaction_CheckOff,
            typeof(MainWindowCommands));

        /// <summary>
        /// Initialize the form with default values for a new transaction.
        /// </summary>
        public static readonly RoutedUICommand AddTransaction = new(
            Resources.Transaction_Form_Add,
            Resources.Transaction_Form_Add_Description,
            typeof(MainWindowCommands),
            new InputGestureCollection
            {
                new KeyGesture(Key.T, ModifierKeys.Control)
            });

        /// <summary>
        /// Navigate to the previous year.
        /// </summary>
        public static readonly RoutedUICommand PreviousYear = new(
            Resources.Navigation_Year_Previous,
            Resources.Navigation_Year_Previous,
            typeof(MainWindowCommands));

        /// <summary>
        /// Navigate to the previous month.
        /// </summary>
        public static readonly RoutedUICommand PreviousMonth = new(
            Resources.Navigation_Month_Previous,
            Resources.Navigation_Month_Previous,
            typeof(MainWindowCommands),
            new InputGestureCollection
            {
                new KeyGesture(Key.Tab, ModifierKeys.Control | ModifierKeys.Shift)
            });

        /// <summary>
        /// Navigate to the next month.
        /// </summary>
        public static readonly RoutedUICommand NextMonth = new(
            Resources.Navigation_Month_Next,
            Resources.Navigation_Month_Next,
            typeof(MainWindowCommands),
            new InputGestureCollection
            {
                new KeyGesture(Key.Tab, ModifierKeys.Control)
            });

        /// <summary>
        /// Navigate to the next year.
        /// </summary>
        public static readonly RoutedUICommand NextYear = new(
            Resources.Navigation_Year_Next,
            Resources.Navigation_Year_Next,
            typeof(MainWindowCommands));

        /// <summary>
        /// Navigate to the current month.
        /// </summary>
        public static readonly RoutedUICommand CurrentMonth = new(
            Resources.Navigation_Current,
            Resources.Navigation_Current,
            typeof(MainWindowCommands),
            new InputGestureCollection
            {
                new KeyGesture(Key.M, ModifierKeys.Control)
            });
    }
}
