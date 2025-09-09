using System;

public abstract class RaceEvent
{
    protected static readonly char s_recoveryFilePatternDelimiter = ';';

    public string SessionId { get; set; }

    public RaceEventType Type { get; protected set; }

    public int PlayerLapNumber { get; set; }

    public TimeSpan PlayerLapTime { get; set; }

    public RaceEvent() { }

    public RaceEvent(string sessionId, RaceEventType type, int playerLapNumber, TimeSpan playerLapTime)
    {
        SessionId = sessionId;
        Type = type;
        PlayerLapNumber = playerLapNumber;
        PlayerLapTime = playerLapTime;
    }

    public abstract string ToRecoveryFileFormat();

    public static RaceEventType GetRaceEventTypeFromLine(string line)
    {
        var tokens = SplitLine(line);
        return RaceEventTypeConverter.ToEnum(tokens[1]);
    }

    protected static string[] SplitLine(string line)
    {
        return line.Split(s_recoveryFilePatternDelimiter);
    }

    protected static TimeSpan ToTimeSpan(string value)
    {
        return TimeSpan.Parse(value);
    }

    protected static int ToInt(string value)
    {
        try
        {
            return int.Parse(value);
        }
        catch (FormatException)
        {
            throw new Exception($"RaceEvent::ToInt(): cannot convert value to int: {value}!");
        }
    }

}