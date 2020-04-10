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

namespace FFmpeg_Wrapper_WPF.NET
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<ffmpegEntry> Entries;
        readonly String debugOutput="";
        public MainWindow()
        {
            InitializeComponent();
            Entries = new ObservableCollection<ffmpegEntry>();
            queueDataGrid.ItemsSource = Entries;
            //testing();
            Application.Current.Exit += new ExitEventHandler(Application_Exit);

        }
        private void Application_Exit(object sender, ExitEventArgs e)
        {
            Process[] processes = Process.GetProcessesByName("ffmpeg");
            foreach (Process process in processes)
            {
                process.Kill();
            }
            Application.Current.Exit -= new ExitEventHandler(Application_Exit);
        }
        private void addButtonClick(object sender, RoutedEventArgs e)
        {
            AddElement subWindow = new AddElement();
            subWindow.Show();
        }

        private void loadFileButtonClick(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.Filter = "Text Files|*.txt;*.csv|All Files|*.*";
            if (dlg.ShowDialog() == true)
            {
                string fileName = dlg.FileName;
                string[] lines = System.IO.File.ReadAllLines(fileName);
                foreach(string line in lines)
                {
                    ffmpegEntry newEntry = new ffmpegEntry();
                    newEntry.loadFromCSVLine(line);
                    Entries.Add(newEntry);
                }
                queueDataGrid.ItemsSource = Entries;
            }
        }
        static Task<int> RunProcessAsync(Process p)
        {
            var tcs = new TaskCompletionSource<int>();
            p.EnableRaisingEvents = true;
            p.Exited += (sender, args) =>
            {
                tcs.SetResult(p.ExitCode);
                p.Dispose();
            };
            p.Start();
            return tcs.Task;
        }
        private async void testing()
        {
            Queue<Process> processes = new Queue<Process>();
            ProcessStartInfo startnfo = new ProcessStartInfo { FileName = @"C:\Windows\system32\notepad.exe" };
            processes.Enqueue(new Process() { StartInfo = startnfo});
            processes.Enqueue(new Process() { StartInfo = startnfo });
            while(processes.Count>0)
            {
                int returnCode = await RunProcessAsync(processes.Dequeue());
                Debug.WriteLine(returnCode.ToString());
            }
        }
        private void experimentalProcessing()
        {
            Queue<Process> processQueue = new Queue<Process>();
            foreach (ffmpegEntry entry in Entries)
            {
                var processStartInfo = new ProcessStartInfo
                {
                    FileName = @"C:\Users\Gabriel\bins\ffmpeg.exe",
                    Arguments = entry.commandArgs,
                    RedirectStandardOutput = false,
                    RedirectStandardError = false,
                    UseShellExecute = false
                };
                processQueue.Enqueue(new Process() { StartInfo = processStartInfo });
            }
            runProcessQueue(processQueue);
        }

        private async void runProcessQueue(Queue<Process> processQueue)
        {
            while (processQueue.Count > 0)
            {
                int returnCode = await RunProcessAsync(processQueue.Dequeue());
                Debug.WriteLine(returnCode.ToString());
            }
        }

        private void startProcessingButtonClick(object sender, RoutedEventArgs e)
        {
            const bool doExperimental = true;
            if(doExperimental)
                experimentalProcessing();
            else
                legacyProcessing();
        }

        private void legacyProcessing()
        {
            foreach (ffmpegEntry entry in Entries)
            {
                var processStartInfo = new ProcessStartInfo
                {
                    FileName = @"C:\Users\Gabriel\bins\ffmpeg.exe",
                    Arguments = entry.commandArgs,
                    RedirectStandardOutput = false,
                    RedirectStandardError = false,
                    UseShellExecute = false
                };
                var process = Process.Start(processStartInfo);
                /* var output = process.StandardOutput.ReadToEnd();
                 var errorOut = process.StandardError.ReadToEnd();
                 if (errorOut.Length != 0)
                     output += "\nERROR " + errorOut;
                 debugOutput += output;
                 debugConsole.Text = debugOutput;
                 process.WaitForExit();*/
            }
        }

        private void clearButtonClick(object sender, RoutedEventArgs e)
        {
            Entries.Clear();
        }

        private void removeButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                ffmpegEntry selection = (ffmpegEntry)queueDataGrid.SelectedItem;
                Entries.Remove(selection);
            }
            catch (InvalidCastException)
            {
                Debug.WriteLine("Invalid column was deleted, recovered");
            }
            catch (Exception ex) { Debug.WriteLine("Exception: " + ex.Message); }
        }

        private void saveFileButtonClick(object sender, RoutedEventArgs e)
        {
            //save as file
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.Filter = "Text Files|*.txt;*.csv";
            if(dlg.ShowDialog() == true)
            {
                string textToWrite = queueToText();
                File.WriteAllText(dlg.FileName, textToWrite);
            }
        }

        private string queueToText()
        {
            string result = "";
            foreach(ffmpegEntry entry in Entries)
            {
                result += entry.ToCSVLine() +"\n";
            }
            return result;
        }
    }
}
