using System;

public class PitOutEvent : PitEvent
{
    // Is needed for calculating the pit stop duration (PitTime = PitOut.CurrentLapTime + (PitOut.LastLapTime - PitIn.CurrentLapTime)
    public TimeSpan LastLapTime { get; }

    public PitOutEvent(string sessionId, string entryId, int currentLapNumber, TimeSpan currentLapTime, TimeSpan lastLapTime)
        : base(sessionId, RaceEventType.PitOut, entryId, currentLapNumber, currentLapTime)
    {
        LastLapTime = lastLapTime;
    }

    public static PitOutEvent CreateFromRecoveryFile(string line)
    {
        if (!MatchesRecoveryFileFormat(line))
        {
            throw new Exception("PitOutEvent(): Cannot parse line: " + line + " into PitOutEvent!");
        }

        var tokens = SplitLine(line);

        if (tokens.Length != 6)
        {
            throw new Exception("PitOutEvent(): Cannot init PitOutEvent! Invalid line:  " + line);
        }

        var sessionId = tokens[0];
        var entryId = tokens[2];
        var currentLapNumber = ToInt(tokens[3]);
        var currentLapTime = ToTimeSpan(tokens[4]);
        var lastLapTime = ToTimeSpan(tokens[5]);

        return new PitOutEvent(sessionId, entryId, currentLapNumber, currentLapTime, lastLapTime);
    }

    public static PitOutEvent CreateFromRecoveryFileLine(string line)
    {
        if (!MatchesRecoveryFileFormat(line))
        {
            throw new Exception("PitOutEvent(): Cannot parse line: " + line + " into PitOutEvent!");
        }

        var tokens = SplitLine(line);

        if (tokens.Length != 6)
        {
            throw new Exception("PitOutEvent(): Cannot init PitOutEvent! Invalid line:  " + line);
        }

        string sessionId = tokens[0];
        RaceEventType type = RaceEventTypeConverter.ToEnum(tokens[1]);
        string entryId = tokens[2];
        int currentLapNumber = ToInt(tokens[3]);
        TimeSpan currentLapTime = ToTimeSpan(tokens[4]);
        TimeSpan lastLapTime = ToTimeSpan(tokens[5]);

        return new PitOutEvent(sessionId, entryId, currentLapNumber, currentLapTime, lastLapTime);
    }

    public override PitEvent Clone()
    {
        return new PitOutEvent(SessionId, EntryId, CurrentLapNumber, CurrentLapTime, LastLapTime);
    }

    public override string ToRecoveryFileFormat()
    {
        return SessionId + s_recoveryFilePatternDelimiter
         + RaceEventTypeConverter.FromEnum(Type) + s_recoveryFilePatternDelimiter
         + EntryId + s_recoveryFilePatternDelimiter
         + CurrentLapNumber + s_recoveryFilePatternDelimiter
         + CurrentLapTime + s_recoveryFilePatternDelimiter
         + LastLapTime;
    }

    public override string ToString()
    {
        return $"[ sessionId: {SessionId}, type: {RaceEventTypeConverter.FromEnum(Type)}, EntryId: {EntryId}, CurrentLapNumber: {CurrentLapNumber}, CurrentLapTime: {CurrentLapTime}, LastLapTime: {LastLapTime} ]";
    }

    public override bool Equals(object obj)
    {
        // If parameter is null return false.
        if (obj == null)
        {
            return false;
        }

        // If parameter cannot be cast to Point return false.
        if (!(obj is PitOutEvent other))
        {
            return false;
        }

        // Return true if the fields match:
        return SessionId == other.SessionId &&
               Type == other.Type &&
               EntryId == other.EntryId &&
               CurrentLapNumber == other.CurrentLapNumber &&
               CurrentLapTime == other.CurrentLapTime &&
               LastLapTime == other.LastLapTime;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public static bool MatchesRecoveryFileFormat(string line)
    {
        var eventType = GetRaceEventTypeFromLine(line);
        return eventType == RaceEventType.PitOut;
    }
}