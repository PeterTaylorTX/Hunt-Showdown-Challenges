using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Twitch.APIs
{
    public class Channel_Point_Redemptions
    {
        /// <summary>
        /// Get the list of Channel Points
        /// Requires scope: moderator:manage:banned_users
        /// </summary>
        /// <param name="config">The Twitch config</param>
        /// <param name="ChannelID">The channel ID to ban the user in</param>
        /// <returns></returns>
        public static async Task<List<Models.Channel_Point_Redemptions.Single>> GetListAsync(Twitch.Config config, string ChannelID)
        {
            string URL = $"https://api.twitch.tv/helix/channel_points/custom_rewards?broadcaster_id={ChannelID}";

            string? result = (string?)await Twitch.Helpers.httpRequests.Get(URL, config);
            if (result == null) { return new List<Models.Channel_Point_Redemptions.Single>(); }

            var response = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Channel_Point_Redemptions>(result);
            if (response == null) { return new List<Models.Channel_Point_Redemptions.Single>(); }
            return response.data;
        }
    }
}
