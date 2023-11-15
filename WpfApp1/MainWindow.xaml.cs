using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.IO;


namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool fileSaved = false;
        private string savedFilePath = string.Empty;

        public MainWindow()
        {
            InitializeComponent();
            SetStatus("Ready");
        }

        private void SetStatus(string message)
        {
            TxtBlock.Text = message;
        }

        private async void StartButton_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(textBox.Text, out int count) &&
                int.TryParse(textBoxRangeFrom.Text, out int minValue) &&
                int.TryParse(textBoxRangeTo.Text, out int maxValue))
            {
                RandomNumberGenerator generator = new RandomNumberGenerator();

                // IProgress<int> progress is used to update progress
                Progress<int> progress = new Progress<int>(value =>
                {
                    TxtBlock.Text = $"Postęp: {value}%";
                    ProgrssBarApp.Value = value; // ProgressBar progress update
                });

                List<int> generatedNumbers = await generator.GenerateAsync(count, minValue, maxValue, progress);

                Console.WriteLine("Wygenerowane liczby:");
                
                TxtOutputBlock.Text = "Wygenerowane liczby:\n" + string.Join(", ", generatedNumbers);   
                
            }
            else
            {
                SetStatus("Invalid input. Please enter valid numbers.");
            }
        }

        private void LoadItem_Click(object sender, RoutedEventArgs e)
        {
            TextFileHandling textFile = new TextFileHandling();

            string? path = textFile.GetFilePath();
            List<string>? text = new List<string>();
            if (path != null)
            {
                text = textFile.ReadTextFile(path);
            }

            if (text != null)
            {
                TxtOutputBlock.Text = "";
                foreach (string line in text)
                {
                    TxtOutputBlock.Text += line + Environment.NewLine;
                }
            }
        }

        private void SaveItem_Click(object sender, RoutedEventArgs e)
        {
            TextFileHandling textFile = new TextFileHandling();
            if (!fileSaved)
            {
                string? path = textFile.SaveFilePath();
                if (path == null)
                {
                    return;
                }

                List<string> text = new List<string>(TxtOutputBlock.Text.Split(Environment.NewLine));

                textFile.WriteTextFile(path, text);
                fileSaved = true;
                savedFilePath = path;

                MessageBox.Show("Plik został zapisany.", "Informacja", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                if (!string.IsNullOrEmpty(savedFilePath))
                {
                    List<string> text = new List<string>(TxtOutputBlock.Text.Split(Environment.NewLine));

                    textFile.WriteTextFile(savedFilePath, text);

                    MessageBox.Show("Plik został nadpisany.", "Informacja", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Błąd: Ścieżka do pliku nie została ustawiona.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }


        }
    }

    public class RandomNumberGenerator
    {
        private readonly Random random = new Random();

        public async Task<List<int>> GenerateAsync(int count, int minValue, int maxValue, IProgress<int> progress = null)
        {
            List<int> result = new List<int>();

            for (int i = 0; i < count; i++)
            {
                // Simulation of the draw delay
                await Task.Delay(500);

                int randomNumber = random.Next(minValue, maxValue + 1);
                result.Add(randomNumber);

                // Progress update
                progress?.Report((i + 1) * 100 / count);
            }

            return result;
        }
    }
    
    public class TextFileHandling
    {
        public List<string>? ReadTextFile(string path)
        {
            try
            {
                StreamReader sr = new StreamReader(path);

                string? line;
                List<string> text = new List<string>();

                while ((line = sr.ReadLine()) != null)
                {
                    text.Add(line);
                }
                return text;
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
                return null;
            }
            finally
            {
                Console.WriteLine("Executing finally block.");
            }

        }

        public void WriteTextFile(string path, List<string> text)
        {
            try
            {
                StreamWriter sw = new StreamWriter(path);

                foreach (string line in text)
                {
                    sw.WriteLine(line);
                }
                sw.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }
            finally
            {
                Console.WriteLine("Executing finally block.");
            }
        }

        public string? GetFilePath()
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.FileName = "Document";
            dialog.DefaultExt = ".txt";
            dialog.Filter = "Text documents (.txt)|*.txt";

            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                return dialog.FileName;
            }

            return null;
        }

        public string? SaveFilePath()
        {
            var dialog = new Microsoft.Win32.SaveFileDialog();
            dialog.FileName = "Document";
            dialog.DefaultExt = ".txt";
            dialog.Filter = "Text documents (.txt)|*.txt";

            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                return dialog.FileName;
            }

            return null;
        }

    }
}
