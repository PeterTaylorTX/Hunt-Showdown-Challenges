using Hunt_Showdown_Challenges.ViewModels;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace Hunt_Showdown_Challenges.Views
{
    /// <summary>
    /// Interaction logic for Twitch_Integration.xaml
    /// </summary>
    public partial class Twitch_Integration : Window
    {
        /// <summary>
        /// The current View Model
        /// </summary>
        ViewModels.Twitch_Integration viewModel = new();

        /// <summary>
        /// The current namespace
        /// </summary>
        protected const string Namespace = "Hunt_Showdown_Challenges.Views.Twitch_Integration";
        public Twitch_Integration(Twitch.Config config)
        {
            InitializeComponent();
            viewModel.TwitchConfig = config;
            viewModel.IsConnected = viewModel.TwitchConfig.OAuthToken != null;
            this.DataContext = viewModel;
        }

        public new Twitch.Config ShowDialog()
        {
            base.ShowDialog();
            return viewModel.TwitchConfig;
        }

        /// <summary>
        /// Load Channel Points
        /// </summary>
        protected async Task LoadChannelPoints()
        {
            if (viewModel.TwitchConfig.OAuthToken == null) { return; }
            viewModel.ConnectedChannel = viewModel.TwitchConfig.Channel.Display_Name;
            var channelPoints = await Twitch.APIs.Channel_Point_Redemptions.GetListAsync(viewModel.TwitchConfig, viewModel.TwitchConfig.Channel.ID);
            channelPoints.Insert(0, new() { title = Hunt_Showdown_Challenges.Resources.Strings.UI.None });
            viewModel.Channel_Points = new(channelPoints);

            if (string.IsNullOrWhiteSpace(Main.TwitchRedeemItem))
            {
                viewModel.Selected_Channel_Points_Redeem = viewModel.Channel_Points[0]; //Default the selection to None
            }
            else
            {
                var redeemItem = viewModel.Channel_Points.Where(cp => cp.title == Main.TwitchRedeemItem).FirstOrDefault();
                if (redeemItem == null)
                {
                    viewModel.Selected_Channel_Points_Redeem = viewModel.Channel_Points[0]; //Default the selection to None
                }
                else
                {
                    viewModel.Selected_Channel_Points_Redeem = redeemItem; //Load the selected channel point redeem
                }
            }
        }

        /// <summary>
        /// Authenticate the Twitch Channel
        /// </summary>
        private async void btnGetNewOAuthToken_Clicked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(viewModel.TwitchConfig.ClientID)) { return; }
                viewModel.TwitchConfig.OverrideScope("channel:read:redemptions+channel:bot+user:write:chat+chat:read");
                viewModel.TwitchConfig.GetOAuthToken();

                var userDetails = await Twitch.APIs.Users.GetUsersAsync(viewModel.TwitchConfig);
                if (userDetails == null) { MessageBox.Show(Hunt_Showdown_Challenges.Resources.Strings.UI.Authentication, Hunt_Showdown_Challenges.Resources.Strings.UI.Authentication_Failed, MessageBoxButton.OK); return; }

                viewModel.TwitchConfig.Channel = userDetails.Data[0];
                viewModel.ConnectedChannel = viewModel.TwitchConfig.Channel.Display_Name;
                await viewModel.TwitchConfig.SaveAsync();
                await this.LoadChannelPoints();
            }
            catch (Exception ex) { MessageBox.Show($"{ex.Message}\n\n{Namespace}.btnGetNewOAuthToken_Clicked", Hunt_Showdown_Challenges.Resources.Strings.UI.ErrorTitle); }
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                await this.LoadChannelPoints();
            }
            catch (Exception ex) { MessageBox.Show($"{ex.Message}\n\n{Namespace}.Window_Loaded", Hunt_Showdown_Challenges.Resources.Strings.UI.ErrorTitle); }
        }

        /// <summary>
        /// The selected Redeem has changed
        /// </summary>
        private void TwitchRedeem_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var addedItem = (Twitch.Models.Channel_Point_Redemptions.Single?)e.AddedItems[0];
            if (addedItem == null) { return; }
            Main.TwitchRedeemItem = addedItem.title;
        }

        /// <summary>
        /// Disconnect the Twitch service
        /// </summary>
        private async void btnDeleteOAuthToken_Clicked(object sender, RoutedEventArgs e)
        {
            if (ViewModels.Main.eventSubClient != null)
            {
                await ViewModels.Main.eventSubClient.DisconnectAsync();
            }
            viewModel.TwitchConfig.Channel = new();
            viewModel.TwitchConfig.OAuthToken = null;
            await viewModel.TwitchConfig.SaveAsync();
            this.Close();
        }
    }
}

