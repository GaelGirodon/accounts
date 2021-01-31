using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using Accounts.Models;
using Accounts.Properties;

namespace Accounts.ViewModels
{
    /// <summary>
    /// View model associated to the main window.
    /// </summary>
    public class MainViewModel : INotifyPropertyChanged
    {
        #region Global

        /// <summary>
        /// Current account file.
        /// </summary>
        public Account? Account
        {
            get => _account;
            set
            {
                _account = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Title));
                OnPropertyChanged(nameof(AccountVisibility));
                OnPropertyChanged(nameof(HomeVisibility));
                OnPropertyChanged(nameof(HasAccount));
            }
        }

        private Account? _account;

        /// <summary>
        /// Reset the view model to a clean state
        /// (to be invoked after the account is changed).
        /// </summary>
        public void Clean()
        {
            DateFilter = DateTime.Today;
            SelectedTransaction = null;
            Name = string.Empty;
            Date = DateTime.Now;
            Method = string.Empty;
            Amount = string.Empty;
            IsChecked = false;
            UpdateTransactionsView();
        }

        /// <summary>
        /// Window title.
        /// </summary>
        public string Title => (_account?.Path == null
                                   ? string.Empty
                                   : $"{Path.GetFileNameWithoutExtension(_account.Path)} - ")
                               + Resources.Application_Name;

        /// <summary>
        /// Visibility of the account panel.
        /// </summary>
        public Visibility AccountVisibility => Account == null ? Visibility.Collapsed : Visibility.Visible;

        /// <summary>
        /// Visibility of the home panel.
        /// </summary>
        public Visibility HomeVisibility => Account == null ? Visibility.Visible : Visibility.Collapsed;

        /// <summary>
        /// Indicates whether there is an opened account or not.
        /// </summary>
        public bool HasAccount => Account != null;

        #endregion

        #region Transactions view

        /// <summary>
        /// Transactions in the account.
        /// </summary>
        private List<Transaction> Transactions => Account?.Transactions ?? new List<Transaction>();

        /// <summary>
        /// Filters transactions by date.
        /// </summary>
        public DateTime DateFilter
        {
            get => _dateFilter;
            set
            {
                _dateFilter = new DateTime(value.Year, value.Month, 1)
                    .AddMonths(1).AddTicks(-1);
                OnPropertyChanged();
            }
        }

        private DateTime _dateFilter;

        /// <summary>
        /// Transactions, filtered by date, currently displayed.
        /// </summary>
        public List<TransactionViewModel> FilteredTransactions
        {
            get => _filteredTransactions;
            set
            {
                _filteredTransactions = value;
                OnPropertyChanged(nameof(FilteredTransactions));
            }
        }

        private List<TransactionViewModel> _filteredTransactions;

        /// <summary>
        /// Account balance at the selected month and year.
        /// </summary>
        public decimal CurrentBalance
        {
            get => _currentBalance;
            private set
            {
                _currentBalance = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CurrentBalanceColor));
            }
        }

        private decimal _currentBalance;

        /// <summary>
        /// Background color of the current balance indicator.
        /// </summary>
        public SolidColorBrush CurrentBalanceColor => new(CurrentBalance >= 0
            ? Color.FromRgb(68, 189, 50)
            : Color.FromRgb(194, 54, 22));

        /// <summary>
        /// Sum of the credit transactions for the current displayed month.
        /// </summary>
        public decimal MonthCredit
        {
            get => _monthCredit;
            private set
            {
                _monthCredit = value;
                OnPropertyChanged();
            }
        }

        private decimal _monthCredit;

        /// <summary>
        /// Sum of the debit transactions for the current displayed month.
        /// </summary>
        public decimal MonthDebit
        {
            get => _monthDebit;
            private set
            {
                _monthDebit = value;
                OnPropertyChanged();
            }
        }

        private decimal _monthDebit;

        /// <summary>
        /// Filter transactions according to the current date filter.
        /// </summary>
        public void UpdateTransactionsView()
        {
            FilteredTransactions = Transactions
                .Where(t => t.Date.Year == DateFilter.Year && t.Date.Month == DateFilter.Month)
                .Select(t => new TransactionViewModel(t)).ToList();
            CurrentBalance = Transactions
                .Where(t => t.Date <= DateFilter)
                .Aggregate(0m, (sum, t) => sum + t.Amount);
            MonthCredit = FilteredTransactions
                .Select(vm => vm.Transaction)
                .Where(t => t.Amount > 0)
                .Aggregate(0m, (sum, t) => sum + t.Amount);
            MonthDebit = FilteredTransactions
                .Select(vm => vm.Transaction)
                .Where(t => t.Amount < 0)
                .Aggregate(0m, (sum, t) => sum + t.Amount);
        }

        /// <summary>
        /// Transactions checked in the list.
        /// </summary>
        public List<TransactionViewModel> SelectedTransactions { get; set; }

        /// <summary>
        /// Selected transaction in the list.
        /// </summary>
        public TransactionViewModel? SelectedTransaction
        {
            get => _selectedTransaction;
            set
            {
                _selectedTransaction = value;
                Transaction = value?.Transaction;
                OnPropertyChanged();
            }
        }

        private TransactionViewModel? _selectedTransaction;

        /// <summary>
        /// Current transaction.
        /// </summary>
        public Transaction? Transaction
        {
            get => _transaction;
            set
            {
                _transaction = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(AddButtonVisibility));
                OnPropertyChanged(nameof(FormVisibility));
                OnPropertyChanged(nameof(SuggestedNames));
            }
        }

        private Transaction? _transaction;

        #endregion

        #region Transaction form

        /// <summary>
        /// Transaction name.
        /// </summary>
        public string? Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(SuggestedNames));
            }
        }

        private string? _name;

        /// <summary>
        /// Suggested values for the name of the transaction.
        /// </summary>
        public List<string> SuggestedNames => Name == null
            ? new List<string>()
            : Transactions.Select(t => t.Name).Distinct()
                .Where(n => n.Contains(Name)).Take(10).ToList();

        /// <summary>
        /// Transaction date.
        /// </summary>
        public DateTime Date
        {
            get => _date;
            set
            {
                _date = value;
                OnPropertyChanged();
            }
        }

        private DateTime _date;

        /// <summary>
        /// Transaction method.
        /// </summary>
        public string? Method
        {
            get => _method;
            set
            {
                _method = value;
                OnPropertyChanged();
            }
        }

        private string? _method;

        /// <summary>
        /// Suggested values for the payment method.
        /// </summary>
        public static List<string> SuggestedMethods => Transaction.Methods;

        /// <summary>
        /// Transaction amount.
        /// </summary>
        public string Amount
        {
            get => _amount;
            set
            {
                _amount = value;
                OnPropertyChanged();
            }
        }

        private string _amount;

        /// <summary>
        /// Currency symbol of the current culture.
        /// </summary>
        public static string Currency => new RegionInfo(Thread.CurrentThread.CurrentUICulture.LCID).CurrencySymbol;

        /// <summary>
        /// Transaction checked.
        /// </summary>
        public bool IsChecked
        {
            get => _isChecked;
            set
            {
                _isChecked = value;
                OnPropertyChanged();
            }
        }

        private bool _isChecked;

        /// <summary>
        /// Visibility of the add transaction button.
        /// </summary>
        public Visibility AddButtonVisibility => Transaction == null ? Visibility.Visible : Visibility.Collapsed;

        /// <summary>
        /// Visibility of the form panel.
        /// </summary>
        public Visibility FormVisibility => Transaction == null ? Visibility.Collapsed : Visibility.Visible;

        #endregion

        public MainViewModel()
        {
            DateFilter = DateTime.Today;
            _filteredTransactions = new List<TransactionViewModel>();
            SelectedTransactions = new List<TransactionViewModel>();
            _amount = string.Empty;
        }

        #region INotifyPropertyChanged

        /// <summary>
        /// Event raised when a property is changed.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Raise the PropertyChanged event for the given property.
        /// </summary>
        /// <param name="propertyName">Property name</param>
        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
