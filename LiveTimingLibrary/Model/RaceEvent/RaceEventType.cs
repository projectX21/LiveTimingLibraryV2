using System;

public enum RaceEventType
{
    PitIn,
    PitOut,
    SessionReload,
    PlayerFinishedLap
}

public class RaceEventTypeConverter
{
    public static RaceEventType ToEnum(string value)
    {
        switch (value)
        {
            case "PIT_IN":
                return RaceEventType.PitIn;

            case "PIT_OUT":
                return RaceEventType.PitOut;

            case "SESSION_RELOAD":
                return RaceEventType.SessionReload;

            case "PLAYER_FINISHED_LAP":
                return RaceEventType.PlayerFinishedLap;

            default:
                throw new Exception($"RaceEventTypeConverter::ToEnum(): cannot convert value to RaceEventType: {value}!");
        }
    }

    public static string FromEnum(RaceEventType type)
    {
        switch (type)
        {
            case RaceEventType.PitIn:
                return "PIT_IN";

            case RaceEventType.PitOut:
                return "PIT_OUT";

            case RaceEventType.SessionReload:
                return "SESSION_RELOAD";

            case RaceEventType.PlayerFinishedLap:
                return "PLAYER_FINISHED_LAP";

            default:
                throw new Exception($"RaceEventTypeConverter::FromEnum(): cannot determine string value of RaceEventType: " + type + "!");
        }
    }
}