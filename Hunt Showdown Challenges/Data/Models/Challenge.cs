using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hunt_Showdown_Challenges.Data.Models
{
    /// <summary>
    /// A Challenge
    /// </summary>
    public partial class Challenge : ObservableObject 
    {
        /// <summary>
        /// The title of the Challenge
        /// </summary>
        [ObservableProperty]
        protected string title = string.Empty;
        /// <summary>
        /// The description of the challenge
        /// </summary>
        [ObservableProperty]
        protected string description = string.Empty;


        /// <summary>
        /// The original title of the Challenge
        /// </summary>
        public string OriginalTitle { get; set; } = string.Empty;
        /// <summary>
        /// The original description of the challenge
        /// </summary>
        public string OriginalDescription { get; set; } = string.Empty;
    }
}
