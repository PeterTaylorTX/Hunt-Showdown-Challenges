using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Twitch.APIs.EventSub
{
    public class TwitchEventSubClient(string clientId, string accessToken, string broadcasterUserId)
    {
        private const string WebSocketUrl = "wss://eventsub.wss.twitch.tv/ws";
        private readonly string _clientId = clientId;
        private readonly string _accessToken = accessToken;
        private readonly string _broadcasterUserId = broadcasterUserId;
        private ClientWebSocket _webSocket = new();
        private string lastMessageIDReceived = string.Empty;

        protected Action<string>? Callback;

        public async Task<bool> ConnectAsync(Action<string>? callback = null)
        {
            _webSocket = new ClientWebSocket();
            await _webSocket.ConnectAsync(new Uri(WebSocketUrl), CancellationToken.None);

            // Read session_welcome
            var buffer = new byte[8192];
            var result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            var response = Encoding.UTF8.GetString(buffer, 0, result.Count);

            var doc = JsonDocument.Parse(response);
            var sessionId = doc.RootElement
                .GetProperty("payload")
                .GetProperty("session")
                .GetProperty("id")
                .GetString();
            if (string.IsNullOrWhiteSpace(sessionId)) { return false; }
            Console.WriteLine("Connected. Session ID: " + sessionId);

            var subscribed = await SubscribeToRedemptionsAsync(sessionId);
            if (!subscribed) { return false; }
            Callback = callback;
            _ = ReceiveLoop();
            return true;
        }

        private async Task<bool> SubscribeToRedemptionsAsync(string sessionId)
        {
            using var http = new HttpClient();
            http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
            http.DefaultRequestHeaders.Add("Client-ID", _clientId);

            var payload = new
            {
                type = "channel.channel_points_custom_reward_redemption.add",
                version = "1",
                condition = new
                {
                    broadcaster_user_id = _broadcasterUserId
                },
                transport = new
                {
                    method = "websocket",
                    session_id = sessionId
                }
            };

            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await http.PostAsync("https://api.twitch.tv/helix/eventsub/subscriptions", content);
            var body = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine("Subscription failed: " + body);
                return false;
            }
            else
            {
                Console.WriteLine("Subscribed to redemptions.");
                return true;
            }
        }

        private async Task ReceiveLoop()
        {
            var buffer = new byte[8192];
            while (_webSocket.State == WebSocketState.Open)
            {
                var result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                var message = Encoding.UTF8.GetString(buffer, 0, result.Count);

                try
                {
                    var doc = JsonDocument.Parse(message);
                    var type = doc.RootElement.GetProperty("metadata").GetProperty("message_type").GetString();

                    if (type == "notification")
                    {
                        var eventData = doc.RootElement.GetProperty("payload").GetProperty("event");
                        var user = eventData.GetProperty("user_name").GetString();
                        var reward = eventData.GetProperty("reward").GetProperty("title").GetString();
                        string currentMessageID = eventData.GetProperty("id").GetString() ?? string.Empty;
                        if (lastMessageIDReceived == currentMessageID) { continue; }
                        lastMessageIDReceived = currentMessageID;

                        Console.WriteLine($"User {user} redeemed reward: {reward}");
                        if (Callback != null && !string.IsNullOrWhiteSpace(reward)) { Callback(reward); }
                    }
                    else if (type == "session_keepalive")
                    {
                        Console.WriteLine("Keep-alive received.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error parsing message: " + ex.Message);
                }
            }
        }
    }
}
