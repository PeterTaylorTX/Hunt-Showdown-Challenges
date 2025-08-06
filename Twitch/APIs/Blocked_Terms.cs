
namespace Twitch.APIs
{
    public class Blocked_Terms
    {
        /// <summary>
        /// Ban a viewer
        /// Requires scope: channel:read:redemptions
        /// </summary>
        /// <param name="config">The Twitch config</param>
        /// <param name="ChannelID">The channel ID to get the Channel Points from</param>
        /// <returns></returns>
        public static async Task<Models.Blocked_Terms.Blocked_Terms_Response> BlockTermAsync(Twitch.Config config, string ChannelID, string Term)
        {
            string URL = $"https://api.twitch.tv/helix/moderation/blocked_terms?broadcaster_id={ChannelID}&moderator_id={config.Channel.ID}";
            var request = new Models.Blocked_Terms.Blocked_Terms_Request() { text = Term };

            string? result = (string?)await Twitch.Helpers.httpRequests.Post(URL, request, config);
            if (result == null) { return new(); }

            Models.Blocked_Terms.Blocked_Terms_Response? response = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Blocked_Terms.Blocked_Terms_Response>(result);
            if (response == null) { return new(); }
            return response;
        }


    }
}
