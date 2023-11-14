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


namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
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
                foreach (int number in generatedNumbers)
                {
                    TxtOutputBlock.Text = "Wygenerowane liczby:\n" + string.Join(", ", generatedNumbers);   
                }
            }
            else
            {
                SetStatus("Invalid input. Please enter valid numbers.");
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
}
