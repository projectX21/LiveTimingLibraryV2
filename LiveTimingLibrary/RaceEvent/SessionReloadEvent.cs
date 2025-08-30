using System;

public class SessionReloadEvent : RaceEvent
{
    public SessionReloadEvent(string sessionId, int currentLapNumber, TimeSpan currentLapTime)
        : base(sessionId, RaceEventType.SessionReload, currentLapNumber, currentLapTime) { }

    public static SessionReloadEvent CreateFromRecoveryFileLine(string line)
    {
        if (!MatchesRecoveryFileFormat(line))
        {
            throw new Exception("SessionReloadEvent(): Cannot parse line: " + line + " into SessionReloadEvent!");
        }

        var tokens = SplitLine(line);

        if (tokens.Length != 4)
        {
            throw new Exception("SessionReloadEvent(): Cannot create SessionReloadEvent from recovery file! Invalid line:  " + line);
        }

        string sessionId = tokens[0];
        int currentLapNumber = ToInt(tokens[2]);
        TimeSpan currentLapTime = ToTimeSpan(tokens[3]);

        return new SessionReloadEvent(sessionId, currentLapNumber, currentLapTime);
    }

    public override string ToRecoveryFileFormat()
    {
        return SessionId + s_recoveryFilePatternDelimiter
          + RaceEventTypeConverter.FromEnum(Type) + s_recoveryFilePatternDelimiter
          + CurrentLapNumber + s_recoveryFilePatternDelimiter
          + CurrentLapTime;
    }

    public override string ToString()
    {
        return $"[ sessionId: {SessionId}, type: {RaceEventTypeConverter.FromEnum(Type)}, CurrentLapNumber: {CurrentLapNumber}, CurrentLapTime: {CurrentLapTime} ]";
    }

    public override bool Equals(object obj)
    {
        // If parameter is null return false.
        if (obj == null)
        {
            return false;
        }

        // If parameter cannot be cast to Point return false.
        if (!(obj is SessionReloadEvent other))
        {
            return false;
        }

        // Return true if the fields match:
        return SessionId == other.SessionId &&
               Type == other.Type &&
               CurrentLapNumber == other.CurrentLapNumber &&
               CurrentLapTime == other.CurrentLapTime;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public static bool MatchesRecoveryFileFormat(string line)
    {
        return GetRaceEventTypeFromLine(line) == RaceEventType.SessionReload;
    }
}