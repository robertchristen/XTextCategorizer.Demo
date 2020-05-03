using Microsoft.Win32;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using XTextCategorizer.Demo.Components;

namespace XTextCategorizer.Demo.Predict
{
    public class PredictViewModel : ViewModelBase
    {
        private readonly TextCategorizer textCategorizer;
        private string samples;
        private string predictions = string.Empty;
        private bool operationInProgress;

        public PredictViewModel(TextCategorizer textCategorizer)
        {
            this.textCategorizer = textCategorizer;

            this.LoadExampleCommand = new DelegateCommand(this.LoadExample, this.CanAct);
            this.LoadCommand = new DelegateCommand(this.Load, this.CanAct);
            this.PredictCommand = new DelegateCommand(this.Predict, this.CanAct);
        }

        public string Samples
        {
            get => samples;
            set => SetValue(ref samples, value);
        }

        public string Predictions
        {
            get => predictions;
            set => SetValue(ref predictions, value);
        }


        public DelegateCommand LoadExampleCommand { get; }
        public DelegateCommand PredictCommand { get; }
        public DelegateCommand LoadCommand { get; }

        private void LoadExample()
        {
            Samples = File.ReadAllText("./Data/test-positive.txt") + File.ReadAllText("./Data/test-negative.txt");
        }

        private void Load()
        {
            var dialog = new OpenFileDialog();
            dialog.DefaultExt = ".json";
            dialog.Filter = "Json file (.json)|*.json";
            dialog.Title = "Open file";

            if (dialog.ShowDialog() == true)
            {
                var path = dialog.FileName;
                this.textCategorizer.LoadFromFile(path);
            }
        }

        private void Predict()
        {
            var samples = new List<string>();
            using (StringReader sr = new StringReader(Samples))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    samples.Add(line);
                }
            }

            var sb = new StringBuilder();
            var watch = new Stopwatch();
            watch.Start();
            this.Predictions = string.Empty;
            this.operationInProgress = true;
            this.LoadExampleCommand.RaiseCanExecuteChanged();
            this.LoadCommand.RaiseCanExecuteChanged();
            this.PredictCommand.RaiseCanExecuteChanged();
            Task.Run(() =>
            {
                foreach (var sample in samples)
                {
                    var prediction = textCategorizer.Predict(sample);
                    var category = prediction.Label ? "Positive" : "Negative";
                    var predString = string.Format("{0:0.0000}", prediction.Score) + " - " + category + "\r\n";
                    sb.Append(predString);
                    if (watch.Elapsed > TimeSpan.FromSeconds(1))
                    {
                        Application.Current.Dispatcher.Invoke(new Action(() =>
                        {
                            this.Predictions = sb.ToString();
                            watch.Restart();
                        }));
                    }
                }
            }).ContinueWith(t =>
            {
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    this.operationInProgress = false;
                    this.LoadExampleCommand.RaiseCanExecuteChanged();
                    this.LoadCommand.RaiseCanExecuteChanged();
                    this.PredictCommand.RaiseCanExecuteChanged();

                    this.Predictions = sb.ToString();
                }));
            });
        }

        private bool CanAct()
        {
            return !this.operationInProgress;
        }
    }
}
