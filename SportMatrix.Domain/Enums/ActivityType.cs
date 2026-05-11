namespace SportMatrix.Domain.Enums;

using System.Text.Json.Serialization;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ActivityType
{
    Run = 1,
    Ride = 2,
    Swim = 3,
    Workout = 4,
    Yoga = 5,
    Hike = 6,
    Brick = 7,
}
