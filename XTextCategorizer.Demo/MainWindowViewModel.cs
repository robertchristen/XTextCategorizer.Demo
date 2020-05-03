using XTextCategorizer.Demo.Components;
using XTextCategorizer.Demo.Predict;
using XTextCategorizer.Demo.Train;

namespace XTextCategorizer.Demo
{
    public class MainWindowViewModel : ViewModelBase
    {
        public MainWindowViewModel()
        {
            var textCategorizer = new TextCategorizer();
            this.TrainViewModel = new TrainViewModel(textCategorizer);
            this.PredictViewModel = new PredictViewModel(textCategorizer);
        }

        public TrainViewModel TrainViewModel { get; }
        public PredictViewModel PredictViewModel { get; }
    }
}
