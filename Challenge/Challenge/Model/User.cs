﻿using Newtonsoft.Json;

namespace Challenge.Model
{
    public class User
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("login")]
        public string Login { get; set; }

        [JsonProperty("avatar_url")]
        public string AvatarUrl { get; set; }
    }
}