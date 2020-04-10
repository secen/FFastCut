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

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            AddElement subWindow = new AddElement();
            subWindow.Show();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
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

        private void Button_Click_2(object sender, RoutedEventArgs e)
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

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            Entries.Clear();
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            ffmpegEntry selection = (ffmpegEntry)queueDataGrid.SelectedItem;
            Entries.Remove(selection);
        }
    }
}
