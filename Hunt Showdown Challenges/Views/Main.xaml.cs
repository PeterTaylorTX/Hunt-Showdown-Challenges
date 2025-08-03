using Hunt_Showdown_Challenges.Views;
using System.ComponentModel;
using System.Configuration;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Hunt_Showdown_Challenges
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ViewModels.Main viewModel = new();

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                await Data.Logic.Challenge.LoadChallengesAsync();
                btnReset_Clicked(sender, e);

                if (Properties.Settings.Default.LastAppVersion == String.Empty)
                {
                    Properties.Settings.Default.Upgrade();
                    Properties.Settings.Default.LastAppVersion = viewModel.AppVersion;
                    Properties.Settings.Default.Save();
                }
                viewModel.WriteDescriptionToFile = Properties.Settings.Default.WriteDescription;
                viewModel.Player1 = Properties.Settings.Default.Player1;
                viewModel.Player2 = Properties.Settings.Default.Player2;
                viewModel.Player3 = Properties.Settings.Default.Player3;


                this.DataContext = viewModel;
            }
            catch (System.IO.FileNotFoundException ex) { MessageBox.Show(Hunt_Showdown_Challenges.Resources.Strings.UI.Error, $"{Hunt_Showdown_Challenges.Resources.Strings.UI.Error_FileNotFound} {ex.Message}", MessageBoxButton.OK); }
            catch (Exception ex) { MessageBox.Show(Hunt_Showdown_Challenges.Resources.Strings.UI.Error, $"{Hunt_Showdown_Challenges.Resources.Strings.UI.ErrorMessage} {ex.Message}", MessageBoxButton.OK); }
        }

        /// <summary>
        /// Add a new Challenge to the list
        /// </summary>
        private async void btnNewChallenge_Clicked(object sender, EventArgs e)
        {
            if (Data.Logic.Challenge.Challenges == null) { return; }
            var id = Random.Shared.Next(0, viewModel.RemainingChallenges.Count - 1);
            var newChallenge = viewModel.RemainingChallenges[id];
            viewModel.RemainingChallenges.Remove(newChallenge);
            viewModel.SelectedChallenges.Add(newChallenge);

            await Data.Logic.Challenge.ResetChallengeFile();
            foreach (var challenge in viewModel.SelectedChallenges)
            {
                string playerName = this.ChooseRandomPlayer();
                challenge.Title = challenge.OriginalTitle.Replace("{Player}", playerName);
                challenge.Description = challenge.OriginalDescription.Replace("{Player}", playerName);
                await Data.Logic.Challenge.UpdateChallengeFile(challenge, viewModel.WriteDescriptionToFile); //Update the Challenge File
            }
        }

        /// <summary>
        /// Choose a random player
        /// </summary>
        private string ChooseRandomPlayer()
        {
            Int32 max = 1;
            if (!string.IsNullOrWhiteSpace(viewModel.Player2)) { max = 2; }
            if (!string.IsNullOrWhiteSpace(viewModel.Player3)) { max = 3; }
            var id = Random.Shared.Next(1, max + 1);

            switch (id)
            {
                case 2: return viewModel.Player2;
                case 3: return viewModel.Player3;
                default: return viewModel.Player1;
            }
        }

        /// <summary>
        /// Reset the current challenges
        /// </summary>
        private async void btnReset_Clicked(object sender, RoutedEventArgs e)
        {
            if (Data.Logic.Challenge.Challenges == null) { return; }
            viewModel.SelectedChallenges.Clear();
            viewModel.RemainingChallenges = Data.Logic.Challenge.Challenges.ToList();
            await Data.Logic.Challenge.ResetChallengeFile(); //Wipe the Challenge file
        }

        /// <summary>
        /// Open the folder containing the Challenge File
        /// </summary>
        private void btnChallengeFile_Clicked(object sender, RoutedEventArgs e)
        {
            string dataDirectory = $"\"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\Hunt Showdown Challenges\"";
            System.Diagnostics.Process.Start("explorer.exe", dataDirectory);
        }

        /// <summary>
        /// Remove the selected Challenge
        /// </summary>
        private async void btnRemoveChallenge_Clicked(object sender, RoutedEventArgs e)
        {
            var removeButton = ((Button)sender);
            var challengeTitle = removeButton.CommandParameter.ToString();

            Data.Logic.Challenge? challenge = viewModel.SelectedChallenges.Where(c => c.Title == challengeTitle).FirstOrDefault();
            if (challenge == null) { return; }

            viewModel.SelectedChallenges.Remove(challenge);
            challenge.Title = challenge.OriginalTitle;
            challenge.Description = challenge.OriginalDescription;
            viewModel.RemainingChallenges.Add(challenge);

            await Data.Logic.Challenge.ResetChallengeFile();
            foreach (var existingChallenge in viewModel.SelectedChallenges)
            {
                await Data.Logic.Challenge.UpdateChallengeFile(existingChallenge, viewModel.WriteDescriptionToFile); //Update the Challenge File
            }
        }

        /// <summary>
        /// Save the settings on exit
        /// </summary>
        protected override void OnClosing(CancelEventArgs e)
        {
            Properties.Settings.Default.WriteDescription = viewModel.WriteDescriptionToFile;
            Properties.Settings.Default.Player1 = viewModel.Player1;
            Properties.Settings.Default.Player2 = viewModel.Player2;
            Properties.Settings.Default.Player3 = viewModel.Player3;
            Properties.Settings.Default.Save();
            btnReset_Clicked(this, new());
            base.OnClosing(e);
        }

        /// <summary>
        /// Open the About Page
        /// </summary>
        private void btnAbout_Clicked(object sender, RoutedEventArgs e)
        {
            var about = new About();
            about.ShowDialog();
        }
    }
}