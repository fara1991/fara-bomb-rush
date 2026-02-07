using System.Collections.Generic;
using Newtonsoft.Json;

namespace FaraBombRush.Models;

internal class ScoreSaberModel
{
    [JsonProperty("id", Required = Required.Always)]
    internal string Id { get; set; }

    [JsonProperty("name", Required = Required.Always)]
    internal string Name { get; set; }

    [JsonProperty("profilePicture", Required = Required.AllowNull)]
    internal string ProfilePicture { get; set; }

    [JsonProperty("bio", Required = Required.AllowNull)]
    internal string Bio { get; set; }

    [JsonProperty("country", Required = Required.Always)]
    internal string Country { get; set; }

    [JsonProperty("pp", Required = Required.Always)]
    internal float PP { get; set; }

    [JsonProperty("rank", Required = Required.Always)]
    internal float Rank { get; set; }

    [JsonProperty("countryRank", Required = Required.Always)]
    internal float CountryRank { get; set; }

    [JsonProperty("role", Required = Required.AllowNull)]
    internal string Role { get; set; }

    [JsonProperty("badges", Required = Required.AllowNull)]
    internal List<BadgeModel> Badges { get; set; }

    [JsonProperty("histories", Required = Required.AllowNull)]
    internal string Histories { get; set; }

    [JsonProperty("scoreStats", Required = Required.AllowNull)]
    internal ScoreStatsModel ScoreStats { get; set; }

    [JsonProperty("permissions", Required = Required.AllowNull)]
    internal float Permissions { get; set; }

    [JsonProperty("banned", Required = Required.AllowNull)]
    internal bool Banned { get; set; }

    [JsonProperty("inactive", Required = Required.AllowNull)]
    internal bool Inactive { get; set; }

    [JsonProperty("firstSeen", Required = Required.AllowNull)]
    internal string FirstSeen { get; set; }
}

internal class BadgeModel
{
    [JsonProperty("description", Required = Required.AllowNull)]
    internal string Description { get; set; }

    [JsonProperty("image", Required = Required.AllowNull)]
    internal string Image { get; set; }
}

internal class ScoreStatsModel
{
    [JsonProperty("totalScore", Required = Required.AllowNull)]
    internal float TotalScore { get; set; }

    [JsonProperty("totalRankedScore", Required = Required.AllowNull)]
    internal float TotalRankedScore { get; set; }

    [JsonProperty("averageRankedAccuracy", Required = Required.AllowNull)]
    internal float AverageRankedAccuracy { get; set; }

    [JsonProperty("totalPlayCount", Required = Required.AllowNull)]
    internal float TotalPlayCount { get; set; }

    [JsonProperty("rankedPlayCount", Required = Required.AllowNull)]
    internal float RankedPlayCount { get; set; }

    [JsonProperty("replayedWatched", Required = Required.AllowNull)]
    internal float ReplayedWatched { get; set; }
}
