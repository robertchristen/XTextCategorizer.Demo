using Microsoft.Win32;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using XTextCategorizer.Demo.Components;
using XTextCategorizer.Events;

namespace XTextCategorizer.Demo.Train
{
    public class TrainViewModel : ViewModelBase
    {
        private readonly TextCategorizer textCategorizer;
        private readonly Stopwatch trainWatch = new Stopwatch();
        private string positiveSamples;
        private string negativeSamples;
        private double modelSize = 0.5;
        private string trainProgressPercentage;
        private int sampleSize;
        private bool operationInProgress;

        public TrainViewModel(TextCategorizer textCategorizer)
        {
            this.textCategorizer = textCategorizer;

            this.LoadExampleCommand = new DelegateCommand(this.LoadExample, this.CanAct);
            this.TrainCommand = new DelegateCommand(this.Train, this.CanAct);
            this.SaveCommand = new DelegateCommand(this.Save, this.CanAct);

            this.textCategorizer.TrainProgress += OnTrainProgress;
        }

        public string PositiveSamples
        {
            get => positiveSamples;
            set => SetValue(ref positiveSamples, value);
        }

        public string NegativeSamples
        {
            get => negativeSamples;
            set => SetValue(ref negativeSamples, value);
        }

        public double ModelSize
        {
            get => modelSize;
            set => SetValue(ref modelSize, value);
        }

        public string TrainProgressPercentage 
        { 
            get => trainProgressPercentage; 
            set => this.SetValue(ref trainProgressPercentage, value); 
        }

        public DelegateCommand LoadExampleCommand { get; }
        public DelegateCommand TrainCommand { get; }
        public DelegateCommand SaveCommand { get; }

        private void LoadExample()
        {
            PositiveSamples = File.ReadAllText("./Data/train-positive.txt");
            NegativeSamples = File.ReadAllText("./Data/train-negative.txt");
        }

        private void Train()
        {
            var samples = new List<Sample>();
            using (StringReader sr = new StringReader(PositiveSamples))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    samples.Add(new Sample { Text = line, Label = true });
                }
            }

            using (StringReader sr = new StringReader(NegativeSamples))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    samples.Add(new Sample { Text = line, Label = false });
                }
            }

            var config = new TrainingConfig();
            if (modelSize < 0.2)
            {
                config.Depth = 2;
                config.EmbedSize = 200;
                config.Width = 8;
            }
            else if (modelSize < 0.4)
            {
                config.Depth = 2;
                config.EmbedSize = 400;
                config.Width = 16;
            }
            else if (modelSize < 0.6)
            {
                config.Depth = 3;
                config.EmbedSize = 800;
                config.Width = 32;
            }
            else if (modelSize < 0.8)
            {
                config.Depth = 3;
                config.EmbedSize = 1600;
                config.Width = 64;
            }
            else
            {
                config.Depth = 4;
                config.EmbedSize = 2000;
                config.Width = 96;
            }

            this.sampleSize = samples.Count;
            
            this.operationInProgress = true;
            this.LoadExampleCommand.RaiseCanExecuteChanged();
            this.TrainCommand.RaiseCanExecuteChanged();
            this.SaveCommand.RaiseCanExecuteChanged();
            Task.Run(() =>
            {
                trainWatch.Restart();
                this.textCategorizer.Train(samples, config);
            }).ContinueWith(t =>
            {
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    this.operationInProgress = false;
                    this.LoadExampleCommand.RaiseCanExecuteChanged();
                    this.TrainCommand.RaiseCanExecuteChanged();
                    this.SaveCommand.RaiseCanExecuteChanged();
                }));
            });
        }

        private void Save()
        {
            var dialog = new SaveFileDialog();
            dialog.FileName = "model"; 
            dialog.DefaultExt = ".json"; 
            dialog.Filter = "Json file (.json)|*.json";
            dialog.Title = "Save file as";
            if (dialog.ShowDialog() == true)
            {
                var path = dialog.FileName;
                this.textCategorizer.SaveToFile(path);
            }
        }

        private bool CanAct()
        {
            return !this.operationInProgress;
        }

        private void OnTrainProgress(object sender, TrainProgressEventArgs e)
        {
            if (trainWatch.Elapsed > TimeSpan.FromSeconds(1))
            {
                var percentage = (double)e.TrainedSoFar * 100 / sampleSize;
                Application.Current.Dispatcher.Invoke(new Action(() => this.TrainProgressPercentage = string.Format("{0:0.00}", percentage) + "%"));
                trainWatch.Restart();
            }
        }
    }
}
