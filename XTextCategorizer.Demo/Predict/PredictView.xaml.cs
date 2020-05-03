using System.Windows.Controls;

namespace XTextCategorizer.Demo.Predict
{
    /// <summary>
    /// Interaction logic for PredictView.xaml
    /// </summary>
    public partial class PredictView : UserControl
    {
        public PredictView()
        {
            InitializeComponent();
        }

        private void OnPredictionTextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            var textbox = (TextBox)sender;
            textbox.ScrollToEnd();
        }
    }
}
