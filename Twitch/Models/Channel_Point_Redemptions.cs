using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Twitch.Models
{
    public class Channel_Point_Redemptions
    {
        public List<Single> data { get; set; } = new();

        public class Single
        {
            public string broadcaster_name { get; set; }
            public string broadcaster_login { get; set; }
            public string broadcaster_id { get; set; }
            public string id { get; set; }
            public string image { get; set; }
            public string background_color { get; set; }
            public bool is_enabled { get; set; }
            public Int32 cost { get; set; }
            public string title { get; set; }
            public string prompt { get; set; }
            public bool is_user_input_required { get; set; }
            public struct_max_per_stream_setting max_per_stream_setting { get; set; }
            public struct_max_per_user_per_stream_setting max_per_user_per_stream_setting { get; set; }
            public struct_global_cooldown_setting global_cooldown_setting { get; set; }
            public bool is_paused { get; set; }
            public bool is_in_stock { get; set; }
            public struct_default_image default_image { get; set; }
            public bool should_redemptions_skip_request_queue { get; set; }
            public string redemptions_redeemed_current_stream { get; set; }
            public string cooldown_expires_at { get; set; }
        }

        public struct struct_max_per_stream_setting
        {
            public bool is_enabled { get; set; }
            public Int32 max_per_stream { get; set; }
        }
        public struct struct_max_per_user_per_stream_setting
        {
            public bool is_enabled { get; set; }
            public Int32 max_per_user_per_stream { get; set; }
        }
        public struct struct_global_cooldown_setting
        {
            public bool is_enabled { get; set; }
            public Int32 global_cooldown_seconds { get; set; }
        }
        public struct struct_default_image
        {
            public string url_1x { get; set; }
            public string url_2x { get; set; }
            public string url_4x { get; set; }
        }
    }
}
