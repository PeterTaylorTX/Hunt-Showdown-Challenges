using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hunt_Showdown_Challenges.ViewModels
{
    public partial class Twitch_Integration : ObservableObject
    {
        /// <summary>
        /// The Twitch Configuration
        /// </summary>
        public Twitch.Config TwitchConfig { get; set; } = new();
        [ObservableProperty]
        private ObservableCollection<Twitch.Models.Channel_Point_Redemptions.Single> channel_Points = new();
        public Twitch.Models.Channel_Point_Redemptions.Single? Selected_Channel_Points_Redeem { get; set; } = null;

        [ObservableProperty]
        protected bool isConnected = false;

    }
}
