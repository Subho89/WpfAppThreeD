using System;
using System.Globalization;
using System.Windows;

namespace WpfAppThreeD
{
    /// <summary>
    /// Interaction logic for RangeDialog.xaml
    /// </summary>
    public partial class RangeDialog : Window
    {
        public enum RoundingMode { Real, Integer, Even, Odd }

        public RoundingMode SelectedMode { get; private set; }
        public int Digits { get; private set; }
        public string Expression { get; private set; }

        public double MinValue { get; private set; }
        public double MaxValue { get; private set; }
        public string axisCo { get; set; }
        public RangeDialog()
        {
            InitializeComponent();

            // defaults
            btnN.IsChecked = true; // Integer as default
            SelectedMode = RoundingMode.Integer;
            sliderDigits.Value = 3;
            sliderDigits.IsEnabled = false;
        }

        // Constructor with prefilled values
        public RangeDialog(double min, double max, string axis) : this()
        {
            MinValueBox.Text = min.ToString(CultureInfo.InvariantCulture);
            MaxValueBox.Text = max.ToString(CultureInfo.InvariantCulture);
            lblExpression.Text = "Expression (Use " + axis + " as the original value of the coordinate to generate the expression.)";
            axisCo=axis;
        }

        private void Rounding_Checked(object sender, RoutedEventArgs e)
        {
            if (btnR.IsChecked == true)
            {
                SelectedMode = RoundingMode.Real;
                sliderDigits.IsEnabled = true; // Only active for R
            }
            else if (btnN.IsChecked == true)
            {
                SelectedMode = RoundingMode.Integer;
                sliderDigits.IsEnabled = false;
            }
            else if (btnE.IsChecked == true)
            {
                SelectedMode = RoundingMode.Even;
                sliderDigits.IsEnabled = false;
            }
            else if (btnO.IsChecked == true)
            {
                SelectedMode = RoundingMode.Odd;
                sliderDigits.IsEnabled = false;
            }
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            // validate min/max
            if (!double.TryParse(MinValueBox.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out double min) ||
                !double.TryParse(MaxValueBox.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out double max))
            {
                MessageBox.Show("Please enter valid numeric min/max values.", "Invalid Input",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            MinValue = min;
            MaxValue = max;

            Digits = (int)sliderDigits.Value;
            Expression = txtExpression.Text.Trim();

            DialogResult = true;

            if (!string.IsNullOrEmpty(Expression)) 
            {
                if (!Expression.Contains(axisCo)) 
                {
                    MessageBox.Show("You have not used "+axisCo+" in the expression which reflects the original value of the coordinate");

                    return;
                }

                Expression = Expression.Trim();

            }
        }
    }
}
