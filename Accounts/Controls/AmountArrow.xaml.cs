using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Accounts.Controls
{
    /// <summary>
    /// Amount arrow control.
    /// <br />
    /// Displays a circle with:
    /// <ul>
    /// <li>A green background and an arrow pointing to the top-right corner if the amount is positive,</li>
    /// <li>A red background and an arrow pointing to the bottom-right corner if the amount is negative,</li>
    /// <li>A gray background and an arrow pointing to the right elsewhere.</li>
    /// </ul>
    /// </summary>
    public partial class AmountArrow : UserControl
    {
        /// <summary>
        /// Amount value.
        /// </summary>
        public decimal Amount
        {
            get => (decimal) GetValue(AmountProperty);
            set => SetValue(AmountProperty, value);
        }

        /// <summary>
        /// Amount dependency property (allows data binding and more).
        /// </summary>
        public static readonly DependencyProperty AmountProperty = DependencyProperty
            .Register(nameof(Amount), typeof(decimal), typeof(AmountArrow),
                new UIPropertyMetadata(0m, AmountPropertyChanged));

        /// <summary>
        /// Modify the circle background and the arrow angle
        /// depending on the amount value.
        /// </summary>
        private static void AmountPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var amount = e.NewValue is decimal value ? value : 0;
            if (!(d is AmountArrow control))
                return;
            control.ArrowRotation.Angle = amount switch
            {
                { } a when a >= 0 => -45,
                { } a when a <= 0 => 45,
                _ => 0
            };
            control.Ellipse.Fill = new SolidColorBrush(amount switch
            {
                { } a when a >= 0 => Color.FromRgb(68, 189, 50),
                { } a when a <= 0 => Color.FromRgb(194, 54, 22),
                _ => Colors.Gray
            });
        }

        /// <summary>
        /// Initialize the circle with a gray background
        /// and an arrow pointing to the right.
        /// </summary>
        public AmountArrow()
        {
            InitializeComponent();
            ArrowRotation.Angle = 0;
            Ellipse.Fill = new SolidColorBrush(Colors.Gray);
        }
    }
}
