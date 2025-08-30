using System;

public class PitInEvent : PitEvent
{
    public PitInEvent(string sessionId, string entryId, int currentLapNumber, TimeSpan currentLapTime)
        : base(sessionId, RaceEventType.PitIn, entryId, currentLapNumber, currentLapTime)
    { }

    public static PitInEvent CreateFromRecoveryFileLine(string line)
    {
        if (!MatchesRecoveryFileFormat(line))
        {
            throw new Exception("PitInEvent(): Cannot parse line: " + line + " into PitInEvent!");
        }

        var tokens = SplitLine(line);

        if (tokens.Length != 5)
        {
            throw new Exception("PitInEvent(): Cannot init PitInEvent! Invalid line:  " + line);
        }

        string sessionId = tokens[0];
        string entryId = tokens[2];
        int currentLapNumber = ToInt(tokens[3]);
        TimeSpan currentLapTime = ToTimeSpan(tokens[4]);

        return new PitInEvent(sessionId, entryId, currentLapNumber, currentLapTime);
    }

    public override PitEvent Clone()
    {
        return new PitInEvent(SessionId, EntryId, CurrentLapNumber, CurrentLapTime);
    }

    public override string ToRecoveryFileFormat()
    {
        return SessionId + s_recoveryFilePatternDelimiter
         + RaceEventTypeConverter.FromEnum(Type) + s_recoveryFilePatternDelimiter
         + EntryId + s_recoveryFilePatternDelimiter
         + CurrentLapNumber + s_recoveryFilePatternDelimiter
         + CurrentLapTime;
    }

    public override string ToString()
    {
        return $"[ sessionId: {SessionId}, type: {RaceEventTypeConverter.FromEnum(Type)}, EntryId: {EntryId}, CurrentLapNumber: {CurrentLapNumber}, CurrentLapTime: {CurrentLapTime} ]";
    }

    public override bool Equals(object obj)
    {
        // If parameter is null return false.
        if (obj == null)
        {
            return false;
        }

        // If parameter cannot be cast to Point return false.
        if (!(obj is PitInEvent other))
        {
            return false;
        }

        // Return true if the fields match:
        return SessionId == other.SessionId &&
               Type == other.Type &&
               EntryId == other.EntryId &&
               CurrentLapNumber == other.CurrentLapNumber &&
               CurrentLapTime == other.CurrentLapTime;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public static bool MatchesRecoveryFileFormat(string line)
    {
        var eventType = GetRaceEventTypeFromLine(line);
        return eventType == RaceEventType.PitIn;
    }
}