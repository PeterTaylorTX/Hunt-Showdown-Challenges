using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hunt_Showdown_Challenges.ViewModels
{
    public partial class Main : ObservableObject
    {
        /// <summary>
        /// A list of currently selected Challenges
        /// </summary>
        [ObservableProperty]
        private ObservableCollection<Data.Logic.Challenge> selectedChallenges = new();
        /// <summary>
        /// A list of remaining Challenges
        /// </summary>
        public List<Data.Logic.Challenge> RemainingChallenges { get; set; } = new();

        /// <summary>
        /// The name of player 1
        /// </summary>
        public string Player1 { get; set; } = Hunt_Showdown_Challenges.Resources.Strings.UI.You;
        /// <summary>
        /// The name of player 2
        /// </summary>
        public string Player2 { get; set; } = string.Empty;
        /// <summary>
        /// The name of player 3
        /// </summary>
        public string Player3 { get; set; } = string.Empty;
        /// <summary>
        /// Should the description be written to the text file
        /// </summary>
        public bool WriteDescriptionToFile { get; set; } = false;
        /// <summary>
        /// The application Version number
        /// </summary>
        public string AppVersion => $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Version}";
        /// <summary>
        /// The application title
        /// </summary>
        public string AppTitle { get; set; } = $"Hunt Showdown Challenges ({System.Reflection.Assembly.GetExecutingAssembly().GetName().Version})";
    }
}
