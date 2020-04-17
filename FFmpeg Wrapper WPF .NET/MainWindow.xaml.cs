using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;

namespace FFmpeg_Wrapper_WPF.NET
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string ffmpegLocation = @"ffmpeg.exe";
        public ObservableCollection<FfmpegEntry> Entries;
        private CancellationTokenSource tok = new CancellationTokenSource();
        public MainWindow()
        {
            InitializeComponent();
            Entries = new ObservableCollection<FfmpegEntry>();
            queueDataGrid.ItemsSource = Entries;
            //testing();
            Application.Current.Exit += new ExitEventHandler(ApplicationCleanupHandler);

        }
        private void ApplicationCleanupHandler(object sender, ExitEventArgs e)
        {
            Process[] processes = Process.GetProcessesByName("ffmpeg");
            foreach (Process process in processes)
            {
                process.Kill();
            }
            Application.Current.Exit -= new ExitEventHandler(ApplicationCleanupHandler);
        }
        private void AddButtonClick(object sender, RoutedEventArgs e)
        {
            AddElement subWindow = new AddElement();
            subWindow.Show();
        }

        private void LoadFileButtonClick(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Text Files|*.txt;*.csv|All Files|*.*"
            };
            if (dlg.ShowDialog() == true)
            {
                string fileName = dlg.FileName;
                string[] lines = System.IO.File.ReadAllLines(fileName);
                foreach(string line in lines)
                {
                    FfmpegEntry newEntry = new FfmpegEntry();
                    newEntry.LoadFromCSVLine(line);
                    Entries.Add(newEntry);
                }
                queueDataGrid.ItemsSource = Entries;
            }
        }
        void AppendDebugData(Object sender, DataReceivedEventArgs e)
        {
                if (e.Data != null)
                {
                this.Dispatcher.Invoke((Action) (() => {
                    debugConsole.Text += e.Data + "\n";
                    debugConsole.CaretIndex = debugConsole.Text.Length;
                }));
                }
        }
        Task<int> RunProcessAsync(Process p)
        {
            var tcs = new TaskCompletionSource<int>();
            p.EnableRaisingEvents = true;
            p.OutputDataReceived += new DataReceivedEventHandler(AppendDebugData);
            p.ErrorDataReceived += new DataReceivedEventHandler(AppendDebugData);
            p.Exited += (sender, args) =>
            {
                tcs.SetResult(p.ExitCode);
                p.Dispose();
            };
            p.Start();
            p.BeginOutputReadLine();
            p.BeginErrorReadLine();
            return tcs.Task;
        }
        private void ExperimentalProcessing()
        {
            startProcessingButton.Content = "Processing";
            startProcessingButton.IsEnabled = false;
            Queue<Process> processQueue = new Queue<Process>();
            foreach (FfmpegEntry entry in Entries)
            {
                var processStartInfo = new ProcessStartInfo
                {
                    FileName = ffmpegLocation,
                    //Use @"C:\Windows\System32\fsutil.exe" for testing.
                    Arguments = entry.CommandArgs,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                processQueue.Enqueue(new Process() { StartInfo = processStartInfo });
            }
            _ = RunProcessQueueAsync(processQueue);
        }

        private async Task RunProcessQueueAsync(Queue<Process> processQueue)
        {
            debugConsole.Text += DateTime.Now.ToString() + ": Starting processing\n";
            while (processQueue.Count > 0)
            {
                _ = await RunProcessAsync(processQueue.Dequeue());
                if (tok.IsCancellationRequested)
                {
                    tok.Dispose();
                    tok = new CancellationTokenSource();
                    processQueue.Clear();
                }
            }
            debugConsole.Text += DateTime.Now.ToString() + ": Processing Done\n";
            //return buttons to normal
            startProcessingButton.Content = "Start";
            startProcessingButton.IsEnabled = true;
            stopProcessingButton.Content = "Stop";
            stopProcessingButton.IsEnabled = false;

        }

        private void StartProcessingButtonClick(object sender, RoutedEventArgs e)
        {
            stopProcessingButton.IsEnabled = true;
            ExperimentalProcessing();
        }

        private void ClearButtonClick(object sender, RoutedEventArgs e)
        {
            Entries.Clear();
        }

        private void RemoveButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                FfmpegEntry selection = (FfmpegEntry)queueDataGrid.SelectedItem;
                Entries.Remove(selection);
            }
            catch (InvalidCastException)
            {
                Debug.WriteLine("Invalid column was deleted, recovered");
            }
            catch (Exception ex) { Debug.WriteLine("Exception: " + ex.Message); }
        }

        private void SaveFileButtonClick(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "Text Files|*.txt;*.csv"
            };
            if (dlg.ShowDialog() == true)
            {
                string textToWrite = QueueToText();
                File.WriteAllText(dlg.FileName, textToWrite);
            }
        }

        private string QueueToText()
        {
            string result = "";
            foreach(FfmpegEntry entry in Entries)
            {
                result += entry.ToCSVLine() +"\n";
            }
            return result;
        }

        private void OptionsButtonClick(object sender, RoutedEventArgs e)
        {
            OptionsMenu options = new OptionsMenu();
            options.Show();
        }

        private void StopProcessingButtonClick(object sender, RoutedEventArgs e)
        {
            tok.Cancel();
            stopProcessingButton.Content = "Stopping";
            stopProcessingButton.IsEnabled = false;
        }
    }
}
