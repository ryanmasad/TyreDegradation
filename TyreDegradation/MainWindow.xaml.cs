using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
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
using System.Xml.Linq;

namespace TyreDegradation
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {   
        //fields describing state of view, including current tyre, track and temperature selections
        ViewModel vm;
        public List<string> frontTyres = new List<string>();
        public List<string> rearTyres = new List<string>();
        public List<string> tracks = new List<string>();

        public Tyre FrontLeft;
        public Tyre FrontRight;
        public Tyre RearLeft;
        public Tyre RearRight;

        public Track currentTrack;
        public int temperature;

        //List of the result Text blocks, allows them to be edited as a group
        public List<TextBlock> resultsBoxes = new List<TextBlock>();

        /// <summary>
        /// Constructor for the UI window, instantiates the ViewModel and sets the initial state of the view
        /// </summary>
        public MainWindow()
        {
            vm = new ViewModel();
            InitializeComponent();
            StringComparison comp = StringComparison.OrdinalIgnoreCase;

            foreach (string tyre in vm.Tyres.Keys)
            {
                if (tyre.Contains("Front", comp))
                {
                    frontTyres.Add(tyre);
                }
                else
                {
                    rearTyres.Add(tyre);
                }
            }
            foreach (string track in vm.Tracks.Keys)
            {
                tracks.Add(track);
            }


            FLCombo.ItemsSource = frontTyres;
            FRCombo.ItemsSource = frontTyres;
            RLCombo.ItemsSource = rearTyres;
            RRCombo.ItemsSource = rearTyres;

            FrontLeft = vm.Tyres[frontTyres[0]];
            FrontRight = vm.Tyres[frontTyres[0]];
            RearLeft = vm.Tyres[rearTyres[0]];
            RearRight = vm.Tyres[rearTyres[0]];

            TrackCombo.ItemsSource = tracks;
            currentTrack = vm.Tracks[tracks[0]];
            temperature = currentTrack.Temperature;

            resultsBoxes.Add(FLARes);
            resultsBoxes.Add(FLMRes);
            resultsBoxes.Add(FLRRes);

            resultsBoxes.Add(FRARes);
            resultsBoxes.Add(FRMRes);
            resultsBoxes.Add(FRRRes);

            resultsBoxes.Add(RLARes);
            resultsBoxes.Add(RLMRes); 
            resultsBoxes.Add(RLRRes);

            resultsBoxes.Add(RRARes);
            resultsBoxes.Add(RRMRes);
            resultsBoxes.Add(RRRRes);

            UpdateResults();


        }

        /// <summary>
        /// Retrieves the current temperature at the currently selected circuit from the ViewModel
        /// </summary>
        /// <param name="location"></param>
        public void GetCurrentTempAsync()
        {

            vm.UpdateTrackTemp(currentTrack);
            temperature = currentTrack.Temperature;
            TempBox.Text = temperature.ToString();
           

        }

        /// <summary>
        /// Updates the result TextBlocks with new values and colors them accordingly
        /// </summary>
        public void UpdateResults()
        {
            if (IsValid())
            {

                int[] FrontLeftResults = vm.CalculateDegradation(currentTrack.Name, FrontLeft.Name, temperature);
                FLARes.Text = FrontLeftResults[0].ToString();
                FLMRes.Text = FrontLeftResults[1].ToString();
                FLRRes.Text = FrontLeftResults[2].ToString();

                int[] FrontRightResults = vm.CalculateDegradation(currentTrack.Name, FrontRight.Name, temperature);
                FRARes.Text = FrontRightResults[0].ToString();
                FRMRes.Text = FrontRightResults[1].ToString();
                FRRRes.Text = FrontRightResults[2].ToString();

                int[] RearLeftResults = vm.CalculateDegradation(currentTrack.Name, RearLeft.Name, temperature);
                RLARes.Text = RearLeftResults[0].ToString();
                RLMRes.Text = RearLeftResults[1].ToString();
                RLRRes.Text = RearLeftResults[2].ToString();

                int[] RearRightResults = vm.CalculateDegradation(currentTrack.Name, RearRight.Name, temperature);
                RRARes.Text = RearRightResults[0].ToString();
                RRMRes.Text = RearRightResults[1].ToString();
                RRRRes.Text = RearRightResults[2].ToString();

                foreach (TextBlock textBlock in resultsBoxes)
                {
                    if (int.Parse(textBlock.Text) < 100)
                    {
                        textBlock.Background = Brushes.Lime;
                    }
                    else if (int.Parse(textBlock.Text) < 300)
                    {
                        textBlock.Background = Brushes.Yellow;
                    }
                    else
                    {
                        textBlock.Background = Brushes.Red;
                    }

                }

            }
            else
            {
                foreach( TextBlock textBlock in resultsBoxes)
                {
                    textBlock.Text = "VOID";
                    textBlock.Background = Brushes.White;
                }

            }

        }

        
        /// <summary>
        /// Checks to see if the current configuration is valid
        /// </summary>
        /// <returns>A Boolean result</returns>
        private bool IsValid()
        {
            string currentType = FrontLeft.Type;
            if (temperature < 0)
            {
                return false;
            }
            else if (!FrontLeft.Family.Equals(FrontRight.Family)||!RearLeft.Family.Equals(RearRight.Family))
            {
                return false;
            }
            else if (!FrontRight.Type.Equals(currentType) || !RearLeft.Type.Equals(currentType) || !RearRight.Type.Equals(currentType))
            {
                return false;
            }

            return true;
        }

        //Handler methods for user input events
        private void FLCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (currentTrack != null)
            {
                string frontLeft = (sender as ComboBox).SelectedItem as string;
                FrontLeft = vm.Tyres[frontLeft];
                UpdateResults();
            }
        }

        private void FRCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (currentTrack != null)
            {
                string frontRight = (sender as ComboBox).SelectedItem as string;
                FrontRight = vm.Tyres[frontRight];
                UpdateResults();
            }
        }

        private void RLCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (currentTrack != null)
            {
                string rearLeft = (sender as ComboBox).SelectedItem as string;
                RearLeft = vm.Tyres[rearLeft];
                UpdateResults();
            }
        }

        private void RRCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (currentTrack != null)
            {
                string rearRight = (sender as ComboBox).SelectedItem as string;
                RearRight = vm.Tyres[rearRight];
                UpdateResults();
            }
        }

        private void TempBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string text = TempBox.Text;
            try
            {
                // Convert the text to an int
                int newtemp = Int32.Parse(text);                
                    
                TempBox.Foreground = Brushes.Black;
                temperature = newtemp;
                UpdateResults();
                
            }
            catch
            {
                // If there is an error, display the text using red
                TempBox.Foreground = Brushes.Red;
            }
        }
        private void TrackCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (currentTrack != null)
            {
                string newtrack = (sender as ComboBox).SelectedItem as string;
                currentTrack = vm.Tracks[newtrack];
                GetCurrentTempAsync();
                UpdateResults();
            }
        }

        private void TempResetButton_Click(object sender, RoutedEventArgs e)
        {
            GetCurrentTempAsync();

            UpdateResults();
        }

        
    }

}
